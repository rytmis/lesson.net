using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LessonNet.Parser.ParseTree {
	public abstract class StatementList : LessNode {
		private IList<Statement> statements;

		protected StatementList(IEnumerable<Statement> statements) {
			this.statements = statements?.ToList() ?? (IList<Statement>)new Statement[0];
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {

			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			var declarations = statements.OfType<Declaration>().ToArray();
			foreach (var declaration in declarations) {
				declaration.DeclareIn(context.CurrentScope);
			}

			foreach (var statement in statements.Except(declarations)) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return generatedNode;
				}
			}
		}
	}

	public class RuleBlock : StatementList {
		private readonly List<Rule> rules;

		public RuleBlock(IEnumerable<Rule> rules, IEnumerable<Statement> statements) : base(statements) {
			this.rules = rules.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var statement in base.EvaluateCore(context)) {
				yield return statement;
			}

			foreach (var rule in rules) {
				foreach (var generatedRule in rule.Evaluate(context)) {
					yield return generatedRule;
				}
			}
		}

		protected override string GetCss() {
			var builder = new StringBuilder();

			foreach (var rule in rules) {
				builder.AppendLine(rule.ToCss());
			}

			return builder.ToString();
		}
	}
}