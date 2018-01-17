using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.ParseTree.Expressions;
using Xunit;

namespace LessonNet.Tests.Parser {
	public class ExpressionTests {
		public Expression Parse(string input) {
			var parser = new LessTreeParser();

			return parser.ParseExpression(input);
		}

		[Fact]
		public void CanParseSpaceSeparatedExpressionList() {
			var result = Parse("1px 2px");

			Assert.IsType<ExpressionList>(result);

			var list = (ExpressionList) result;

			Assert.Equal(2, list.Values.Count);
			Assert.False(list.IsCommaSeparated);
			Assert.Equal(new Measurement(1, "px"), list.Values[0]);
			Assert.Equal(new Measurement(2, "px"), list.Values[1]);
		}

		[Fact]
		public void CanParseCommaSeparatedExpressionList() {
			var result = Parse("1px, 2px, 3px");

			Assert.IsType<ExpressionList>(result);

			var list = (ExpressionList) result;

			Assert.Equal(3, list.Values.Count);
			Assert.True(list.IsCommaSeparated);
			Assert.Equal(new Measurement(1, "px"), list.Values[0]);
			Assert.Equal(new Measurement(2, "px"), list.Values[1]);
			Assert.Equal(new Measurement(3, "px"), list.Values[2]);
		}

		[Fact]
		public void CanParseCommaSeparatedExpressionListWithParenthesizedExpression() {
			var result = Parse("(1px * 0.9), 2px, 3px");

			Assert.IsType<ExpressionList>(result);

			var list = (ExpressionList) result;

			Assert.Equal(3, list.Values.Count);
			Assert.True(list.IsCommaSeparated);

			Assert.Equal(new ParenthesizedExpression(new MathOperation(new Measurement(1, "px"), "*", new Measurement(0.9m, null))), list.Values[0]);
			Assert.Equal(new Measurement(2, "px"), list.Values[1]);
			Assert.Equal(new Measurement(3, "px"), list.Values[2]);
		}

		[Fact]
		public void CanParseCommaSeparatedListOfSpaceSeparatedLists() {
			void AssertIsSpaceSeparatedList(Expression expr, int length) {
				Assert.IsType<ExpressionList>(expr);

				var targetList = (ExpressionList) expr;

				Assert.False(targetList.IsCommaSeparated);
				Assert.Equal(length, targetList.Values.Count);
			}

			var result = Parse("1 2 3, 4 5 6 7, 8 9 10 11 12");

			Assert.IsType<ExpressionList>(result);

			var list = (ExpressionList) result;

			Assert.Equal(3, list.Values.Count);
			Assert.True(list.IsCommaSeparated);


			AssertIsSpaceSeparatedList(list.Values[0], 3);
			AssertIsSpaceSeparatedList(list.Values[1], 4);
			AssertIsSpaceSeparatedList(list.Values[2], 5);
		}

		[Fact]
		public void UrlWithString() {
			var parsed = (Url) Parse("url(\"this is a test\")");

			Assert.Equal("\"this is a test\"", parsed.Content.ToString());
		}

		[Fact]
		public void UrlWithRawUrl() {
			var parsed = (Url) Parse("url(this is a test)");

			Assert.Equal("this is a test", parsed.Content.ToString());
		}

		[Fact]
		public void UrlWithRawUrlAndStringCharacters() {
			var parsed = (Url) Parse("url(this is \"'a test)");

			Assert.Equal("this is \"'a test", parsed.Content.ToString());
		}

		[Fact]
		public void UrlWithVariable() {
			var parsed = (Url) Parse("url(@avar)");

			Assert.Equal("@avar", parsed.Content.ToString());
		}

		[Fact]
		public void NegativeNumber() {
			var parsed = Parse("-1");

			Assert.IsType<Measurement>(parsed);

			var measurement = (Measurement) parsed;

			Assert.Equal(-1, measurement.Number);
		}

		[Fact]
		public void NegativeNumberWithUnit() {
			var parsed = Parse("-1px");

			Assert.IsType<Measurement>(parsed);

			var measurement = (Measurement) parsed;

			Assert.Equal(-1, measurement.Number);
			Assert.Equal("px", measurement.Unit);
		}

		[Fact]
		public void NumberWithUnit() {
			var parsed = Parse("1px");

			Assert.IsType<Measurement>(parsed);

			var measurement = (Measurement) parsed;

			Assert.Equal(1, measurement.Number);
			Assert.Equal("px", measurement.Unit);
		}

		[Fact]
		public void AdditionWithArbitraryUnit() {
			var parsed = Parse("4n+1");

			Assert.IsType<MathOperation>(parsed);

			var op = (MathOperation) parsed;

			Assert.Equal(new Measurement(4, "n"), op.LeftOperand);
			Assert.Equal("+", op.Operator);
			Assert.Equal(new Measurement(1, null), op.RightOperand);
		}
	}
}
