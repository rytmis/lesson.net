using System;

namespace LessonNet.Parser.ParseTree.Expressions.Functions
{
	public class PercentageFunction : LessFunction
	{
		public PercentageFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var number = arguments.Single<Measurement>();
			if (number != null) {
				return new Measurement(number.Number * 100, "%");
			}

			throw new EvaluationException("Argument must be a number");
		}
	}
}
