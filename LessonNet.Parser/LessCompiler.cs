using System;
using System.IO;
using System.IO.Abstractions;
using Antlr4.Runtime;
using LessonNet.Grammar;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree;

namespace LessonNet.Parser
{
	public class LessCompiler
	{
		public void Compile(string inputFileName)
		{
			var context = new EvaluationContext(new LessTreeParser(), new FileResolver(new FileSystem(), inputFileName));
			var rootNode = context.ParseCurrentStylesheet(isReference: false);

			var evaluated = rootNode.EvaluateSingle<Stylesheet>(context);

			var outputContext = new OutputContext(' ', 4);
			outputContext.Append(evaluated);

			Console.WriteLine(outputContext.GetCss());
		}
	}
}