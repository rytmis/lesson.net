using System.Collections.Generic;
using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
	public class StringFunctionsFixture : SpecFixtureBase
    {
        [Fact]
        public void NoFormatting()
        {
            AssertExpression("abc", "e('abc')");
            AssertExpression("abc", "e(\"abc\")");
        }

        [Fact]
        public void EscapesQuotes()
        {
            AssertExpression("'abc'", @"e('\'abc\'')");
            AssertExpression("\"abc\"", @"e('\""abc\""')");
        }

        [Fact]
        public void EscapeCopesWithIE8Hack()
        {
            var input = @"e(""\9"")";
            var expected = @"\9";

            AssertExpression(expected, input);
        }

        [Fact]
        public void TestEFunctionInfo()
        {
            var info1 = "e(string) is not supported by less.js, so this will work but not compile with other less implementations." +
                @" You may want to consider using ~"""" which does the same thing and is supported.";

            AssertExpressionLogMessage(info1, "e('abc {0}')");
        }

        [Fact]
        public void TestPercentageFunctionInfo()
        {
            var info1 = "%(string, args...) is not supported by less.js, so this will work but not compile with other less implementations." +
                @" You may want to consider using ~"""" and string interpolation which does the same thing and is supported.";

            AssertExpressionLogMessage(info1, "%('abc %s', 'd')");
        }

        [Fact]
        public void SubsequentArgumentssIgnored()
        {
            AssertExpression("'abc'", "%('abc', 'd', 'e')");
        }

        [Fact]
        public void SimpleFormatting()
        {
            AssertExpression("'abc d'", "%('abc %s', 'd', 'e')");
            AssertExpression("'abc d e'", "%('abc %s %s', 'd', 'e')");
        }

        [Fact]
        public void FormattingWithVariables()
        {
            var variables = new Dictionary<string, string> {{"x", "'def'"}, {"y", "'ghi'"}, {"z", @"'\'jkl\''"}};

            AssertExpression("'abc def ghi'", "%('abc %s %s', @x, @y)", variables);
            AssertExpression("'abc def ghi \\'jkl\\''", "%('abc %s %s %s', @x, @y, @z)", variables);
        }

        [Fact]
        public void FormatGradients()
        {
            var input =
                @"
#gradients {
  @from: #444;
  @to: #999;
  @ffgradient: ""-moz-linear-gradient(top, %s, %s)"";
  @wkgradient: ""-webkit-gradient(linear,left top,left bottom,color-stop(0, %s),color-stop(1, %s))"";
  @iegradient: ""progid:DXImageTransform.Microsoft.gradient(startColorstr='%s', endColorstr='%s')"";
  @ie8gradient: ""\""progid:DXImageTransform.Microsoft.gradient(startColorstr='%s', endColorstr='%s')\"""";

  background-image: e(%(@ffgradient, @from, @to));  // FF3.6
  background-image: e(%(@wkgradient, @from, @to));  // Saf4+, Chrome
  filter:           e(%(@iegradient, @from, @to));  // IE6,IE7
  -ms-filter:       e(%(@ie8gradient, @from, @to)); // IE8
}";

            var expected =
                @"
#gradients {
  background-image: -moz-linear-gradient(top, #444, #999);
  background-image: -webkit-gradient(linear,left top,left bottom,color-stop(0, #444),color-stop(1, #999));
  filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#444', endColorstr='#999');
  -ms-filter: ""progid:DXImageTransform.Microsoft.gradient(startColorstr='#444', endColorstr='#999')"";
}";

            AssertLess(input, expected);
        }
    }
}