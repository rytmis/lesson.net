using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {

	public class RuleBlock : LessNode {
		public IEnumerable<Statement> Statements { get; }
		public int RuleCount => Statements.OfType<Rule>().Count();

		public RuleBlock(IEnumerable<Statement> statements) {
			Statements = statements?.ToList() ?? new List<Statement>();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			(var declarations, var mediaBlocks) = Statements.Split<Declaration, MediaBlock>();
			foreach (var declaration in declarations) {
				declaration.DeclareIn(context);
			}

			// Rulesets are both declarations and general statements (invokable but also output-producing).
			var rulesets = declarations.OfType<Ruleset>();

			foreach (var mediaBlock in mediaBlocks) {
				yield return mediaBlock;
			}

			foreach (var statement in Statements.Except(mediaBlocks)) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return generatedNode;
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.IncreaseIndentLevel();

			foreach (var statement in Statements) {
				context.Append(statement);
			}

			context.DecreaseIndentLevel();
		}

		public static RuleBlock Combine(IEnumerable<RuleBlock> blocks) {
			var blockList = blocks.ToArray();

			return new RuleBlock(blockList.SelectMany(b => b.Statements));
		}
	}
}