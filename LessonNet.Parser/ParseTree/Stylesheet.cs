using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree
{
	public class Stylesheet : StatementList
	{
		public Stylesheet(IEnumerable<Statement> statements) : base(statements) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Stylesheet(base.EvaluateCore(context).Cast<Statement>()) {
				IsEvaluated = true
			};
		}

		public override void WriteOutput(OutputContext context) {
			foreach (var childNode in Statements) {
				context.Append(childNode);
			}
		}

	}
}