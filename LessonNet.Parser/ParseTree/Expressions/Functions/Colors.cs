using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions
{
	public class ColorFunction : LessFunction
	{
		public ColorFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var str = (arguments as LessString)?.GetUnquotedValue();

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
		public ArgbFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var color = arguments as Color;
			if (color != null) {
				return new Identifier(new ConstantIdentifierPart(color.ToArgbString()));
			}

			throw new EvaluationException("Argument must be a color");
		}
	}

	public class AlphaFunction : LessFunction {
		public AlphaFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var color = arguments as Color;
			if (color == null) {
				throw new EvaluationException("Argument must be a color");
			}

			if (color.Alpha.HasValue) {
				return new Measurement(color.Alpha.Value, "");
			}

			return new Measurement(1, "");
		}
	}

	public class LightnessFunction : LessFunction {
		public LightnessFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var color = arguments as Color;
			if (color == null) {
				throw new EvaluationException("Argument must be a color");
			}

			var hsl = HslColor.FromRgbColor(color);

			return new Measurement(hsl.Lightness, "");
		}
	}

	public class RgbFunction : LessFunction {
		public RgbFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var (r, g, b) = VerifyArguments(arguments);
			
			return new Color((byte)r.Number, (byte)g.Number, (byte)b.Number, null);
		}

		private static (Measurement, Measurement, Measurement) VerifyArguments(Expression arguments) {
			var list = arguments as ExpressionList;
			if (list?.Values.Count != 3) {
				throw new EvaluationException($"Unexpected argument count: {list?.Values.Count ?? 1}");
			}


			var values = list.Values;
			for (var i = 0; i < values.Count; i++) {
				var value = values[i];
				if (!(value is Measurement)) {
					throw new EvaluationException($"Unexpected argument: {value}");
				}
			}

			return ((Measurement)values[0], (Measurement)values[1], (Measurement)values[2]);
		}
	}

	public class RgbaFunction : LessFunction {
		public RgbaFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var (r, g, b, a) = VerifyArguments(arguments);
			
			return new Color((byte)r.Number, (byte)g.Number, (byte)b.Number, a.Number);
		}

		private static (Measurement, Measurement, Measurement, Measurement) VerifyArguments(Expression arguments) {
			var valueList = arguments as ExpressionList;
			if (valueList?.Values.Count != 4) {
				throw new EvaluationException($"Unexpected argument count: {valueList?.Values.Count ?? 1}");
			}

			for (var i = 0; i < valueList.Values.Count; i++) {
				var value = valueList.Values[0];
				if (!(value is Measurement)) {
					throw new EvaluationException($"Unexpected argument: {value}");
				}
			}

			var values = valueList.Values;

			return ((Measurement)values[0], (Measurement)values[1], (Measurement)values[2], (Measurement)values[3]);
		}
	}

	public class HslFunction : LessFunction {
		public HslFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			var (h, s, l) = VerifyArguments(arguments);

			return HslColor.FromHslaFunction(h.Number, s.Number, l.Number, 1).ToRgbColor();
		}

		private static (Measurement, Measurement, Measurement) VerifyArguments(Expression arguments) {
			var valueList = arguments as ExpressionList;
			if (valueList?.Values.Count != 3) {
				throw new EvaluationException($"Unexpected argument count: {valueList?.Values.Count ?? 1}");
			}

			for (var i = 0; i < valueList.Values.Count; i++) {
				var value = valueList.Values[0];
				if (!(value is Measurement)) {
					throw new EvaluationException($"Unexpected argument: {value}");
				}
			}

			var values = valueList.Values;

			return ((Measurement)values[0], (Measurement)values[1], (Measurement)values[2]);
		}
	}
}
