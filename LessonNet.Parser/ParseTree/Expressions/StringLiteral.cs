using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public abstract class LessStringFragment : Expression {
		
	}

	public class LessStringLiteral : LessStringFragment {
		private readonly string value;

		public LessStringLiteral(string value) {
			this.value = value;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(value);
		}

		protected override string GetStringRepresentation() => value;

		protected bool Equals(LessStringLiteral other) {
			return string.Equals(value, other.value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LessStringLiteral) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (value != null ? value.GetHashCode() : 0);
		}
	}

	public class InterpolatedVariable : LessStringFragment {
		private readonly Variable variable;

		public InterpolatedVariable(Variable variable) {
			this.variable = variable;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var expression = variable.EvaluateSingle<ListOfExpressionLists>(context).Single<Expression>();

			if (expression is LessString str) {
				yield return new LessStringLiteral(str.GetUnquotedValue());
			} else {
				yield return new LessStringLiteral(expression.ToString());
			}
		}

		protected bool Equals(InterpolatedVariable other) {
			return Equals(variable, other.variable);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InterpolatedVariable) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (variable != null ? variable.GetHashCode() : 0);
		}
	}

	public class LessString : Expression {
		private readonly char quoteChar;
		private readonly IList<LessStringFragment> parts;

		public LessString(char quoteChar, IEnumerable<LessStringFragment> parts) {
			this.quoteChar = quoteChar;
			this.parts = parts.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var evaluatedParts = parts.Select(p => p.EvaluateSingle<LessStringLiteral>(context));

			yield return new LessString(quoteChar, evaluatedParts);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(quoteChar);
			foreach (var lessStringFragment in parts) {
				context.Append(lessStringFragment);
			}
			context.Append(quoteChar);
		}

		protected override string GetStringRepresentation() {
			return $"{quoteChar}{GetUnquotedValue()}{quoteChar}";
		}

		public string GetUnquotedValue() {
			return string.Join("", parts);
		}

		protected bool Equals(LessString other) {
			return quoteChar == other.quoteChar && parts.SequenceEqual(other.parts);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LessString) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ quoteChar.GetHashCode();
				hashCode = (hashCode * 397) ^ (parts != null ? parts.Aggregate(hashCode, (h, p) => (h * 397) ^ p.GetHashCode()) : 0);
				return hashCode;
			}
		}
	}
}