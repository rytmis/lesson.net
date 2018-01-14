using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LessonNet.Grammar;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.ParseTree.Mixins;

namespace LessonNet.Parser {
	internal class SyntaxTreeToParseTreeVisitor : LessEvaluatorVisitorBase {
		private readonly ITokenStream tokenStream;

		public SyntaxTreeToParseTreeVisitor(ITokenStream tokenStream) {
			this.tokenStream = tokenStream;
		}

		public override LessNode VisitStylesheet(LessParser.StylesheetContext context) {
			IEnumerable<Statement> GetStatements() {
				foreach (var child in context.terminatedStatement()) {
					yield return (Statement) child.Accept(this);
				}

				var unterminatedStatement = context.statement();
				if (unterminatedStatement != null) {
					yield return (Statement) unterminatedStatement.Accept(this);
				}
			}

			return new Stylesheet(GetStatements());
		}

		public override LessNode VisitStatement(LessParser.StatementContext context) {
			return context.blockStatement()?.Accept(this)
				?? context.lineStatement()?.Accept(this)
				?? throw new ParserException($"Unexpected statement type: [{context.GetText()}]");
		}

		public override LessNode VisitTerminatedStatement(LessParser.TerminatedStatementContext context) {
			return context.blockStatement()?.Accept(this)
				?? context.lineStatement()?.Accept(this)
				?? throw new ParserException($"Unexpected statement type: [{context.GetText()}]");
		}

		public override LessNode VisitBlockStatement(LessParser.BlockStatementContext context) {
			return context.mixinDefinition()?.Accept(this)
				?? context.ruleset()?.Accept(this)
				?? context.mediaBlock()?.Accept(this)
				?? context.atRule()?.Accept(this);
		}

		public override LessNode VisitLineStatement(LessParser.LineStatementContext context) {
			ExtendStatement GetExtendStatement() {
				var extend = context.extend();
				if (extend == null) {
					return null;
				}

				return new ExtendStatement(GetExtend(extend));
			}

			return context.importDeclaration()?.Accept(this)
				?? context.variableDeclaration()?.Accept(this)
				?? context.mixinCall()?.Accept(this)
				?? context.property()?.Accept(this)
				?? GetExtendStatement()
				?? throw new ParserException($"Unexpected statement type: [{context.GetText()}]");
		}

		public override LessNode VisitAtRule(LessParser.AtRuleContext atRule) {
			return atRule.toplevelAtRule()?.Accept(this)
				?? atRule.nestedAtRule()?.Accept(this);
		}

		public override LessNode VisitToplevelAtRule(LessParser.ToplevelAtRuleContext context) {
			var charset = context.charsetAtRule();
			if (charset != null) {
				return new CharsetAtRule((LessString) charset.@string().Accept(this));
			}

			var ns = context.namespaceAtRule();

			var identifier = (Identifier) ns.identifier().Accept(this);
			var namespaceExpr = (Expression) (ns.@string().Accept(this) ?? ns.url().Accept(this));

			return new NamespaceAtRule(identifier, namespaceExpr);
		}

		public override LessNode VisitNestedAtRule(LessParser.NestedAtRuleContext context) {
			return context.supportsAtRule()?.Accept(this)
				?? context.documentAtRule()?.Accept(this)
				?? context.pageAtRule()?.Accept(this)
				?? context.keyframesAtRule()?.Accept(this)
				?? context.genericAtRule()?.Accept(this);
		}

		public override LessNode VisitSupportsAtRule(LessParser.SupportsAtRuleContext context) {
			SupportsCondition GetConditionList(LessParser.SupportsConditionListContext list, bool negate) {
				var conditions = list?.supportsCondition().Select(GetCondition);
				if (list?.AND().Length > 0) {
					return new ConjunctionSupportsCondition(negate, conditions);
				}

				if (list?.OR().Length > 0) {
					return new DisjunctionSupportsCondition(negate, conditions);
				}

				return null;
			}

			SupportsCondition GetCondition(LessParser.SupportsConditionContext supports) {
				var negate = supports.NOT() != null;

				return GetConditionList(supports.supportsConditionList(), negate)
					?? new PropertySupportsCondition(negate, (Rule) supports.property().Accept(this));
			}

			var condition = GetConditionList(context.supportsDeclaration().supportsConditionList(), false)
				?? GetCondition(context.supportsDeclaration().supportsCondition());

			var block = (RuleBlock) context.block().Accept(this);

			return new SupportsAtRule(condition, block);
		}

