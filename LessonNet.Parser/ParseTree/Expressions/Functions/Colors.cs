using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions
{
	public class ColorFunction : LessFunction
	{
		public ColorFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var str = arguments.Single<LessString>().GetUnquotedValue();

			try {
				return Color.FromHexString(str);
			} catch {
				
			}

			try {
				return Color.FromKeyword(str);
			} catch {
				
			}

			throw new EvaluationException("Argument must be a color keyword or hex");
		}
	}

	public class ArgbFunction : LessFunction
	{
		public ArgbFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var color = arguments.Single<Color>();
			if (color != null) {
				return new Identifier(new ConstantIdentifierPart(color.ToArgbString()));
			}

			throw new EvaluationException("Argument must be a color");
		}
	}

	public class AlphaFunction : LessFunction {
		public AlphaFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var color = arguments.Single<Color>();
			if (color == null) {
				throw new EvaluationException("Argument must be a color");
			}

			if (color.Alpha.HasValue) {
				return new Measurement(color.Alpha.Value, "");
			}

			return new Measurement(1, "");
		}
	}

	public class RgbFunction : LessFunction {
		public RgbFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var (r, g, b) = VerifyArguments(arguments);
			
			return new Color((uint)r.Number, (uint)g.Number, (uint)b.Number, null);
		}

		private static (Measurement, Measurement, Measurement) VerifyArguments(ListOfExpressionLists arguments) {
			if (arguments.Count != 3) {
				throw new EvaluationException($"Unexpected argument count: {arguments.Count}");
			}

			for (var i = 0; i < arguments.Count; i++) {
				if (arguments[i].Values.Count != 1) {
					throw new EvaluationException($"Unexpected argument: {arguments[i]}");
				}

				var value = arguments[i].Values[0];
				if (!(value is Measurement)) {
					throw new EvaluationException($"Unexpected argument: {value}");
				}
			}

			var values = arguments.Select(arg => arg.Values[0] as Measurement).ToList();

			return (values[0], values[1], values[2]);
		}
	}

	public class RgbaFunction : LessFunction {
		public RgbaFunction(ListOfExpressionLists arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var (r, g, b, a) = VerifyArguments(arguments);
			
			return new Color((uint)r.Number, (uint)g.Number, (uint)b.Number, a.Number);
		}

		private static (Measurement, Measurement, Measurement, Measurement) VerifyArguments(ListOfExpressionLists arguments) {
			if (arguments.Count != 4) {
				throw new EvaluationException($"Unexpected argument count: {arguments.Count}");
			}

			for (var i = 0; i < arguments.Count; i++) {
				if (arguments[i].Values.Count != 1) {
					throw new EvaluationException($"Unexpected argument: {arguments[i]}");
				}

				var value = arguments[i].Values[0];
				if (!(value is Measurement)) {
					throw new EvaluationException($"Unexpected argument: {value}");
				}
			}

			var values = arguments.Select(arg => arg.Values[0] as Measurement).ToList();

			return (values[0], values[1], values[2], values[3]);
		}
	}
}
