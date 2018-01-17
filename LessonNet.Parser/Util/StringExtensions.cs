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

		public static (string Path, string Query) SplitPathAndQuery(this string input) {
			if (string.IsNullOrEmpty(input)) {
				return ("", "");
			}

			var bits = input.Split(new []{ '?'}, 2);

			return (bits[0], bits.Length == 2 ? bits[1] : "");
		}

		public static string AppendQuery(this string url, string query) {
			if (string.IsNullOrEmpty(query)) {
				return url;
			}

			return $"{url}?{query}";
		}
	}
}
