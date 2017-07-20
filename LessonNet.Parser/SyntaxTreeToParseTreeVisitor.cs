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
				foreach (var child in context.statement()) {
					yield return (Statement) child.Accept(this);
				}
			}

			return new Stylesheet(GetStatements());
		}

		public override LessNode VisitStatement(LessParser.StatementContext context) {
			ExtendStatement GetExtendStatement() {
				var extend = context.extend();
				if (extend == null) {
					return null;
				}

				return new ExtendStatement(GetExtend(extend));
			}

			return context.importDeclaration()?.Accept(this)
				?? context.variableDeclaration()?.Accept(this)
				?? context.mixinDefinition()?.Accept(this)
				?? context.ruleset()?.Accept(this)
				?? context.mixinCall()?.Accept(this)
				?? context.mediaBlock()?.Accept(this)
				?? context.atRule()?.Accept(this)
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
				if (list?.AND() != null) {
					return new ConjunctionSupportsCondition(negate, conditions);
				}
				if (list?.OR() != null) {
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
			var block = (RuleBlock)context.block().Accept(this);

			return new DocumentAtRule(specifiers, block);
		}

		public override LessNode VisitPageAtRule(LessParser.PageAtRuleContext context) {
			var selector = (Selector) context.selector()?.Accept(this);
			var block = (RuleBlock) context.block().Accept(this);

			return new PageAtRule(selector, block);
		}

		public override LessNode VisitKeyframesAtRule(LessParser.KeyframesAtRuleContext context) {
			var identifier = (Identifier) context.identifier().Accept(this);

			var keyframes = context.keyframesBlock().keyframe().Select(f => (Keyframe)f.Accept(this));

			return new KeyframesAtRule(identifier, keyframes);
		}

		public override LessNode VisitKeyframe(LessParser.KeyframeContext context) {
			string keyword = context.FROM()?.GetText()
				?? context.TO()?.GetText();

			var block = (RuleBlock) context.block().Accept(this);

			if (!string.IsNullOrEmpty(keyword)) {
				return new Keyframe(keyword, block);
			}

			var percentage = new Measurement(decimal.Parse(context.Number().GetText()), "%");

			return new Keyframe(percentage, block);
		}

		public override LessNode VisitGenericAtRule(LessParser.GenericAtRuleContext context) {
			var identifier = (Identifier) context.identifier().Accept(this);
			var value = (Expression) context.valueList()?.Accept(this);
			var block = (RuleBlock) context.block().Accept(this);

			return new GenericAtRule(identifier, value, block);
		}

		public override LessNode VisitVariableName(LessParser.VariableNameContext variable) {
			string GetVariableName(LessParser.VariableNameContext variableName) {
				var id = variableName.Identifier()
					?? variableName.URL();

				return id.ToString();
			}

			var variableVariable = variable.variableName();
			if (variableVariable != null) {
				return new Variable(new Variable(GetVariableName(variableVariable)));
			}

			return new Variable(GetVariableName(variable));
		}

		public override LessNode VisitImportDeclaration(LessParser.ImportDeclarationContext context) {
			var referenceUrlContext = context.referenceUrl();

			string url = referenceUrlContext.GetText().Trim('"');

			return new ImportStatement(url);
		}

		public override LessNode VisitRuleset(LessParser.RulesetContext context) {
			SelectorList selectors = (SelectorList) context.selectors().Accept(this);

			return new Ruleset(selectors, (RuleBlock) context.block().Accept(this));
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

				return new Identifier(new PseudoclassIdentifierPart(prefix, pseudo.Identifier().GetText(), (Expression) pseudo.expression()?.Accept(this)));
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
						return new AttributeSelectorElement(identifier, op.GetText(), (Expression) attrib.attribValue().Accept(this), hasTrailingWhitespace);
					}
					return new AttributeSelectorElement(identifier, hasTrailingWhitespace);
				}

				// The lexer rules might match an ID selector as a color, so we account for that here
				if (context.HexColor() != null) {
					return new IdentifierSelectorElement(new Identifier(new ConstantIdentifierPart(context.HexColor().GetText())), hasTrailingWhitespace);
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
				?? context.@string().Accept(this);
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

			string name = context.variableName().Identifier().GetText();

			return new VariableDeclaration(name, GetExpressionLists(context.valueList(), context.IMPORTANT() != null));
		}
		public override LessNode VisitValueList(LessParser.ValueListContext context) {
			return context.commaSeparatedExpressionList()?.Accept(this)
				?? context.expressionList()?.Accept(this);
		}

		public override LessNode VisitExpressionList(LessParser.ExpressionListContext context) {
			IEnumerable<Expression> GetValues() {
				foreach (var value in context.expression()) {
					yield return (Expression) value.Accept(this);
				}
			}

			return new ExpressionList(GetValues());
		}

		public override LessNode VisitParenthesizedExpression(LessParser.ParenthesizedExpressionContext context) {
			return new ParenthesizedExpression((Expression) context.expression().Accept(this));
		}

		public override LessNode VisitExpression(LessParser.ExpressionContext context) {
			Expression GetMathOperation() {
				if (context.mathCharacter() == null) {
					return null;
				}

				var lhs = (Expression) context.expression(0).Accept(this);
				var rhs = (Expression) context.expression(1).Accept(this);

				return new MathOperation(lhs, context.mathCharacter().GetText(), rhs);
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

			Expression GetFraction() {
				var fraction = context.fraction();
				if (fraction == null) {
					return null;
				}

				var numbers = fraction.Number().Select(n => decimal.Parse(n.GetText())).ToArray();

				return new Fraction(numbers[0], numbers[1], fraction.Unit()?.GetText());
			}

			return context.variableName()?.Accept(this)
				?? GetColor()
				?? context.measurement()?.Accept(this)
				?? GetStringLiteral()
				?? context.function()?.Accept(this)
				?? context.identifier()?.Accept(this)
				?? context.parenthesizedExpression()?.Accept(this)
				?? context.measurementList()?.Accept(this)
				?? GetFraction()
				?? GetMathOperation()
				?? context.url()?.Accept(this)
				?? context.quotedExpression()?.Accept(this)
				?? context.selector()?.Accept(this)
				?? throw new ParserException($"Unexpected expression {context.GetText()}");
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

			return new Url(context.Url().GetText());
		}

		public override LessNode VisitQuotedExpression(LessParser.QuotedExpressionContext context) {
			return new QuotedExpression((LessString) context.@string().Accept(this));
		}

		public override LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context) {
			MixinParameterBase GetParameter(LessParser.MixinDefinitionParamContext param) {
				var variable = param.variableName();
				if (variable != null) {
					return new MixinParameter(variable.GetText().TrimStart('@'), null);
				}
				var variableDeclaration = param.variableDeclaration();
				if (variableDeclaration != null) {
					var decl = (VariableDeclaration) variableDeclaration.Accept(this);
					return new MixinParameter(decl.Name, decl.Values);
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

			IEnumerable<MixinParameterBase> GetParameters() {
				if (context.mixinDefinitionParam() == null) {
					yield break;
				}
				foreach (var param in context.mixinDefinitionParam()) {
					yield return GetParameter(param);
				}
			}

			var selector = (Selector) context.selector().Accept(this);
			var ruleBlock = (RuleBlock) context.block().Accept(this);

			var guard = (MixinGuard) context.mixinGuard()?.Accept(this);

			return new MixinDefinition(selector, GetParameters(), ruleBlock, guard);
		}

		public override LessNode VisitMixinGuard(LessParser.MixinGuardContext context) {
			return new MixinGuard((OrConditionList) context.mixinGuardConditions().Accept(this));
		}

		public override LessNode VisitMixinGuardConditions(LessParser.MixinGuardConditionsContext context) {
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
			IEnumerable<T> GetChildren<T>(IEnumerable<IParseTree> nodes) where T : LessNode {
				foreach (var child in nodes) {
					if (child is LessParser.PropertyContext || child is LessParser.StatementContext) {
						yield return (T) child.Accept(this);
					}
				}
			}


			return new RuleBlock(GetChildren<Statement>(context.children));
		}

		public override LessNode VisitProperty(LessParser.PropertyContext context) {
			string name = context.identifier().GetText();

			return new Rule(name, GetExpressionLists(context.valueList(), context.IMPORTANT() != null));
		}

		public override LessNode VisitMixinCall(LessParser.MixinCallContext context) {
			IEnumerable<MixinCallArgument> GetArguments() {
				bool semicolonSeparated = context.SEMI().Any();
						
				foreach (var arg in context.mixinCallArgument()) {
					var namedArg = arg.variableDeclaration();
					if (namedArg != null) {
						yield return new NamedArgument(namedArg.variableName().Identifier().GetText(), GetExpressionLists(namedArg.valueList()));
					} else {
						var expressionLists = GetExpressionLists(arg.valueList());
						if (semicolonSeparated) {
							yield return new PositionalArgument(expressionLists);
						} else {
							foreach (var expressionList in expressionLists) {
								yield return new PositionalArgument(new ListOfExpressionLists(expressionList, ' '));
							}
						}
					}
				}
			}

			bool important = context.IMPORTANT() != null;

			var selector = (Selector)context.selector().Accept(this);

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
			var modifier = (MediaQueryModifier) Enum.Parse(typeof(MediaQueryModifier), context.MediaQueryModifier()?.GetText() ?? "None", ignoreCase: true);
			var property = context.property();
			if (property != null) {
				return new MediaPropertyQuery(modifier, (Rule) property.Accept(this));
			}

			var value = context.identifier()?.Accept(this)
				?? context.variableName().Accept(this);

			bool parens = context.LPAREN() != null;

			return new MediaIdentifierQuery(modifier, new ExpressionList((Expression) value), parens);
		}

		public override LessNode VisitMeasurementList(LessParser.MeasurementListContext context) {
			return new MeasurementList(context.measurement().Select(m => (Measurement) m.Accept(this)));
		}


		public override LessNode VisitMeasurement(LessParser.MeasurementContext context) {
			return new Measurement(decimal.Parse(context.Number().GetText(), CultureInfo.InvariantCulture), context.Unit()?.GetText());
		}

		public override LessNode VisitFunction(LessParser.FunctionContext context) {
			return FunctionResolver.Resolve(context.functionName().GetText(), GetExpressionLists(context.valueList()));
		}

		private ListOfExpressionLists GetExpressionLists(LessParser.ValueListContext valueList, bool important = false) {
			if (valueList == null) {
				return null;
			}

			var commaSeparatedExpressionListContext = valueList.commaSeparatedExpressionList();

			if (commaSeparatedExpressionListContext != null) {
				var lists = commaSeparatedExpressionListContext.expressionList().Select(l => (ExpressionList) l.Accept(this));
				return new ListOfExpressionLists(lists, ',', important);
			} else {
				return new ListOfExpressionLists(new[] {(ExpressionList) valueList.expressionList().Accept(this)}, ' ', important);
			}
		}

	}
}