using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime.Tree;
using LessonNet.Grammar;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser {
	internal class SyntaxTreeToParseTreeVisitor : LessEvaluatorVisitorBase {
		public override LessNode VisitStylesheet(LessParser.StylesheetContext context) {
			Stylesheet stylesheet = new Stylesheet();

			foreach (LessParser.StatementContext child in context.statement()) {
				stylesheet.Add((Statement) child.Accept(this));
			}

			return stylesheet;
		}

		public override LessNode VisitStatement(LessParser.StatementContext context) {
			return context.importDeclaration()?.Accept(this)
				?? context.variableDeclaration()?.Accept(this)
				?? context.mixinDefinition()?.Accept(this)
				?? context.ruleset()?.Accept(this);
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
			var parentSelector = context.parentSelectorReference();
			if (parentSelector != null) {
				return new ParentReferenceSelectorElement(context.GetText());
			}

			return new SelectorElement(context.GetText());
		}

		public override LessNode VisitSelector(LessParser.SelectorContext context) {
			IEnumerable<SelectorElement> GetElements() {
				foreach (var element in context.selectorElement()) {
					yield return (SelectorElement) element.Accept(this);
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

		public override LessNode VisitExpression(LessParser.ExpressionContext context) {
			return new Expression(context.GetText());
		}

		public override LessNode VisitMixinDefinition(LessParser.MixinDefinitionContext context) {
			SelectorList selectors = (SelectorList) context.selectors().Accept(this);

			return new MixinDefinition(selectors, (RuleBlock) context.block().Accept(this));
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