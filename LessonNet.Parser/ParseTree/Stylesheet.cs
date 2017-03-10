using System.Collections.Generic;

namespace LessonNet.Parser.SyntaxTree
{
	public class Stylesheet : LessNode
	{
		public IReadOnlyCollection<Statement> Statements => statements.AsReadOnly();

		private List<Statement> statements = new List<Statement>();
		public Stylesheet Add(Statement statement)
		{
			statements.Add(statement);

			return this;
		}

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			foreach (var statement in Statements) {
				foreach (var generatedStatement in statement.Evaluate(context)) {
					yield return generatedStatement;
				}
			}
		}
	}
}