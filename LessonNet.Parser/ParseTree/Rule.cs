using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
		private readonly string property;
		private readonly bool important;
		private readonly List<ExpressionList> values;

		public Rule(string property, IEnumerable<ExpressionList> values, bool important) {
			this.property = property;
			this.important = important;
			this.values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<ExpressionList> EvaluateValues() {
				foreach (var value in values) {
					yield return value.EvaluateSingle<ExpressionList>(context);
				}
			}

			yield return new Rule(property, EvaluateValues(), important) {IsEvaluated = true};
		}

		public override void WriteOutput(OutputContext context) {
			string cssValues = string.Join(", ", this.values.Select(v => v.ToCss()));

			context.Append($"{property}: {cssValues}");
			if (important) {
				context.Append(" !important");
			}
		}

		protected override string GetStringRepresentation() {
			return $"{property}: {string.Join(", ", values)}{(important ? " !important" : "")}";
		}

		protected bool Equals(Rule other) {
			return string.Equals(property, other.property) && important == other.important && values.SequenceEqual(other.values);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Rule) obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (property != null ? property.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ important.GetHashCode();
				hashCode = (hashCode * 397) ^ (values != null ? values.Aggregate(hashCode, (h, e) => (h * 397) ^ e.GetHashCode()) : 0);
				return hashCode;
			}
		}
	}
}