		public override LessNode VisitDocumentAtRule(LessParser.DocumentAtRuleContext context) {
			var specifiers = context.documentSpecifierList().documentSpecifier().Select(d => (Expression) d.Accept(this));
			var block = (RuleBlock) context.block().Accept(this);

			return new DocumentAtRule(specifiers, block);
		}

		public override LessNode VisitPageAtRule(LessParser.PageAtRuleContext context) {
			var selector = (Selector) context.selector()?.Accept(this);
			var block = (RuleBlock) context.block().Accept(this);

			return new PageAtRule(selector, block);
		}

		public override LessNode VisitKeyframesAtRule(LessParser.KeyframesAtRuleContext context) {
			var identifier = (Identifier) context.identifier().Accept(this);

			var keyframes = context.keyframesBlock().keyframe().Select(f => (Keyframe) f.Accept(this));

			var ruleIdentifier = context.KEYFRAMES().GetText();

			return new KeyframesAtRule(ruleIdentifier, identifier, keyframes);
		}

		public override LessNode VisitKeyframe(LessParser.KeyframeContext context) {
			var expressions = context.singleValuedExpression().Select(GetSingleValuedExpression);

			var block = (RuleBlock) context.block().Accept(this);

			return new Keyframe(expressions, block);
		}

		public override LessNode VisitGenericAtRule(LessParser.GenericAtRuleContext context) {
			var identifier = (Identifier) context.identifier().Accept(this);
			var value = (Expression) context.expression()?.Accept(this);
			var block = (RuleBlock) context.block().Accept(this);

			return new GenericAtRule(identifier, value, block);
		}

		public override LessNode VisitVariableName(LessParser.VariableNameContext variable) {
			string GetVariableName(LessParser.VariableNameContext variableName) {
				return variableName.GetText().TrimStart('@');
			}

			var variableVariable = variable.variableName();
			if (variableVariable != null) {
				return new Variable(new Variable(GetVariableName(variableVariable)));
			}

			return new Variable(GetVariableName(variable));
		}

		public override LessNode VisitImportDeclaration(LessParser.ImportDeclarationContext context) {
			Expression GetImportTarget() {
				var str = context.referenceUrl().@string();
				if (str != null) {
					return (Expression) str.Accept(this);
				}

				return (Url) context.referenceUrl().url().Accept(this);
			}

			return new ImportStatement(GetImportTarget());
		}

		public override LessNode VisitRuleset(LessParser.RulesetContext context) {
			var selectors = (SelectorList) context.selectors().Accept(this);
			var guard = (RulesetGuard) context.rulesetGuard()?.Accept(this);

			return new Ruleset(selectors, guard, (RuleBlock) context.block().Accept(this));
		}

		public override LessNode VisitRulesetGuard(LessParser.RulesetGuardContext context) {
			return new RulesetGuard((OrConditionList) context.guardConditions().Accept(this));
		}

		private IEnumerable<IdentifierPart> GetIdentifierParts(string prefix, LessParser.IdentifierContext idContext) {
			if (!string.IsNullOrEmpty(prefix)) {
				yield return new ConstantIdentifierPart(prefix);
			}

			var identifier = idContext.Identifier() ?? idContext.IdentifierAfter();
			if (identifier != null) {
				yield return new ConstantIdentifierPart(identifier.GetText());
			}

			if (idContext.keywordAsIdentifier() != null) {
				yield return new ConstantIdentifierPart(idContext.keywordAsIdentifier().GetText());
			}

			var variableInterpolation = idContext.variableInterpolation();
			if (variableInterpolation != null) {
				yield return new InterpolatedVariableIdentifierPart(variableInterpolation.identifierVariableName().GetText());
			}

			foreach (var partContext in idContext.identifierPart()) {
				if (partContext.IdentifierAfter() != null) {
					yield return new ConstantIdentifierPart(partContext.IdentifierAfter().GetText());
				}

				if (partContext.identifierVariableName() != null) {
					yield return new InterpolatedVariableIdentifierPart(partContext.identifierVariableName().GetText());
				}
			}
		}


