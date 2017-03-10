using System;
using System.IO;
using LessonNet.Parser;

namespace LessonNet.Commandline
{
	class Program
	{
		static void Main(string[] args)
		{
			new LessCompiler().Compile(args[0]);
		}
	}
}