namespace LessonNet.Parser.ParseTree.Expressions.Functions {

	public class IsColorFunction : LessFunction {
		public IsColorFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var argumentMatch = arguments as Color;

			return new BooleanValue(argumentMatch != null);
		}
	}

	public class IsEmFunction : LessFunction {
		public IsEmFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var measurement = arguments as Measurement;

			return new BooleanValue(string.Equals(measurement?.Unit, "em"));
		}
	}

	public class IsNumberFunction : LessFunction {
		public IsNumberFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var measurement = arguments as Measurement;

			return new BooleanValue(measurement != null);
		}
	}

	public class IsPercentageFunction : LessFunction {
		public IsPercentageFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var measurement = arguments as Measurement;

			return new BooleanValue(string.Equals(measurement?.Unit, "%"));
		}
	}

	public class IsPixelFunction : LessFunction {
		public IsPixelFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var measurement = arguments as Measurement;

			return new BooleanValue(string.Equals(measurement?.Unit, "px"));
		}
	}

	public class IsStringFunction : LessFunction {
		public IsStringFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var str = arguments as LessString;

			return new BooleanValue(str != null);
		}
	}

	public class IsKeywordFunction : LessFunction {
		public IsKeywordFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var identifier = arguments as Identifier;

			return new BooleanValue(identifier != null);
		}
	}
}