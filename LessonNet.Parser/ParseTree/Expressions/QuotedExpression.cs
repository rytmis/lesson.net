using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class QuotedExpression : Expression {
		private readonly LessString value;

		public QuotedExpression(LessString value) {
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Identifier(new ConstantIdentifierPart(value.EvaluateSingle<LessString>(context).GetUnquotedValue()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(value.GetUnquotedValue());
		}

		protected bool Equals(QuotedExpression other) {
			return string.Equals(value, other.value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((QuotedExpression) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (value != null ? value.GetHashCode() : 0);
		}
	}
}