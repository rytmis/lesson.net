using System.Collections;
using System.Linq;

namespace LessonNet.Parser.Util
{
	public static class EnumerableExtensions
	{
		public static bool HasAny<T>(this IEnumerable source)
		{
			return source.OfType<T>().Any();
		}
	}
}