		public override LessNode VisitSelectorElement(LessParser.SelectorElementContext context) {
			IEnumerable<IdentifierPart> GetIdentifierParts() {
				string prefix = context.HASH()?.GetText()
					?? context.DOT()?.GetText();

				foreach (var identifierPart in this.GetIdentifierParts(prefix, context.identifier())) {
					yield return identifierPart;
				}
			}

			Identifier GetPseudoclassIdentifier() {
				var pseudo = context.pseudoClass();
				if (pseudo == null) {
					return null;
				}

				string prefix = pseudo.COLON()?.GetText()
					?? pseudo.COLONCOLON()?.GetText()
					?? "";

				return new Identifier(new PseudoclassIdentifierPart(prefix, pseudo.pseudoclassIdentifier().GetText(),
					(Expression) pseudo.expression()?.Accept(this)));
			}

			Identifier GetIdentifier() {
				return new Identifier(GetIdentifierParts());
			}

			SelectorElement GetElement(bool hasTrailingWhitespace) {
				var parentSelector = context.parentSelectorReference();
				if (parentSelector != null) {
					return new ParentReferenceSelectorElement(hasTrailingWhitespace);
				}

				if (context.pseudoClass() != null) {
					return new IdentifierSelectorElement(GetPseudoclassIdentifier(), hasTrailingWhitespace);
				}

				if (context.identifier() != null) {
					return new IdentifierSelectorElement(GetIdentifier(), hasTrailingWhitespace);
				}

				var attrib = context.attrib();
				if (attrib != null) {
					var identifier = (Identifier) attrib.identifier().Accept(this);

					var op = attrib.attribRelate();
					if (op != null) {
						return new AttributeSelectorElement(identifier, op.GetText(), (Expression) attrib.attribValue().Accept(this),
							hasTrailingWhitespace);
					}

					return new AttributeSelectorElement(identifier, hasTrailingWhitespace);
				}

				// The lexer rules might match an ID selector as a color, so we account for that here
				if (context.HexColor() != null) {
					return new IdentifierSelectorElement(new Identifier(new ConstantIdentifierPart(context.HexColor().GetText())),
						hasTrailingWhitespace);
				}

				return new CombinatorSelectorElement(context.combinator().GetText(), hasTrailingWhitespace);
			}

			int possibleWhitespaceIndex = context.Stop.TokenIndex + 1;

			bool trailingWhitespace = possibleWhitespaceIndex < tokenStream.Size
				&& tokenStream.Get(possibleWhitespaceIndex).Type == LessLexer.WS;

			return GetElement(trailingWhitespace);
		}

		public override LessNode VisitAttribValue(LessParser.AttribValueContext context) {
			return context.identifier()?.Accept(this)
				?? context.@string()?.Accept(this)
				?? context.measurement().Accept(this);
		}

		public override LessNode VisitIdentifier(LessParser.IdentifierContext context) {
			return new Identifier(GetIdentifierParts(null, context));
		}

		public override LessNode VisitSelector(LessParser.SelectorContext context) {
			return GetSelector(context.selectorElement());
		}

		public Extend GetExtend(LessParser.ExtendContext context) {
			IEnumerable<Extender> GetExtenders() {
				foreach (var extender in context.extenderList().extender()) {
					var elements = extender.selector().selectorElement();
					bool partialMatch = elements.LastOrDefault()?.GetText() == "all";
					var selectorElements = partialMatch ? elements.Take(elements.Length - 1) : elements;

					yield return new Extender(GetSelector(selectorElements), partialMatch);
				}
			}

			if (context == null) {
				return null;
			}

			return new Extend(GetExtenders());
		}

		private Selector GetSelector(IEnumerable<LessParser.SelectorElementContext> selectorElements, Extend extend = null) {
			IEnumerable<SelectorElement> GetElements() {
				foreach (var element in selectorElements) {
					yield return (SelectorElement) element.Accept(this);
				}
			}

			return new Selector(GetElements(), extend);
		}

		public override LessNode VisitSelectors(LessParser.SelectorsContext context) {
			IEnumerable<Selector> GetSelectors() {
				foreach (var selectorContext in context.selectorListElement()) {
					yield return GetSelector(selectorContext.selector().selectorElement(), GetExtend(selectorContext.extend()));
				}
			}

			return new SelectorList(GetSelectors());
		}

