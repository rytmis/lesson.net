namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public abstract class AlphaFunctionBase : LessFunction {
		protected AlphaFunctionBase(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments, EvaluationContext context) {
			if (arguments is ExpressionList list
				&& list.Values.Count == 2
				&& list.Values[0] is Color color
				&& list.Values[1] is Measurement measurement) {

				var alpha = measurement.Number / 100m;

				return new Color(color.R, color.G, color.B, AdjustAlpha(color.Alpha ?? 1, alpha));
			}

			throw new EvaluationException($"Unexpected arguments: {Arguments}");
		}

		protected abstract decimal AdjustAlpha(decimal alpha, decimal adjustment);
	}


	public class FadeInFunction : AlphaFunctionBase {
		public FadeInFunction(Expression arguments) : base(arguments) { }

		protected override decimal AdjustAlpha(decimal alpha, decimal adjustment) {
			return alpha + adjustment;
		}
	}

	public class FadeOutFunction : AlphaFunctionBase {
		public FadeOutFunction(Expression arguments) : base(arguments) { }

		protected override decimal AdjustAlpha(decimal alpha, decimal adjustment) {
			return alpha - adjustment;
		}
	}

	public class FadeFunction : AlphaFunctionBase {
		public FadeFunction(Expression arguments) : base(arguments) { }

		protected override decimal AdjustAlpha(decimal alpha, decimal adjustment) {
			return adjustment;
		}
	}
}
