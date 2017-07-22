using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Mixins;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {

	public class RuleBlock : LessNode {
		public IList<Statement> Statements { get; }
		public int RuleCount => Statements.OfType<Rule>().Count();

		public RuleBlock(IEnumerable<Statement> statements) {
			Statements = statements?.ToList() ?? new List<Statement>();
		}

		public RuleBlock ForceImportant() {
			return new RuleBlock(Statements.Select(s => s.ForceImportant()));
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			(var variables, var mediaBlocks, var mixinDefinitions, var otherStatements) = Statements
				.Split<VariableDeclaration, MediaBlock, MixinDefinition, Statement>();
			// Variables first, because declaring rulesets evaluates selectors that may depend on those variables
			foreach (var variable in variables) {
				variable.DeclareIn(context);
			}

			foreach (var mixinDefinition in mixinDefinitions) {
				mixinDefinition.DeclareIn(context);
			}

			foreach (var declaration in otherStatements.OfType<Declaration>()) {
				declaration.DeclareIn(context);
			}

			foreach (var mediaBlock in mediaBlocks) {
				foreach (var generatedBlock in mediaBlock.Evaluate(context)) {
					yield return generatedBlock;
				}
			}

			foreach (var statement in otherStatements) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return generatedNode;
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.IncreaseIndentLevel();

			foreach (var statement in Statements) {
				if (statement is Rule r) {
					// Rules may exist within media queries, but are only indented and semicolon-terminated within rule blocks
					context.Indent();
					context.Append(r);
					context.AppendLine(";");
				} else {
					context.Append(statement);
				}
			}

			context.DecreaseIndentLevel();
		}

		public static RuleBlock Combine(IEnumerable<RuleBlock> blocks) {
			var blockList = blocks.ToArray();

			return new RuleBlock(blockList.SelectMany(b => b.Statements));
		}
	}
}