		public override LessNode VisitVariableDeclaration(LessParser.VariableDeclarationContext context) {

			string name = ((Variable) context.variableName().Accept(this)).Name;

			var value = GetValue(context.expression());
			var important = context.IMPORTANT() != null;
			if (important) {
				return new VariableDeclaration(name, new ImportantExpression(value));
			}

			return new VariableDeclaration(name, value);
		}

		public override LessNode VisitParenthesizedExpression(LessParser.ParenthesizedExpressionContext context) {
			return new ParenthesizedExpression((Expression) context.expression().Accept(this));
		}

		public override LessNode VisitExpression(LessParser.ExpressionContext context) {
			return GetExpression(context);
		}

		private Expression GetExpression(LessParser.ExpressionContext context) {
			Expression GetExpressionOrList(int start, int end) {
				int itemCount = end - start;
				if (itemCount == 1) {
					return GetSingleValuedExpression((LessParser.SingleValuedExpressionContext) context.GetChild(start));
				}

				var childExpressions = context.children
					.Skip(start)
					.Take(itemCount)
					.Select(c => GetSingleValuedExpression((LessParser.SingleValuedExpressionContext) c));

				return new ExpressionList(childExpressions, ' ');
			}

			IEnumerable<Expression> GetCommaSeparatedExpressions() {
				var commaStops = context.children
					.Select((t, i) => (Node: t, Index: i))
					.Where(p => p.Node is ITerminalNode)
					.Select(p => p.Index);

				int current = 0;
				foreach (var commaStop in commaStops) {
					yield return GetExpressionOrList(current, commaStop);
					current = commaStop + 1;
				}

				yield return GetExpressionOrList(current, context.children.Count);
			}

			if (context == null) {
				return null;
			}

			var commas = context.COMMA();
			if (commas.Length > 0) {
				return new ExpressionList(GetCommaSeparatedExpressions(), ',');
			}

			var expressions = context.singleValuedExpression();
			if (expressions.Length > 1) {
				return new ExpressionList(expressions.Select(GetSingleValuedExpression), ' ');
			}

			return GetSingleValuedExpression(expressions[0]);
		}

		private Expression GetSingleValuedExpression(LessParser.SingleValuedExpressionContext context) {
			Expression GetMathOperation() {
				var mathOperation = context.op;
				if (mathOperation == null) {
					return null;
				}

				var lhs = GetSingleValuedExpression(context.singleValuedExpression(0));
				var rhs = GetSingleValuedExpression(context.singleValuedExpression(1));

				return new MathOperation(lhs, mathOperation.Text, rhs, keepSpaces: HasLeadingWhitespace(mathOperation) && HasTrailingWhitespace(mathOperation));
			}

			Expression GetColor() {
				var color = context.color();
				if (color == null) {
					return null;
				}

				var hexColor = color.HexColor();
				if (hexColor != null) {
					return Color.FromHexString(hexColor.GetText());
				}

				return Color.FromKeyword(color.KnownColor().GetText());
			}

			Expression GetStringLiteral() {
				return (LessString) context.@string()?.Accept(this);
			}

			Expression GetBoolean() {
				var boolean = context.booleanValue();
				if (boolean == null) {
					return null;
				}

				return new BooleanValue(string.Equals("true", boolean.GetText()));
			}

			if (context == null) {
				return null;
			}

			LessNode result = context.variableName()?.Accept(this)
				?? GetColor()
				?? context.measurement()?.Accept(this)
				?? GetStringLiteral()
				?? context.escapeSequence()?.Accept(this)
				?? context.function()?.Accept(this)
				?? context.identifier()?.Accept(this)
				?? context.parenthesizedExpression()?.Accept(this)
				?? GetMathOperation()
				?? context.url()?.Accept(this)
				?? context.quotedExpression()?.Accept(this)
				?? context.selector()?.Accept(this)
				?? GetBoolean()
				?? throw new ParserException($"Unexpected expression {context.GetText()}");

			return (Expression) result;
		}

		public override LessNode VisitString(LessParser.StringContext context) {
			char quote = context.SQUOT_STRING_START() != null ? '\'' : '"';

			return new LessString(quote, GetFragments());

			IEnumerable<LessStringFragment> GetFragments() {
				// First and last character are quotes, don't include them
				var nonQuoteParts = context.children.Skip(1).Take(context.children.Count - 2);

				foreach (var strChild in nonQuoteParts) {
					if (strChild is LessParser.VariableInterpolationContext interpolation) {
						yield return new InterpolatedVariable(new Variable(interpolation.identifierVariableName().GetText()));
					} else {
						yield return new LessStringLiteral(strChild.GetText());
					}
				}
			}
		}

