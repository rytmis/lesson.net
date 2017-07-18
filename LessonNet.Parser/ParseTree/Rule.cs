using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
		public string Property { get; }
		public ListOfExpressionLists Values { get; }

		public Rule(string property, ListOfExpressionLists values) {
			this.Property = property;
			this.Values = values;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var evaluatedValues = this.Values.EvaluateSingle<ListOfExpressionLists>(context);

			yield return new Rule(Property, evaluatedValues);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append($"{Property}: {Values.ToCss()}");
		}

		protected override string GetStringRepresentation() {
			return $"{Property}: {string.Join(", ", Values)}";
		}

		protected bool Equals(Rule other) {
			return string.Equals(Property, other.Property) && Values.Equals(other.Values);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Rule) obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (Property != null ? Property.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Values != null ? Values.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}