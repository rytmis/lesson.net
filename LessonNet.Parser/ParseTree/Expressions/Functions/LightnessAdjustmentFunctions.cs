﻿namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public abstract class LightnessAdjustmentFunctionBase : LessFunction {
		protected LightnessAdjustmentFunctionBase(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments, EvaluationContext context) {
			if (arguments is ExpressionList list
				&& list.Values.Count == 2
				&& list.Values[0] is Color color
				&& list.Values[1] is Measurement amount) {
				return EditHsl(color, GetAdjustment(amount));
			}

			throw new EvaluationException($"Unexpected arguments: {arguments}");
		}

		protected Color EditHsl(Color color, decimal amount) {
			var hslColor = HslColor.FromRgbColor(color);
			hslColor.Lightness += amount / 100;
			return hslColor.ToRgbColor();
		}

		protected abstract decimal GetAdjustment(Measurement amount);
	}


	public class DarkenFunction : LightnessAdjustmentFunctionBase {
		public DarkenFunction(Expression arguments) : base(arguments) { }
		protected override decimal GetAdjustment(Measurement amount) => -amount.Number;
	}
}
