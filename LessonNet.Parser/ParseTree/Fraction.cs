using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class Fraction : Expression {
		public decimal Numerator { get; }
		public decimal Denominator { get; }
		public string Unit { get; }

		public Fraction(decimal numerator, decimal denominator, string unit) {
			Numerator = numerator;
			Denominator = denominator;
			Unit = unit;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(((int)Numerator).ToString());
			context.Append("/");
			context.Append(((int)Denominator).ToString());

			if (!string.IsNullOrEmpty(Unit)) {
				context.Append((string) Unit);
			}
		}

		protected bool Equals(Fraction other) {
			return Numerator == other.Numerator
				&& Denominator == other.Denominator
				&& string.Equals(Unit, other.Unit);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Fraction) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ Numerator.GetHashCode();
				hashCode = (hashCode * 397) ^ Denominator.GetHashCode();
				hashCode = (hashCode * 397) ^ (Unit != null ? Unit.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}