		public override LessNode VisitUrl(LessParser.UrlContext context) {
			var variableContent = (Variable) context.variableName()?.Accept(this);
			if (variableContent != null) {
				return new Url(variableContent);
			}

			var stringContent = (LessString) context.@string()?.Accept(this);
			if (stringContent != null) {
				return new Url(stringContent);
			}

			return new Url(context.Url()?.GetText());
		}

		public override LessNode VisitQuotedExpression(LessParser.QuotedExpressionContext context) {
			return new QuotedExpression((LessString) context.@string().Accept(this));
		}

		public override LessNode VisitEscapeSequence(LessParser.EscapeSequenceContext context) {
			return new EscapeSequence(context.EscapeSequence().ToString());
		}

		public override LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context) {
			MixinParameterBase GetParameter(LessParser.MixinDefinitionParamContext param) {
				var ellipsis = param.Ellipsis();
				var variable = param.variableName();
				if (variable != null) {
					var paramName = variable.GetText().TrimStart('@');

					if (ellipsis != null) {
						return new NamedVarargsParameter(paramName);
					}

					return new MixinParameter(paramName, null);
				}

				if (ellipsis != null) {
					return VarargsParameter.Instance;
				}

				var variableDeclaration = param.variableDeclaration();
				if (variableDeclaration != null) {
					var decl = (VariableDeclaration) variableDeclaration.Accept(this);

					return new MixinParameter(decl.Name, decl.Value);
				}

				var str = param.@string();
				if (str != null) {
					return new PatternMatchParameter((LessString) str.Accept(this));
				}

				var number = param.Number();
				if (number != null) {
					return new PatternMatchParameter(new Measurement(decimal.Parse(number.GetText()), ""));
				}

				return new PatternMatchParameter((Identifier) param.identifier().Accept(this));
			}

			IEnumerable<MixinParameterBase> GetParameters(LessParser.MixinDeclarationContext decl) {
				var parameters = decl.mixinDefinitionParam();
				if (parameters == null) {
					yield break;
				}

				foreach (var param in parameters) {
					yield return GetParameter(param);
				}
			}

			var declaration = context.mixinDeclaration();
			var selector = (Selector) declaration.selector().Accept(this);
			var ruleBlock = (RuleBlock) context.block().Accept(this);

			var guard = (MixinGuard) context.mixinGuard()?.Accept(this);

			return new MixinDefinition(selector, GetParameters(declaration), ruleBlock, guard);
		}

		public override LessNode VisitMixinGuard(LessParser.MixinGuardContext context) {
			if (context.mixinGuardDefault() != null) {
				return DefaultMixinGuard.Instance;
			}

			return new ConditionMixinGuard((OrConditionList) context.guardConditions().Accept(this));
		}

		public override LessNode VisitGuardConditions(LessParser.GuardConditionsContext context) {
			var conditionLists = context.conditionList().Select(cl => (AndConditionList) cl.Accept(this));

			return new OrConditionList(conditionLists);
		}

		public override LessNode VisitConditionList(LessParser.ConditionListContext context) {
			var conditions = context.condition().Select(c => (Condition) c.Accept(this));

			return new AndConditionList(conditions);
		}

		public override LessNode VisitCondition(LessParser.ConditionContext context) {
			bool negate = context.NOT() != null;

			var conditionStatement = context.conditionStatement();
			var comparison = conditionStatement.comparison();
			if (comparison != null) {
				var lhs = (Expression) comparison.expression(0).Accept(this);
				string op = comparison.comparisonOperator().GetText();
				var rhs = (Expression) comparison.expression(1).Accept(this);

				return new ComparisonCondition(negate, lhs, op, rhs);
			}

			return new BooleanExpressionCondition(negate, (Expression) conditionStatement.expression().Accept(this));
		}

		public override LessNode VisitBlock(LessParser.BlockContext context) {
			IEnumerable<Statement> GetStatements() {
				foreach (var statement in context.terminatedStatement()) {
					yield return (Statement) statement.Accept(this);
				}

				var unterminatedStatement = context.statement();
				if (unterminatedStatement != null) {
					yield return (Statement) unterminatedStatement.Accept(this);
				}
			}


			return new RuleBlock(GetStatements());
		}

