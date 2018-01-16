using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class QuotedExpression : Expression {
		public LessString Value { get; }

		public QuotedExpression(LessString value) {
			this.Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Identifier(new ConstantIdentifierPart(Value.EvaluateSingle<LessString>(context).GetUnquotedValue()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Value.GetUnquotedValue());
		}

		protected bool Equals(QuotedExpression other) {
			return string.Equals(Value, other.Value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((QuotedExpression) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (Value != null ? Value.GetHashCode() : 0);
		}
	}
}