using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class BooleanValue : Expression {
		public static readonly BooleanValue True = new BooleanValue(true);
		public static readonly BooleanValue False = new BooleanValue(false);

		public bool Value { get; }

		public BooleanValue(bool value) {
			this.Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Value.ToString().ToLowerInvariant());
		}

		public bool Equals(BooleanValue other) {
			return Value == other.Value;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((BooleanValue) obj);
		}

		public override int GetHashCode() {
			return Value.GetHashCode();
		}
	}
}