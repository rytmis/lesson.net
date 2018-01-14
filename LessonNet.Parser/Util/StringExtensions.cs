using System;

namespace LessonNet.Parser.Util {
	public static class StringExtensions {
		public static TEnum ParseEnum<TEnum>(this string input, bool ignoreCase = true) where TEnum : struct {
			return (TEnum) Enum.Parse(typeof(TEnum), input, ignoreCase);
		}
	}
}
