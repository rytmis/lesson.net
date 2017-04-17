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
			return context.importDeclaration()?.Accept(this)
				?? context.variableDeclaration()?.Accept(this)
				?? context.mixinDefinition()?.Accept(this)
				?? context.ruleset()?.Accept(this)
				?? context.mixinCall()?.Accept(this)
				?? context.mediaBlock()?.Accept(this)
				?? throw new ParserException($"Unexpected statement type: [{context.GetText()}]");
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
			if (idContext.Identifier() != null) {
				yield return new ConstantIdentifierPart(prefix + idContext.Identifier().GetText());
			}

			if (idContext.KnownColor() != null) {
				yield return new ConstantIdentifierPart(prefix + idContext.KnownColor().GetText());
			}

			if (idContext.identifierVariableName() != null) {
				yield return new InterpolatedVariableIdentifierPart(idContext.identifierVariableName().GetText());
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
				string prefix = context.COLON()?.GetText()
					?? context.COLONCOLON()?.GetText()
					?? "";

				return new Identifier(new ConstantIdentifierPart(prefix + context.Identifier().GetText()));
			}

			Identifier GetIdentifier() {
				if (context.Identifier() != null) {
					return GetPseudoclassIdentifier();
				}

				return new Identifier(GetIdentifierParts());
			}

			SelectorElement GetElement() {
				var parentSelector = context.parentSelectorReference();
				if (parentSelector != null) {
					return new ParentReferenceSelectorElement();
				}

				if (context.identifier() != null || context.Identifier() != null) {
					return new IdentifierSelectorElement(GetIdentifier());
				}

				if (context.attrib() != null) {
					return new AttributeSelectorElement(context.attrib().GetText());
				}

				// The lexer rules might match an ID selector as a color, so we account for that here
				if (context.HexColor() != null) {
					return new IdentifierSelectorElement(new Identifier(new ConstantIdentifierPart(context.HexColor().GetText())));
				}

				return new CombinatorSelectorElement(context.combinator().GetText());
			}

			int possibleWhitespaceIndex = context.Stop.TokenIndex + 1;

			bool hasTrailingWhitespace = possibleWhitespaceIndex < tokenStream.Size
				&& tokenStream.Get(possibleWhitespaceIndex).Type == LessLexer.WS;

			var element = GetElement();
			element.HasTrailingWhitespace = hasTrailingWhitespace;
			return element;
		}

		public override LessNode VisitIdentifier(LessParser.IdentifierContext context) {
			return new Identifier(GetIdentifierParts(null, context));
		}

		public override LessNode VisitSelector(LessParser.SelectorContext context) {
			IEnumerable<SelectorElement> GetElements() {
				SelectorElement lastSelector = null;
				foreach (var element in context.selectorElement()) {
					lastSelector = (SelectorElement) element.Accept(this);
					yield return lastSelector;
				}

				if (lastSelector != null) {
					lastSelector.HasTrailingWhitespace = true;
				}
			}

			return new Selector(GetElements());
		}

		public override LessNode VisitSelectors(LessParser.SelectorsContext context) {
			IEnumerable<Selector> GetSelectors() {
				foreach (var selectorContext in context.selector()) {
					yield return (Selector) selectorContext.Accept(this);
				}
			}

			return new SelectorList(GetSelectors());
		}

		public override LessNode VisitVariableDeclaration(LessParser.VariableDeclarationContext context) {

			string name = context.variableName().Identifier().GetText();

			return new VariableDeclaration(name, GetExpressionLists(context.valueList()));
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
				if (context.StringLiteral() == null) {
					return null;
				}

				return new StringLiteral(context.StringLiteral().GetText());
			}

			return context.variableName()?.Accept(this)
				?? GetColor()
				?? context.measurement()?.Accept(this)
				?? GetStringLiteral()
				?? context.function()?.Accept(this)
				?? context.identifier()?.Accept(this)
				?? context.parenthesizedExpression()?.Accept(this)
				?? context.measurementList()?.Accept(this)
				?? GetMathOperation()
				?? context.url()?.Accept(this)
				?? context.quotedExpression()?.Accept(this)
				?? throw new ParserException($"Unexpected expression {context.GetText()}");
		}

		public override LessNode VisitUrl(LessParser.UrlContext context) {
			return new Url(context.Url().GetText());
		}

		public override LessNode VisitQuotedExpression(LessParser.QuotedExpressionContext context) {
			return new QuotedExpression(context.StringLiteral().GetText());
		}

		public override LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context) {
			IEnumerable<MixinParameterBase> GetParameters() {
				if (context.mixinDefinitionParam() == null) {
					yield break;
				}
				foreach (var param in context.mixinDefinitionParam()) {
					if (param.variableName() != null) {
						yield return new MixinParameter(param.variableName().GetText().TrimStart('@'), null);
					} else if (param.variableDeclaration() != null) {
						var decl = (VariableDeclaration) param.variableDeclaration().Accept(this);
						yield return new MixinParameter(decl.Name, decl.Values);
					} else {
						yield return new PatternMatchParameter((Identifier) param.identifier().Accept(this));
					}
				}
			}

			var selectors = (SelectorList) context.selectors().Accept(this);
			var ruleBlock = (RuleBlock) context.block().Accept(this);

			var guard = (MixinGuard) context.mixinGuard()?.Accept(this);

			return new MixinDefinition(selectors, GetParameters(), ruleBlock, guard);
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

			return new Rule(name, GetExpressionLists(context.valueList()), context.IMPORTANT() != null);
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
								yield return new PositionalArgument(expressionList);
							}
						}
					}
				}
			}

			var selector = (SelectorList)context.selectors().Accept(this);

			if (context.LPAREN() != null) {
				return new MixinCall(selector, GetArguments());
			}

			return new RulesetCall(selector);
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

			return new MediaIdentifierQuery(modifier, context.identifier().GetText());
		}

		public override LessNode VisitMeasurementList(LessParser.MeasurementListContext context) {
			return new MeasurementList(context.measurement().Select(m => (Measurement) m.Accept(this)));
		}


		public override LessNode VisitMeasurement(LessParser.MeasurementContext context) {
			return new Measurement(decimal.Parse(context.Number().GetText(), CultureInfo.InvariantCulture), context.Unit()?.GetText());
		}

		public override LessNode VisitFunction(LessParser.FunctionContext context) {
			return new Function(context.functionName().GetText(), GetExpressionLists(context.valueList()));
		}

		private IEnumerable<ExpressionList> GetExpressionLists(LessParser.ValueListContext valueList) {
			if (valueList == null) {
				yield break;
			}

			var commaSeparatedExpressionListContext = valueList.commaSeparatedExpressionList();

			if (commaSeparatedExpressionListContext != null) {
				foreach (var value in commaSeparatedExpressionListContext.expressionList()) {
					yield return (ExpressionList) value.Accept(this);
				}
			} else {
				yield return (ExpressionList) valueList.expressionList().Accept(this);
			}
		}

	}
}