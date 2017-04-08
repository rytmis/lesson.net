using System.Collections.Generic;
using System.Text;

namespace LessonNet.Parser.ParseTree
{
	public class Stylesheet : StatementList
	{
		public Stylesheet(IEnumerable<Statement> statements) : base(statements) { }

		public string GenerateCss(EvaluationContext context) {
			StringBuilder builder = new StringBuilder();

			foreach (var childNode in Evaluate(context)) {
				builder.Append(childNode.ToCss());
			}

			return builder.ToString();
		}
	}
}