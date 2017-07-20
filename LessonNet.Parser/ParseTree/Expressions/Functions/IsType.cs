namespace LessonNet.Parser.ParseTree.Expressions.Functions {

	public class IsColorFunction : LessFunction {
		public IsColorFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var argumentMatch = arguments.Single<Color>();

			return new BooleanValue(argumentMatch != null);
		}
	}

	public class IsEmFunction : LessFunction {
		public IsEmFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var measurement = arguments.Single<Measurement>();

			return new BooleanValue(string.Equals(measurement?.Unit, "em"));
		}
	}

	public class IsNumberFunction : LessFunction {
		public IsNumberFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var measurement = arguments.Single<Measurement>();

			return new BooleanValue(measurement != null);
		}
	}

	public class IsPercentageFunction : LessFunction {
		public IsPercentageFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var measurement = arguments.Single<Measurement>();

			return new BooleanValue(string.Equals(measurement?.Unit, "%"));
		}
	}

	public class IsPixelFunction : LessFunction {
		public IsPixelFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var measurement = arguments.Single<Measurement>();

			return new BooleanValue(string.Equals(measurement?.Unit, "px"));
		}
	}

	public class IsStringFunction : LessFunction {
		public IsStringFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var str = arguments.Single<LessString>();

			return new BooleanValue(str != null);
		}
	}

	public class IsKeywordFunction : LessFunction {
		public IsKeywordFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var identifier = arguments.Single<Identifier>();

			return new BooleanValue(identifier != null);
		}
	}
}