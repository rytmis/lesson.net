using System;
using Antlr4.Runtime;

namespace LessonNet.Parser
{
	public class ParserException : Exception
	{
		public ParserException(string fileName, string message, int line, int column)
			: base($"{message} at {fileName} line {line}, column {column}") { }
		public ParserException(string message) : base(message) { }
		public ParserException(string message, Exception innerException) : base(message, innerException) { }

		public static ParserException FromToken(string fileName, IToken token) {
			return new ParserException(fileName, $"Unrecognized input: '{token.Text}'", token.Line, token.Column);
		}
	}
}
