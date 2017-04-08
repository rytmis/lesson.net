using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.Util
{
	public static class EnumerableExtensions
	{
		public static bool HasAny<T>(this IEnumerable source)
		{
			return source.OfType<T>().Any();
		}

		public static (IEnumerable<TFirst> first, IEnumerable<TSecond> second)
			Split<TFirst, TSecond>(this IEnumerable source) {
			IList<TFirst> first = new List<TFirst>();
			IList<TSecond> second = new List<TSecond>();

			foreach (var item in source) {
				if (item is TFirst f) {
					first.Add(f);
				} else if (item is TSecond s) {
					second.Add(s);
				}
			}

			return (first, second);
		}

		public static (IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third)
			Split<TFirst, TSecond, TThird>(this IEnumerable source) {
			IList<TFirst> first = new List<TFirst>();
			IList<TSecond> second = new List<TSecond>();
			IList<TThird> third = new List<TThird>();

			foreach (var item in source) {
				if (item is TFirst f) {
					first.Add(f);
				} else if (item is TSecond s) {
					second.Add(s);
				} else if (item is TThird t) {
					third.Add(t);
				}
			}

			return (first, second, third);
		}
	}
}
