using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public class Measurement : Expression
	{
		public decimal Number { get; }
		public string Unit { get; }

		public Measurement(decimal number, string unit) {
			this.Number = number;
			this.Unit = unit;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return $"{Number.ToString(CultureInfo.InvariantCulture)}{Unit}";
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(GetStringRepresentation());
		}

		protected bool Equals(Measurement other) {
			return Number == other.Number && string.Equals(Unit, other.Unit);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Measurement) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ Number.GetHashCode();
				hashCode = (hashCode * 397) ^ (Unit != null ? Unit.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}
