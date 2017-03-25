using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class RuleBlock : LessNode {
		private readonly List<Rule> rules;
		private IList<Statement> statements;

		public RuleBlock(IEnumerable<Rule> rules, IEnumerable<Statement> statements) {
			this.rules = rules.ToList();
			this.statements = statements?.ToList() ?? (IList<Statement>)new Statement[0];
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var rule in rules) {
				foreach (var generatedRule in rule.Evaluate(context)) {
					yield return generatedRule;
				}
			}

			foreach (var statement in statements) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return generatedNode;
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