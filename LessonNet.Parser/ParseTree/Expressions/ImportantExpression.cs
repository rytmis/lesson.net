using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ImportantExpression : Expression {
		public Expression Value { get; }

		public ImportantExpression(Expression value) {
			this.Value = value;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new ImportantExpression(Value.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Value);
			context.Append(" !important");
		}

		protected bool Equals(ImportantExpression other) {
			return Equals(Value, other.Value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
			return Equals((ImportantExpression) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return 397 ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}
	}
}