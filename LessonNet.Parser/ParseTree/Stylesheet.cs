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
		private readonly bool isReference;

		public IList<Statement> Statements { get; }

		public Stylesheet(IEnumerable<Statement> statements, bool isReference) {
			this.isReference = isReference;
			Statements = statements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.BeginReferenceScope(isReference)) {
				yield return new Stylesheet(EvaluateStatements(context), isReference);
			}
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

			foreach (var mixinDefinition in mixinDefinitions) {
				mixinDefinition.DeclareIn(context);
			}

			// Rulesets are both declarations and general statements (invokable but also output-producing). Declare them before evaluating,
			// because an invocation may appear before the declaration in the source.
			foreach (var declaration in otherStatements.OfType<Declaration>()) {
				declaration.DeclareIn(context);
			}

			foreach (var statement in otherStatements) {
				foreach (var result in statement.Evaluate(context)) {
					yield return (Statement) result;
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			using (context.BeginReferenceScope(isReference)) {
				foreach (var childNode in Statements) {
					if (!isReference || childNode is MediaBlock || childNode is Ruleset) {
						childNode.WriteOutput(context);
					} 
				}
			}
		}
	}
}