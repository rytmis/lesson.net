using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {

	public class RuleBlock : LessNode {
		public IEnumerable<Statement> Statements { get; }
		private readonly List<Rule> rules;
		public int RuleCount => rules.Count;

		public RuleBlock(IEnumerable<Rule> rules, IEnumerable<Statement> statements) {
			Statements = statements?.ToList() ?? new List<Statement>();
			this.rules = rules.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			(var declarations, var mediaBlocks, var otherStatements) = Statements.Split<Declaration, MediaBlock, Statement>();
			foreach (var declaration in declarations) {
				declaration.DeclareIn(context);
			}

			// Rulesets are both declarations and general statements (invokable but also output-producing).
			var rulesets = declarations.OfType<Ruleset>();

			foreach (var mediaBlock in mediaBlocks) {
				yield return mediaBlock;
			}

			foreach (var statement in rulesets.Concat(otherStatements)) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return generatedNode;
				}
			}

			foreach (var rule in rules) {
				foreach (var generatedRule in rule.Evaluate(context)) {
					yield return generatedRule;
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.IncreaseIndentLevel();

			foreach (var rule in rules) {
				context.Append(rule);
			}

			foreach (var statement in Statements) {
				context.Append(statement);
			}

			context.DecreaseIndentLevel();
		}
	}
}