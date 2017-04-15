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
	}
}