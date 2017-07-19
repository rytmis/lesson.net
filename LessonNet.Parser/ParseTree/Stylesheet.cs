using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree
{
	public class Stylesheet : Statement
	{
		public IList<Statement> Statements { get; }

		public Stylesheet(IEnumerable<Statement> statements) {
			Statements = statements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			(var rulesets, var mediaBlocks) = EvaluateStatements(context).Split<Ruleset, MediaBlock>();

			yield return new Stylesheet(rulesets.Concat<Statement>(mediaBlocks));
		}

		private IEnumerable<Statement> EvaluateStatements(EvaluationContext context) {
			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			(var variables, var declarations, var mediaBlocks, var otherStatements) = Statements.Split<VariableDeclaration, Declaration, MediaBlock, Statement>();
			// Variables first, because declaring rulesets evaluates selectors that may depend on those variables
			foreach (var variable in variables) {
				variable.DeclareIn(context);
			}

			foreach (var declaration in declarations) {
				declaration.DeclareIn(context);
			}

			// Rulesets are both declarations and general statements (invokable but also output-producing).
			var rulesets = declarations.OfType<Ruleset>();

			foreach (var mediaBlock in mediaBlocks) {
				foreach (var resultMediaBlock in mediaBlock.Evaluate(context).Cast<MediaBlock>()) {
					yield return resultMediaBlock;
				}
			}

			foreach (var statement in rulesets.Concat(otherStatements)) {
				foreach (var generatedNode in statement.Evaluate(context)) {
					yield return (Statement) generatedNode;
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			foreach (var childNode in Statements) {
				context.Append(childNode);
			}
		}
	}
}