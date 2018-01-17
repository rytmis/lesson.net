using System;
using System.Collections.Generic;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public abstract class ColorBlendingFunction : LessFunction {
		protected ColorBlendingFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {
			if (arguments is ExpressionList list
				&& list.Values.Count == 2
				&& list.Values[0] is Color backdrop
				&& list.Values[1] is Color source) {
				return Operate(backdrop, source);
			}

			throw new EvaluationException($"Unexpected arguments: {arguments}");
		}

		protected abstract Color Operate(Color backdrop, Color source);
	}

	public class MultiplyFunction : ColorBlendingFunction {
		public MultiplyFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) => (byte) (c1 * c2 / 255));
		}
	}

	public class ScreenFunction : ColorBlendingFunction {
		public ScreenFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) => (byte) (255 - (255 - c1) * (255 - c2) / 255));
		}
	}

	public class OverlayFunction : ColorBlendingFunction {
		public OverlayFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source,
				(c1, c2) => (byte) (c1 < 128 ? 2 * c1 * c2 / 255 : 255 - 2 * (255 - c1) * (255 - c2) / 255));
		}
	}


	public class SoftlightFunction : ColorBlendingFunction {
		public SoftlightFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) => {
				decimal t = c1 * (decimal) c2 / 255;
				return (byte) Math.Ceiling(t + c1 * (255 - (255 - c1) * (255 - c2) / 255 - t) / 255);
			});
		}
	}

	public class HardlightFunction : ColorBlendingFunction {
		public HardlightFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) =>
				(byte) (c2 < 128 ? 2 * c2 * c1 / 255 : 255 - 2 * (255 - c2) * (255 - c1) / 255)
			);
		}
	}

	public class DifferenceFunction : ColorBlendingFunction {
		public DifferenceFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) =>
				(byte) Math.Abs(c1 - c2)
			);
		}
	}

	public class ExclusionFunction : ColorBlendingFunction {
		public ExclusionFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) =>
				(byte) (c1 + c2 * (255 - c1 - c1) / 255)
			);
		}
	}

	public class AverageFunction : ColorBlendingFunction {
		public AverageFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) =>
				(byte) ((c1 + c2) / 2)
			);
		}
	}

	public class NegationFunction : ColorBlendingFunction {
		public NegationFunction(Expression arguments) : base(arguments) { }

		protected override Color Operate(Color backdrop, Color source) {
			return backdrop.BlendWith(source, (c1, c2) =>
				(byte) (255 - Math.Abs(255 - c2 - c1))
			);
		}
	}

	public class MixFunction : LessFunction {
		const decimal DefaultWeight = 50;

		public MixFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {
			if (arguments is ExpressionList list) {
				if (list.Values.Count == 2) {
					var (c1, c2) = UnpackArguments<Color, Color>();
					return Mix(c1, c2, DefaultWeight);
				}

				if (list.Values.Count == 3) {
					var (c1, c2, weight) = UnpackArguments<Color, Color, Measurement>();
					return Mix(c1, c2, weight.Number);
				}

			}

			throw new EvaluationException($"Unexpected arguments: {arguments}");
		}

		protected Color Mix(Color color1, Color color2, decimal weight) {
			// Note: this algorithm taken from http://github.com/nex3/haml/blob/0e249c844f66bd0872ed68d99de22b774794e967/lib/sass/script/functions.rb

			var a1 = color1.Alpha ?? 1;
			var a2 = color2.Alpha ?? 1;

			var p = weight / 100.0m;
			var w = p * 2 - 1;
			var a = a1 - a2;

			var w1 = (((w * a == -1) ? w : (w + a) / (1 + w * a)) + 1) / 2.0m;
			var w2 = 1m - w1;

			var alpha = a1 * p + a2 * (1 - p);

			var color = new Color(
				Mix(color1.R, color2.R, w1, w2), 
				Mix(color1.G, color2.G, w1, w2), 
				Mix(color1.B, color2.B, w1, w2), 
				alpha);
			return color;

			byte Mix(decimal channel1, decimal channel2, decimal weight1, decimal weight2) {
				return (byte)(channel1 * weight1 + channel2 * weight2).Clamp(0, 255);
			}
		}
	}

	public class TintFunction : MixFunction {
		public TintFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {
			var (color, weight) = UnpackArguments<Color, Measurement>();

			return Mix(new Color(255, 255, 255), color, weight.Number);
		}
	}

	public class ShadeFunction : MixFunction {
		public ShadeFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {

			var (color, weight) = UnpackArguments<Color, Measurement>();

			return Mix(new Color(0, 0, 0), color, weight.Number);
		}
	}
}