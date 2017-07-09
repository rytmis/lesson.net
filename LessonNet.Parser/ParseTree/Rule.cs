using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
		private readonly string property;
		private readonly bool important;
		private readonly ListOfExpressionLists values;

		public Rule(string property, ListOfExpressionLists values, bool important) {
			this.property = property;
			this.important = important;
			this.values = values;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Rule(property, values.EvaluateSingle<ListOfExpressionLists>(context), important) {
				IsEvaluated = true
			};
		}

		public override void WriteOutput(OutputContext context) {
			context.Append($"{property}: {values.ToCss()}");
			if (important) {
				context.Append(" !important");
			}
		}

		protected override string GetStringRepresentation() {
			return $"{property}: {string.Join(", ", values)}{(important ? " !important" : "")}";
		}

		protected bool Equals(Rule other) {
			return string.Equals(property, other.property) && important == other.important && values.Equals(other.values);
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
				hashCode = (hashCode * 397) ^ (values != null ? values.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}