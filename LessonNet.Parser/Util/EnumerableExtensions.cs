using System;
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

		public static (TElement first, IList<TElement> rest)
			SplitFirst<TElement>(this IEnumerable<TElement> source) {
			var elements = source.ToList();

			return (elements.FirstOrDefault(), elements.Skip(1).ToList());
		}

		public static (IList<TFirst> first, IList<TSecond> second)
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

		public static (IList<TFirst> first, IList<TSecond> second, IList<TThird> third)
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

		public static (IList<TFirst> first, IList<TSecond> second, IList<TThird> third, IList<TFourth> fourth)
			Split<TFirst, TSecond, TThird, TFourth>(this IEnumerable source) {
			IList<TFirst> firsts = new List<TFirst>();
			IList<TSecond> seconds = new List<TSecond>();
			IList<TThird> thirds = new List<TThird>();
			IList<TFourth> fourths = new List<TFourth>();

			foreach (var item in source) {
				if (item is TFirst first) {
					firsts.Add(first);
				} else if (item is TSecond second) {
					seconds.Add(second);
				} else if (item is TThird third) {
					thirds.Add(third);
				} else if (item is TFourth fourth) {
					fourths.Add(fourth);
				}
			}

			return (firsts, seconds, thirds, fourths);
		}
	}
}
