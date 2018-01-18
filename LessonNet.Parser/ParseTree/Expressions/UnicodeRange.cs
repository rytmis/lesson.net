using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class UnicodeRange : Expression {
		private readonly string rangeStart;
		private readonly string rangeEnd;

		public UnicodeRange(string rangeStart, string rangeEnd) {
			this.rangeStart = rangeStart;
			this.rangeEnd = rangeEnd;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(rangeStart);
			if (!string.IsNullOrEmpty(rangeEnd)) {
				context.Append('-');
				context.Append(rangeEnd);
			}
		}

		protected bool Equals(UnicodeRange other) {
			return string.Equals(rangeStart, other.rangeStart) && string.Equals(rangeEnd, other.rangeEnd);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((UnicodeRange) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = (rangeStart?.GetHashCode() ?? 0) * 397;

				return hashCode  ^ (rangeEnd?.GetHashCode() ?? 0);
			}
		}
	}
}