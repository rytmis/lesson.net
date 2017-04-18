using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class QuotedExpression : Expression {
		private readonly string literal;

		public QuotedExpression(string literal) {
			this.literal = literal;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(literal.Trim('"'));
		}
	}
}