using System.Collections.Generic;
using System.Text;
using LessonNet.Parser.ParseTree;

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

		public string GenerateCss(EvaluationContext context) {
			StringBuilder builder = new StringBuilder();

			foreach (var childNode in Evaluate(context)) {
				builder.Append(childNode.ToCss());
			}

			return builder.ToString();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var statement in Statements) {
				foreach (var generatedStatement in statement.Evaluate(context)) {
					if (generatedStatement is Ruleset rs) {
						yield return rs;
					}
				}
			}
		}
	}
}