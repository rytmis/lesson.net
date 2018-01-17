using System;

namespace LessonNet.Parser.Util
{
	public static class NumberExtensions
	{
		public static T Clamp<T>(this T value, T? max) where T : struct, IComparable<T>
		{
			return value.Clamp(null, max);
		}

		public static T Clamp<T>(this T value, T? min, T? max) where T : struct, IComparable<T>
		{
			if (max.HasValue && value.CompareTo(max.Value) > 0) {
				return max.Value;
			}

			if (min.HasValue && value.CompareTo(min.Value) < 0) {
				return min.Value;
			}

			return value;
		}
	}
}
