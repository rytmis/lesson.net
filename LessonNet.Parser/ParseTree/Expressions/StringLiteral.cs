using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
			var expression = variable.EvaluateSingle<Expression>(context);

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

		protected override string GetStringRepresentation() {
			return $"@{{{variable.Name}}}";
		}
	}

	public class LessString : Expression {
		public char QuoteChar { get; }
		private readonly IList<LessStringFragment> parts;

		public LessString(char quoteChar, LessStringFragment fragment)
			: this(quoteChar, new[] {fragment}) { }

		public LessString(char quoteChar, IEnumerable<LessStringFragment> parts) {
			this.QuoteChar = quoteChar;
			this.parts = parts.ToList();
		}

		public static LessString FromString(string str) {
			return new LessString('"', new[] {new LessStringLiteral(str)});
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var evaluatedParts = parts.Select(p => p.EvaluateSingle<LessStringLiteral>(context));

			yield return new LessString(QuoteChar, evaluatedParts);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(QuoteChar);
			foreach (var lessStringFragment in parts) {
				context.Append(lessStringFragment);
			}
			context.Append(QuoteChar);
		}

		protected override string GetStringRepresentation() {
			return $"{QuoteChar}{GetUnquotedValue()}{QuoteChar}";
		}

		public string GetUnquotedValue() {
			return Unescape(string.Join("", parts));
		}

		private string Unescape(string input) {
			return Regex.Replace(input, @"\\(""|')", "$1");
		}

		protected bool Equals(LessString other) {
			return QuoteChar == other.QuoteChar && parts.SequenceEqual(other.parts);
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
				hashCode = (hashCode * 397) ^ QuoteChar.GetHashCode();
				hashCode = (hashCode * 397) ^ (parts != null ? parts.Aggregate(hashCode, (h, p) => (h * 397) ^ p.GetHashCode()) : 0);
				return hashCode;
			}
		}
	}
}