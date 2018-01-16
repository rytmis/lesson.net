using System;

namespace LessonNet.Parser.Util {
	public static class StringExtensions {
		public static TEnum ParseEnum<TEnum>(this string input, bool ignoreCase = true) where TEnum : struct {
			return (TEnum) Enum.Parse(typeof(TEnum), input, ignoreCase);
		}

		public static bool IsLocalFilePath(this string uri) {
			if (uri.StartsWith("//")) {
				// Protocol-relative URI
				return false;
			}

			if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri parsedUri)) {
				return false;
			}

			if (parsedUri.IsAbsoluteUri && parsedUri.Scheme != "file") {
				return false;
			}

			return true;
		}
	}
}
