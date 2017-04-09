using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using LessonNet.Grammar;
using LessonNet.Parser.ParseTree;

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

		public override LessNode VisitSelectorElement(LessParser.SelectorElementContext context) {
			int possibleWhitespaceIndex = context.Stop.TokenIndex + 1;

			bool hasTrailingWhitespace = possibleWhitespaceIndex < tokenStream.Size
				&& tokenStream.Get(possibleWhitespaceIndex).Type == LessLexer.WS;

			var parentSelector = context.parentSelectorReference();
			if (parentSelector != null) {
				return new ParentReferenceSelectorElement() {
					HasTrailingWhitespace = hasTrailingWhitespace
				};
			}

			return new SelectorElement(context.GetText()) {
				HasTrailingWhitespace = hasTrailingWhitespace
			};
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
			MathOperation GetMathOperation() {
				if (context.mathCharacter() == null) {
					return null;
				}

				var lhs = (Expression) context.expression(0).Accept(this);
				var rhs = (Expression) context.expression(1).Accept(this);

				return new MathOperation(lhs, context.mathCharacter().GetText(), rhs);
			}

			return context.variableName()?.Accept(this)
				?? context.Color()?.Accept(this)
				?? context.measurement()?.Accept(this)
				?? context.StringLiteral()?.Accept(this)
				?? context.function()?.Accept(this)
				?? context.identifier()?.Accept(this)
				?? context.parenthesizedExpression()?.Accept(this)
				?? GetMathOperation()
				?? throw new ParserException($"Unexpected expression {context.GetText()}");
		}

		public override LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context) {
			var selectors = (SelectorList) context.selectors().Accept(this);
			var ruleBlock = (RuleBlock) context.block().Accept(this);

			// TODO: Handle the extra '@' symbol in the parser grammar
			var arguments = context.mixinDefinitionParam()
					?.Select(p => p.variableName().GetText().TrimStart('@'))
				?? Enumerable.Empty<string>();

			var guard = (MixinGuard) context.mixinGuard()?.Accept(this);

			return new MixinDefinition(selectors, arguments, ruleBlock, guard);
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
					yield return (T) child.Accept(this);
				}
			}

			return new RuleBlock(
				GetChildren<Rule>(context.property()), 
				GetChildren<Statement>(context.statement()));
		}

		public override LessNode VisitProperty(LessParser.PropertyContext context) {
			string name = context.identifier().GetText();

			return new Rule(name, GetExpressionLists(context.valueList()));
		}

		public override LessNode VisitMixinCall(LessParser.MixinCallContext context) {
			IEnumerable<MixinCallArgument> GetArguments() {
				var expressionLists = context.commaSeparatedExpressionList();
				if (expressionLists.Length == 1) {
					// Single comma-separated list of expressions (e.g. 10px 10px, 20px 20px)
					// --> each comma-separated list is an argument
					foreach (var list in expressionLists[0].expressionList()) {
						yield return new MixinCallArgument((ExpressionList) list.Accept(this));
					}
					yield break;
				}

				// Semicolon-separated list of potentially comma-separated lists
				foreach (var expressionList in expressionLists) {
					yield return new MixinCallArgument((ListOfExpressionLists) expressionList.Accept(this));
				}
			}

			var selector = (SelectorList)context.selectors().Accept(this);

			return new MixinCall(selector, GetArguments());
		}

		public override LessNode VisitCommaSeparatedExpressionList(LessParser.CommaSeparatedExpressionListContext context) {
			IEnumerable<ExpressionList> GetExpressionLists() {
				foreach (var value in context.expressionList()) {
					yield return (ExpressionList) value.Accept(this);
				}
			}

			return new ListOfExpressionLists(GetExpressionLists());
		}

		public override LessNode VisitMediaBlock(LessParser.MediaBlockContext context) {
			var queries = context.mediaQuery().Select(q => (MediaQuery) q.Accept(this));
			var block = (RuleBlock) context.block().Accept(this);

			return new MediaBlock(queries, block);
		}

		public override LessNode VisitMediaQuery(LessParser.MediaQueryContext context) {
			var modifier = (MediaQueryModifier) Enum.Parse(typeof(MediaQueryModifier), context.MediaQueryModifier()?.GetText() ?? "None", ignoreCase: true);
			var featureQueries = context.featureQuery().Select(f => (MediaFeatureQuery) f.Accept(this));

			return new MediaQuery(modifier, featureQueries);
		}

		public override LessNode VisitFeatureQuery(LessParser.FeatureQueryContext context) {
			var property = context.property();
			if (property != null) {
				return new MediaPropertyQuery((Rule) property.Accept(this));
			}

			return new MediaIdentifierQuery(context.identifier().GetText());
		}

		public override LessNode VisitMeasurement(LessParser.MeasurementContext context) {
			return new Measurement(decimal.Parse(context.Number().GetText()), context.Unit()?.GetText());
		}

		private IEnumerable<ExpressionList> GetExpressionLists(LessParser.ValueListContext valueList) {

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