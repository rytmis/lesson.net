using System;

namespace LessonNet.Parser.Util
{
	public static class NumberExtensions
	{
		public static T Clamp<T>(this T value, T? max = null, T? min = null) where T : struct, IComparable<T>
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
