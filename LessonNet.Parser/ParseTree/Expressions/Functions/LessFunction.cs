using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public abstract class LessFunction : Expression {
		protected Expression Arguments { get; }

		protected LessFunction(Expression arguments) {
			this.Arguments = arguments;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return EvaluateFunction(Arguments.EvaluateSingle<Expression>(context));
		}

		protected abstract Expression EvaluateFunction(Expression arguments);
		protected bool Equals(LessFunction other) {
			return Equals(Arguments, other.Arguments);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((LessFunction) obj);
		}

		public override int GetHashCode() {
			return Arguments.GetHashCode();
		}
	}
}