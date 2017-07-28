using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using static System.FormattableString;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Color : Expression, IComparable<Color> {
		public static readonly Color Transparent = new Color(0, 0, 0, 0, "transparent");

		public byte R { get; }
		public byte G { get; }
		public byte B { get; }
		public decimal? Alpha { get; }
		public string Keyword { get; }


		public Color(byte r, byte g, byte b, decimal? alpha = null, string keyword = null) {
			this.R = r;
			this.G = g;
			this.B = b;
			this.Alpha = AlphaValue(alpha);
			this.Keyword = keyword;
		}

		public Color(Measurement m) : this((byte) m.Number, (byte) m.Number, (byte) m.Number) {
		}

		private static decimal? AlphaValue(decimal? alpha) {
			if (!alpha.HasValue) {
				return null;
			}

			return Math.Max(Math.Min(alpha.Value, 1), 0);
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public Color BlendWith(Color other, Func<byte, byte, byte> op) {
			var otherAlpha = other.Alpha ?? 1;
			var selfAlpha = Alpha ?? 1;

			var resultAlpha = otherAlpha + selfAlpha * (1 - otherAlpha);

			return new Color(
				ComposeWith(other, resultAlpha, c => c.R, op),
				ComposeWith(other, resultAlpha, c => c.G, op),
				ComposeWith(other, resultAlpha, c => c.B, op),
				resultAlpha);
		}

		private byte ComposeWith(Color other, decimal resultAlpha, Func<Color, byte> channel, Func<byte,byte,byte> op)
		{
			var backdropChannelValue = channel(this);
			var sourceChannelValue = channel(other);
			var backdropAlpha = Alpha ?? 1;
			var sourceAlpha = other.Alpha ?? 1;
			byte channelResult = op(backdropChannelValue, sourceChannelValue);
			if (resultAlpha <= 0) {
				return channelResult;
			}

			return (byte) ((sourceAlpha * sourceChannelValue + backdropAlpha * (backdropChannelValue - sourceAlpha * (backdropChannelValue + sourceChannelValue - channelResult))) / resultAlpha);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(ToString());
		}

		protected override string GetStringRepresentation() {
			if (!string.IsNullOrEmpty(Keyword)) {
				return Keyword;
			}

			if (Alpha.HasValue && Alpha < 1) {
				return Invariant($"rgba({R}, {G}, {B}, {Alpha})");
			}

			return $"#{R:x2}{G:x2}{B:x2}";
		}

		public string ToArgbString() {
			int alpha = (int) Math.Ceiling((Alpha ?? 1) * 255);

			return $"#{alpha:x2}{R:x2}{G:x2}{B:x2}";
		}

		public static Color FromHexString(string hex) {
			return FromHexStringCore(hex, hex);
		}

		private static Color FromHexStringCore(string hex, string keyword) {
			uint value = uint.Parse(EnsureSixDigitHex(hex.TrimStart('#')), NumberStyles.HexNumber);

			byte[] values = BitConverter.GetBytes(value);

			return new Color(values[2], values[1], values[0], null, keyword);
		}

		public static Color FromKeyword(string keyword) {
			if (string.Equals(Transparent.Keyword, keyword)) {
				return Color.Transparent;
			}
			return FromHexStringCore(KnownColors.GetHexString(keyword), keyword);
		}

		private static string EnsureSixDigitHex(string input) {
			if (input.Length == 6) {
				return input;
			}

			if (input.Length != 3) {
				throw new ArgumentException($"Invalid color hex string {input}", nameof(input));
			}

			return new string(DuplicateDigits().ToArray());

			IEnumerable<Char> DuplicateDigits() {
				for (var i = 0; i < input.Length; i++) {
					yield return input[i];
					yield return input[i];
				}
			}
		}

		protected bool Equals(Color other) {
			return R == other.R
				&& G == other.G
				&& B == other.B
				&& Alpha.Equals(other.Alpha)
				&& string.Equals(Keyword, other.Keyword);
		}

		public int CompareTo(Color other) {
			if (other == null) {
				return -1;
			}

			if (other.R == R && other.G == G && other.B == B && other.Alpha == Alpha) {
				return 0;
			}

			return other.ToComparable() < ToComparable() ? 1 : -1;
		}

		private decimal ToComparable() {
			return (256 * 3 - (R + G + B)) * (Alpha ?? 1);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Color) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (int) R;
				hashCode = (hashCode * 397) ^ (int) G;
				hashCode = (hashCode * 397) ^ (int) B;
				hashCode = (hashCode * 397) ^ Alpha.GetHashCode();
				hashCode = (hashCode * 397) ^ (Keyword != null ? Keyword.GetHashCode() : 0);
				return hashCode;
			}
		}

		public static Color operator +(Color c1, Color c2) {
			return new Color(
				(byte) (c1.R + c2.R),
				(byte) (c1.G + c2.G),
				(byte) (c1.B + c2.B)
			);
		}

		public static Color operator -(Color c1, Color c2) {
			return new Color(
				(byte) (c1.R - c2.R),
				(byte) (c1.G - c2.G),
				(byte) (c1.B - c2.B)
			);
		}

		public static Color operator *(Color c1, Color c2) {
			return new Color(
				(byte) (c1.R * c2.R),
				(byte) (c1.G * c2.G),
				(byte) (c1.B * c2.B)
			);
		}

		public static Color operator /(Color c1, Color c2) {
			return new Color(
				(byte) (c1.R / c2.R),
				(byte) (c1.G / c2.G),
				(byte) (c1.B / c2.B)
			);
		}

		public static bool operator <(Color c1, Color c2) {
			return c1.CompareTo(c2) == -1;
		}
		public static bool operator >(Color c1, Color c2) {
			return c1.CompareTo(c2) == 1;
		}

		public static bool operator <=(Color c1, Color c2) {
			return c1.CompareTo(c2) <= 0;
		}
		public static bool operator >=(Color c1, Color c2) {
			return c1.CompareTo(c2) >= 0;
		}
		public static bool operator ==(Color c1, Color c2) {
			return Equals(c1, c2);
		}

		public static bool operator !=(Color c1, Color c2) {
			return !(c1 == c2);
		}

	}

	public static class KnownColors {
		private static readonly Dictionary<string, string> colorsByKeyword = new Dictionary<string, string> {
			["aliceblue"] = "#F0F8FF",
			["antiquewhite"] = "#FAEBD7",
			["aqua"] = "#00FFFF",
			["aquamarine"] = "#7FFFD4",
			["azure"] = "#F0FFFF",
			["beige"] = "#F5F5DC",
			["bisque"] = "#FFE4C4",
			["black"] = "#000000",
			["blanchedalmond"] = "#FFEBCD",
			["blue"] = "#0000FF",
			["blueviolet"] = "#8A2BE2",
			["brown"] = "#A52A2A",
			["burlywood"] = "#DEB887",
			["cadetblue"] = "#5F9EA0",
			["chartreuse"] = "#7FFF00",
			["chocolate"] = "#D2691E",
			["coral"] = "#FF7F50",
			["cornflowerblue"] = "#6495ED",
			["cornsilk"] = "#FFF8DC",
			["crimson"] = "#DC143C",
			["cyan"] = "#00FFFF",
			["darkblue"] = "#00008B",
			["darkcyan"] = "#008B8B",
			["darkgoldenrod"] = "#B8860B",
			["darkgray"] = "#A9A9A9",
			["darkgrey"] = "#A9A9A9",
			["darkgreen"] = "#006400",
			["darkkhaki"] = "#BDB76B",
			["darkmagenta"] = "#8B008B",
			["darkolivegreen"] = "#556B2F",
			["darkorange"] = "#FF8C00",
			["darkorchid"] = "#9932CC",
			["darkred"] = "#8B0000",
			["darksalmon"] = "#E9967A",
			["darkseagreen"] = "#8FBC8F",
			["darkslateblue"] = "#483D8B",
			["darkslategray"] = "#2F4F4F",
			["darkslategrey"] = "#2F4F4F",
			["darkturquoise"] = "#00CED1",
			["darkviolet"] = "#9400D3",
			["deeppink"] = "#FF1493",
			["deepskyblue"] = "#00BFFF",
			["dimgray"] = "#696969",
			["dimgrey"] = "#696969",
			["dodgerblue"] = "#1E90FF",
			["firebrick"] = "#B22222",
			["floralwhite"] = "#FFFAF0",
			["forestgreen"] = "#228B22",
			["fuchsia"] = "#FF00FF",
			["gainsboro"] = "#DCDCDC",
			["ghostwhite"] = "#F8F8FF",
			["gold"] = "#FFD700",
			["goldenrod"] = "#DAA520",
			["gray"] = "#808080",
			["grey"] = "#808080",
			["green"] = "#008000",
			["greenyellow"] = "#ADFF2F",
			["honeydew"] = "#F0FFF0",
			["hotpink"] = "#FF69B4",
			["indianred "] = "#CD5C5C",
			["indigo "] = "#4B0082",
			["ivory"] = "#FFFFF0",
			["khaki"] = "#F0E68C",
			["lavender"] = "#E6E6FA",
			["lavenderblush"] = "#FFF0F5",
			["lawngreen"] = "#7CFC00",
			["lemonchiffon"] = "#FFFACD",
			["lightblue"] = "#ADD8E6",
			["lightcoral"] = "#F08080",
			["lightcyan"] = "#E0FFFF",
			["lightgoldenrodyellow"] = "#FAFAD2",
			["lightgray"] = "#D3D3D3",
			["lightgrey"] = "#D3D3D3",
			["lightgreen"] = "#90EE90",
			["lightpink"] = "#FFB6C1",
			["lightsalmon"] = "#FFA07A",
			["lightseagreen"] = "#20B2AA",
			["lightskyblue"] = "#87CEFA",
			["lightslategray"] = "#778899",
			["lightslategrey"] = "#778899",
			["lightsteelblue"] = "#B0C4DE",
			["lightyellow"] = "#FFFFE0",
			["lime"] = "#00FF00",
			["limegreen"] = "#32CD32",
			["linen"] = "#FAF0E6",
			["magenta"] = "#FF00FF",
			["maroon"] = "#800000",
			["mediumaquamarine"] = "#66CDAA",
			["mediumblue"] = "#0000CD",
			["mediumorchid"] = "#BA55D3",
			["mediumpurple"] = "#9370DB",
			["mediumseagreen"] = "#3CB371",
			["mediumslateblue"] = "#7B68EE",
			["mediumspringgreen"] = "#00FA9A",
			["mediumturquoise"] = "#48D1CC",
			["mediumvioletred"] = "#C71585",
			["midnightblue"] = "#191970",
			["mintcream"] = "#F5FFFA",
			["mistyrose"] = "#FFE4E1",
			["moccasin"] = "#FFE4B5",
			["navajowhite"] = "#FFDEAD",
			["navy"] = "#000080",
			["oldlace"] = "#FDF5E6",
			["olive"] = "#808000",
			["olivedrab"] = "#6B8E23",
			["orange"] = "#FFA500",
			["orangered"] = "#FF4500",
			["orchid"] = "#DA70D6",
			["palegoldenrod"] = "#EEE8AA",
			["palegreen"] = "#98FB98",
			["paleturquoise"] = "#AFEEEE",
			["palevioletred"] = "#DB7093",
			["papayawhip"] = "#FFEFD5",
			["peachpuff"] = "#FFDAB9",
			["peru"] = "#CD853F",
			["pink"] = "#FFC0CB",
			["plum"] = "#DDA0DD",
			["powderblue"] = "#B0E0E6",
			["purple"] = "#800080",
			["rebeccapurple"] = "#663399",
			["red"] = "#FF0000",
			["rosybrown"] = "#BC8F8F",
			["royalblue"] = "#4169E1",
			["saddlebrown"] = "#8B4513",
			["salmon"] = "#FA8072",
			["sandybrown"] = "#F4A460",
			["seagreen"] = "#2E8B57",
			["seashell"] = "#FFF5EE",
			["sienna"] = "#A0522D",
			["silver"] = "#C0C0C0",
			["skyblue"] = "#87CEEB",
			["slateblue"] = "#6A5ACD",
			["slategray"] = "#708090",
			["slategrey"] = "#708090",
			["snow"] = "#FFFAFA",
			["springgreen"] = "#00FF7F",
			["steelblue"] = "#4682B4",
			["tan"] = "#D2B48C",
			["teal"] = "#008080",
			["thistle"] = "#D8BFD8",
			["tomato"] = "#FF6347",
			["turquoise"] = "#40E0D0",
			["violet"] = "#EE82EE",
			["wheat"] = "#F5DEB3",
			["white"] = "#FFFFFF",
			["whitesmoke"] = "#F5F5F5",
			["yellow"] = "#FFFF00",
			["yellowgreen"] = "#9ACD32",
		};

		public static string GetHexString(string knownColor) {
			if (colorsByKeyword.TryGetValue(knownColor.ToLowerInvariant(), out string hexValue)) {
				return hexValue;
			}

			throw new ArgumentException($"Unknown color {knownColor}", nameof(knownColor));
		}
	}
}

/*
 * 
 */