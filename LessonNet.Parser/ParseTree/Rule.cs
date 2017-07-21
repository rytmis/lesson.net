using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
		public string Property { get; }
		public Expression Value { get; }

		public Rule(string property, Expression value) {
			this.Property = property;
			this.Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {

			yield return new Rule(Property, this.Value.EvaluateSingle<Expression>(context));
		}

		public override Statement ForceImportant() {
			if (Value is ImportantExpression) {
				return this;
			}

			return new Rule(Property, new ImportantExpression(Value));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append($"{Property}: {Value.ToCss()}");
		}

		protected override string GetStringRepresentation() {
			return $"{Property}: {Value}";
		}

		protected bool Equals(Rule other) {
			return string.Equals(Property, other.Property) && Value.Equals(other.Value);
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
				hashCode = (hashCode * 397) ^ (Value != null ? Value.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}