		private Expression GetValue(LessParser.ExpressionContext[] expressions) {
			var values = expressions.Select(GetExpression).ToList();
			if (values.Count == 1) {
				return values[0];
			}

			return new ExpressionList(values, ' ');
		}

		public override LessNode VisitProperty(LessParser.PropertyContext context) {
			string name = context.identifier().GetText();

			var expr = (Expression) context.expression()?.Accept(this)
				?? (Expression) context.ieFilter()?.Accept(this);

			var important = context.IMPORTANT() != null;
			if (important) {
				return new Rule(name, new ImportantExpression(expr));
			}

			return new Rule(name, expr);
		}

		public override LessNode VisitIeFilter(LessParser.IeFilterContext context) {
			var identifier = context.ieFilterIdentifier().GetText();

			var expressions = context.ieFilterExpression().Select(fe => (IeFilterExpression) fe.Accept(this));

			return new IeFilter(identifier, expressions);
		}

		public override LessNode VisitIeFilterExpression(LessParser.IeFilterExpressionContext context) {
			var identifier = (Identifier) context.identifier().Accept(this);

			var value = GetSingleValuedExpression(context.singleValuedExpression());

			return new IeFilterExpression(identifier, value);
		}

		public override LessNode VisitMixinCall(LessParser.MixinCallContext context) {
			IEnumerable<MixinCallArgument> GetArguments() {
				foreach (var arg in context.mixinCallArgument()) {
					var namedArg = arg.variableDeclaration();
					var expression = GetExpression(arg.expression()) ?? GetValue(namedArg?.expression());

					if (namedArg != null) {
						yield return new NamedArgument(namedArg.variableName().identifier().GetText(), expression);
					} else {
						yield return new PositionalArgument(expression);
					}
				}
			}


			bool important = context.IMPORTANT() != null;

			var selector = (Selector) context.selector().Accept(this);

			if (context.LPAREN() != null) {
				return new MixinCall(selector, GetArguments(), important);
			}

			return new RulesetCall(selector, important);
		}

		public override LessNode VisitMediaBlock(LessParser.MediaBlockContext context) {
			var queries = context.mediaQuery().Select(q => (MediaQuery) q.Accept(this));
			var block = (RuleBlock) context.block().Accept(this);

			return new MediaBlock(queries, block);
		}

		public override LessNode VisitMediaQuery(LessParser.MediaQueryContext context) {
			var featureQueries = context.featureQuery().Select(f => (MediaFeatureQuery) f.Accept(this));

			return new MediaQuery(featureQueries);
		}

		public override LessNode VisitFeatureQuery(LessParser.FeatureQueryContext context) {
			var modifier = (MediaQueryModifier) Enum.Parse(typeof(MediaQueryModifier),
				context.mediaQueryModifier()?.GetText() ?? "None", ignoreCase: true);
			var property = context.property();
			if (property != null) {
				return new MediaPropertyQuery(modifier, (Rule) property.Accept(this));
			}

			var value = context.identifier()?.Accept(this)
				?? context.variableName().Accept(this);

			bool parens = context.LPAREN() != null;

			return new MediaIdentifierQuery(modifier, new ExpressionList((Expression) value, ' '), parens);
		}

		public override LessNode VisitMeasurement(LessParser.MeasurementContext context) {
			string unit = context.unit()?.GetText()
				?? context.identifier()?.GetText();

			return new Measurement(decimal.Parse(context.Number().GetText(), CultureInfo.InvariantCulture), unit);
		}

		public override LessNode VisitFunction(LessParser.FunctionContext context) {
			return FunctionResolver.Resolve(context.functionName().GetText(), GetExpression(context.expression()));
		}

		private bool IsWhitespace(IToken token, int relativePosition) {
				int possibleWhitespaceIndex = token.TokenIndex + relativePosition;

			return possibleWhitespaceIndex <= tokenStream.Size
				&& tokenStream.Get(possibleWhitespaceIndex).Type == LessLexer.WS;
		}

		private bool HasLeadingWhitespace(IToken token) => IsWhitespace(token, -1);
		private bool HasTrailingWhitespace(IToken token) => IsWhitespace(token, 1);
	}

}