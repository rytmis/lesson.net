using System;
using System.Diagnostics;
using System.IO;
using LessonNet.Parser;

namespace LessonNet.Commandline
{
	class Program
	{
		static void Main(string[] args)
		{
			var watch = Stopwatch.StartNew();
			try {
				new LessCompiler().Compile(args[0]);
			} catch (Exception ex) {
				Console.WriteLine($"/* Error: {ex} */");
			} finally {
				Console.WriteLine($"/* Generated in {watch.Elapsed} */");
			}
		}
	}
}