using System;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree.Expressions
{
	public class HslColor
	{
		// Note: To avoid converting back and forth between HlsColor and Color, HlsColor should perhaps inherit from color and only cenvert when needed.

		private decimal hue;

		public decimal Hue
		{
			get { return hue; }
			set { hue = value % 1; }
		}

		private decimal saturation;

		public decimal Saturation
		{
			get { return saturation; }
			set { saturation = value.Clamp(1); }
		}

		private decimal lightness;

		public decimal Lightness
		{
			get { return lightness; }
			set { lightness = value.Clamp(1); }
		}

		public decimal? Alpha { get; set; }

		public HslColor(decimal hue, decimal saturation, decimal lightness)
			: this(hue, saturation, lightness, 1)
		{
		}

		public HslColor(decimal hue, decimal saturation, decimal lightness, decimal? alpha)
		{
			Hue = hue;
			Saturation = saturation;
			Lightness = lightness;
			Alpha = alpha;
		}

		public static HslColor FromHslaFunction(decimal hue, decimal saturation, decimal lightness, decimal alpha) {
			var h = (hue / 360m) % 1;
			var s = saturation.Clamp(100m) / 100m;
			var l = lightness.Clamp(100m) / 100m;
			var a = alpha.Clamp(1);

			return new HslColor(h, s, l, a);
		}

		public static HslColor FromRgbColor(Color color)
		{
			// Note: this algorithm from http://www.easyrgb.com/index.php?X=MATH&H=18#text18

			var rgb = (R: color.R / 255m, G: color.G / 255m, B: color.B / 255m);

			var min = rgb.Min();
			var max = rgb.Max();
			var range = max - min;

			decimal lightness = (max + min) / 2m;

			decimal saturation = 0;
			decimal hue = 0;

			if (range != 0)
			{
				if (lightness < 0.5m)
					saturation = range / (max + min);
				else
					saturation = range / (2 - max - min);

				var deltas = (R: GetDelta(rgb.R), G: GetDelta(rgb.G), B: GetDelta(rgb.B));

				if (rgb.R == max)
					hue = deltas.B - deltas.G;
				else if (rgb.G == max)
					hue = (1m / 3) + deltas.R - deltas.B;
				else if (rgb.B == max)
					hue = (2m / 3) + deltas.G - deltas.R;

				if (hue < 0) hue += 1;
				if (hue > 1) hue -= 1;
			}

			return new HslColor(hue, saturation, lightness, color.Alpha);

			decimal GetDelta(decimal x) {
				return (((max - x) / 6) + (range / 2)) / range;
			}
		}


		public Color ToRgbColor()
		{
			// Note: this algorithm from http://www.easyrgb.com/index.php?X=MATH&H=19#text19

			if (Saturation == 0)
			{
				var grey = (byte)Math.Round(Lightness * 255);
				return new Color(grey, grey, grey, Alpha);
			}

			decimal q;
			if (Lightness < 0.5m)
				q = Lightness * (1 + Saturation);
			else
				q = (Lightness + Saturation) - (Saturation * Lightness);

			var p = 2 * Lightness - q;

			var red = (byte)(255 * Hue_2_RGB(p, q, Hue + (1m / 3)));
			var green = (byte) (255 * Hue_2_RGB(p, q, Hue));
			var blue = (byte) (255 * Hue_2_RGB(p, q, Hue - (1m / 3)));

			return new Color(red, green, blue, Alpha);
		}

		private static decimal Hue_2_RGB(decimal v1, decimal v2, decimal vH)
		{
			if (vH < 0) vH += 1;
			if (vH > 1) vH -= 1;
			if ((6 * vH) < 1) return (v1 + (v2 - v1) * 6 * vH);
			if ((2 * vH) < 1) return (v2);
			if ((3 * vH) < 2) return (v1 + (v2 - v1) * ((2m / 3) - vH) * 6);
			return (v1);
		}

		//public Number GetHueInDegrees()
		//{
		//	return new Number(Hue * 360, "deg");
		//}

		//public Number GetSaturation()
		//{
		//	return new Number(Saturation * 100, "%");
		//}

		//public Number GetLightness()
		//{
		//	return new Number(Lightness * 100, "%");
		//}
	}
}
