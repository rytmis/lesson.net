using System;
using System.Collections.Generic;

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
}