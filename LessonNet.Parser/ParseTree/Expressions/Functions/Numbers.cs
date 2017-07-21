using System;

namespace LessonNet.Parser.ParseTree.Expressions.Functions
{
	public abstract class NumberFunction : LessFunction {
		protected NumberFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var number = arguments as Measurement;
			if (number == null) {
				throw new EvaluationException("Argument must be a number");
			}

			return EvaluateFunction(number);
		}

		protected abstract Measurement EvaluateFunction(Measurement input);
	}

	public class PercentageFunction : NumberFunction
	{
		public PercentageFunction(Expression arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(input.Number * 100, "%");
		}
	}

	public class CeilFunction : NumberFunction
	{
		public CeilFunction(Expression arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(Math.Ceiling(input.Number), input.Unit);
		}
	}

	public class FloorFunction : NumberFunction
	{
		public FloorFunction(Expression arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(Math.Floor(input.Number), input.Unit);
		}
	}
}
