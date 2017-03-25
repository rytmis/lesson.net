using System;
using System.IO;
using Antlr4.Runtime;
using LessonNet.Grammar;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser
{
	public class LessCompiler
	{
		public void Compile(string inputFileName)
		{
			var context = new EvaluationContext(new LessTreeParser(), new FileResolver(inputFileName));
			var rootNode = context.ParseCurrentStylesheet();

			string css = rootNode.GenerateCss(context);

			Console.WriteLine(css);
		}
	}
}