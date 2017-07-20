using System;

namespace LessonNet.Parser.ParseTree.Expressions.Functions
{
	public abstract class NumberFunction : LessFunction {
		protected NumberFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var number = arguments.Single<Measurement>();
			if (number == null) {
				throw new EvaluationException("Argument must be a number");
			}

			return EvaluateFunction(number);
		}

		protected abstract Measurement EvaluateFunction(Measurement input);
	}

	public class PercentageFunction : NumberFunction
	{
		public PercentageFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(input.Number * 100, "%");
		}
	}

	public class CeilFunction : NumberFunction
	{
		public CeilFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(Math.Ceiling(input.Number), input.Unit);
		}
	}

	public class FloorFunction : NumberFunction
	{
		public FloorFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override Measurement EvaluateFunction(Measurement input) {
			return new Measurement(Math.Floor(input.Number), input.Unit);
		}
	}
}
