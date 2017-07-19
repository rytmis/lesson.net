using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Mixins;
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
			yield return new Stylesheet(EvaluateStatements(context));
		}

		private IEnumerable<Statement> EvaluateStatements(EvaluationContext context) {
			// Handle variables and mixin definitions first: Variable scoping rules dictate that within a given
			// variable scope, the last declaration is the one that takes effect for
			// both the current scope and child scopes.
			(var variables, var mixinDefinitions, var otherStatements) = Statements.Split<VariableDeclaration, MixinDefinition, Statement>();
			// Variables first, because declaring rulesets evaluates selectors that may depend on those variables
			foreach (var variable in variables) {
				variable.DeclareIn(context);
			}

			foreach (var declaration in mixinDefinitions) {
				declaration.DeclareIn(context);
			}

			// Rulesets are both declarations and general statements (invokable but also output-producing). Declare them before evaluating,
			// because an invocation may appear before the declaration in the source.
			var rulesets = otherStatements.OfType<Ruleset>();
			foreach (var ruleset in rulesets) {
				ruleset.DeclareIn(context);
			}

			foreach (var statement in otherStatements) {
				foreach (var result in statement.Evaluate(context)) {
					yield return (Statement) result;
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