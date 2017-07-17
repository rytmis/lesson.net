using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ConstantIdentifierPart : IdentifierPart {
		public string Value { get; }

		public ConstantIdentifierPart(string value) {
			Value = value;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return Value;
		}

		protected bool Equals(ConstantIdentifierPart other) {
			return string.Equals(Value, other.Value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ConstantIdentifierPart) obj);
		}

		public override int GetHashCode() {
			return (Value != null ? Value.GetHashCode() : 0);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Value);
		}
	}
}