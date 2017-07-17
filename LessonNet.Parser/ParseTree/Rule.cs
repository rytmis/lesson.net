using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
		private readonly string property;
		private readonly ListOfExpressionLists values;

		public Rule(string property, ListOfExpressionLists values) {
			this.property = property;
			this.values = values;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var evaluatedValues = this.values.EvaluateSingle<ListOfExpressionLists>(context);

			yield return new Rule(property, evaluatedValues);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append($"{property}: {values.ToCss()}");
		}

		protected override string GetStringRepresentation() {
			return $"{property}: {string.Join(", ", values)}";
		}

		protected bool Equals(Rule other) {
			return string.Equals(property, other.property) && values.Equals(other.values);
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
				hashCode = (hashCode * 397) ^ (values != null ? values.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}