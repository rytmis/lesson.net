using System;
using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class BooleanValue : Expression {
		public bool Value { get; }

		public BooleanValue(bool value) {
			this.Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
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