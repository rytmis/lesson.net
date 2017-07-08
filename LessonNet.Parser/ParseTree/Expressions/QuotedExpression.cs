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

		protected bool Equals(QuotedExpression other) {
			return string.Equals(literal, other.literal);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((QuotedExpression) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (literal != null ? literal.GetHashCode() : 0);
		}
	}
}