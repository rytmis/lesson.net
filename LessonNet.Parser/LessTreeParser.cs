using System;
using System.IO;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LessonNet.Grammar;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser {
	public class LessTreeParser {
		public Stylesheet Parse(string fileName, Stream input, bool isReference) {
			var charStream = new AntlrInputStream(input);
			var lexer = new LessLexer(charStream);

			var tokenStream = new CommonTokenStream(lexer);

			var parser = new LessParser(tokenStream) {
				ErrorHandler = new BailErrorStrategy()
			};

			try {
				var lessStylesheet = parser.stylesheet();

				return (Stylesheet) lessStylesheet.Accept(new SyntaxTreeToParseTreeVisitor(tokenStream, isReference));

			} catch (ParseCanceledException ex) when (ex.InnerException is InputMismatchException ime) {
				throw ParserException.FromToken(fileName, ime.OffendingToken);
			} catch (ParseCanceledException ex) when (ex.InnerException is NoViableAltException nvae) {
				throw ParserException.FromToken(fileName, nvae.OffendingToken);
			}
		}

		public Expression ParseExpression(string input) {
			return (Expression) Parse(input, parser => parser.expression());
		}

		public SelectorList ParseSelectorList(string input) {
			return (SelectorList) Parse(input, parser => parser.selectors());
		}

		private LessNode Parse(string input, Func<LessParser, RuleContext> parseFunc) {
			var lexer = new LessLexer(new AntlrInputStream(input));
			var tokenStream = new CommonTokenStream(lexer);
			var parser = new LessParser(tokenStream);

			return parseFunc(parser).Accept(new SyntaxTreeToParseTreeVisitor(tokenStream, isReference: false));
		}
	}
}