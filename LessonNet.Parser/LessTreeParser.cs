using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LessonNet.Grammar;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser {
	public class LessTreeParser {
		private static readonly SyntaxTreeToParseTreeVisitor parserVisitor = new SyntaxTreeToParseTreeVisitor();	

		public Stylesheet Parse(string fileName, Stream input) {
			var charStream = new AntlrInputStream(input);
			var lexer = new LessLexer(charStream);

			var tokenStream = new CommonTokenStream(lexer);

			var parser = new LessParser(tokenStream) {
				ErrorHandler = new BailErrorStrategy()
			};


			try {
				var lessStylesheet = parser.stylesheet();

				return (Stylesheet) lessStylesheet.Accept(parserVisitor);

			} catch (ParseCanceledException ex) when (ex.InnerException is InputMismatchException ime) {
				throw ParserException.FromToken(fileName, ime.OffendingToken);
			} catch (ParseCanceledException ex) when (ex.InnerException is NoViableAltException nvae) {
				throw ParserException.FromToken(fileName, nvae.OffendingToken);
			}
		}
	}
}