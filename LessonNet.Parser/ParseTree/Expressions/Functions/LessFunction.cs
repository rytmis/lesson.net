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

		protected TArg UnpackArguments<TArg>()
			where TArg : Expression
		{
			if (Arguments is TArg arg) {
				return arg;
			}

			throw new EvaluationException($"Unexpected arguments: {Arguments}");
		}

		protected (TArg1, TArg2) UnpackArguments<TArg1, TArg2>()
			where TArg1 : Expression
			where TArg2 : Expression
		{
			if (Arguments is ExpressionList list
				&& list.Values.Count == 2
				&& list.Values[0] is TArg1 arg1
				&& list.Values[1] is TArg2 arg2) {
				return (arg1, arg2);
			}

			throw new EvaluationException($"Unexpected arguments: {Arguments}");
		}

		protected (TArg1, TArg2, TArg3) UnpackArguments<TArg1, TArg2, TArg3>()
			where TArg1 : Expression
			where TArg2 : Expression
			where TArg3 : Expression
		{
			if (Arguments is ExpressionList list
				&& list.Values.Count == 2
				&& list.Values[0] is TArg1 arg1
				&& list.Values[1] is TArg2 arg2
				&& list.Values[2] is TArg3 arg3) {
				return (arg1, arg2, arg3);
			}

			throw new EvaluationException($"Unexpected arguments: {Arguments}");
		}
	}
}