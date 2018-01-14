using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.Util
{
	public static class ValueTupleExtensions
	{
		public static T Max<T>(this (T, T, T) items)
		{
			return new[] { items.Item1, items.Item2, items.Item3 }.Max();
		}
		public static T Min<T>(this (T, T, T) items)
		{
			return new[] { items.Item1, items.Item2, items.Item3 }.Min();
		}
	}
}
