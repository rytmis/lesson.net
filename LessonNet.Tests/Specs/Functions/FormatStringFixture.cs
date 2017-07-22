using System.Collections.Generic;
using LessonNet.Parser;
using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
	public class FormatStringFixture : SpecFixtureBase
    {
        [Fact]
        public void NoFormatting()
        {
            AssertExpression("abc", "formatstring('abc')");
            AssertExpression("abc", "formatstring(\"abc\")");
        }

        [Fact]
        public void EscapesQuotes()
        {
            AssertExpression("'abc'", @"formatstring('\'abc\'')");
            AssertExpression("\"abc\"", @"formatstring('\""abc\""')");
        }

        [Fact]
        public void SubsequentArgumentsIgnored()
        {
            AssertExpression("abc", "formatstring('abc', 'd', 'e')");
        }

        [Fact]
        public void ExceptionOnMissingArguments1()
        {
            AssertExpressionException<ParserException>("formatstring('abc{0}')");
        }

        [Fact]
        public void ExceptionOnMissingArguments2()
        {
            AssertExpressionException<ParserException>("formatstring('{2}abc')");
        }

        [Fact]
        public void SimpleFormatting()
        {
            AssertExpression("abc d", "formatstring('abc {0}', 'd', 'e')");
            AssertExpression("abc d e", "formatstring('abc {0} {1}', 'd', 'e')");
            AssertExpression("abc e d", "formatstring('abc {1} {0}', 'd', 'e')");
        }

        [Fact]
        public void FormattingWithVariables()
        {
            var variables = new Dictionary<string, string> {{"x", "'def'"}, {"y", "'ghi'"}, {"z", @"'\'jkl\''"}};

            AssertExpression("abc def ghi", "formatstring('abc {0} {1}', @x, @y)", variables);
            AssertExpression("abc def ghi 'jkl'", "formatstring('abc {0} {1} {2}', @x, @y, @z)", variables);
        }

        [Fact]
        public void FormatGradients()
        {
            var input =
                @"
#gradients {
  @from: #444;
  @to: #999;
  @ffgradient: ""-moz-linear-gradient(top, {0}, {1})"";
  @wkgradient: ""-webkit-gradient(linear,left top,left bottom,color-stop(0, {0}),color-stop(1, {1}))"";
  @iegradient: ""progid:DXImageTransform.Microsoft.gradient(startColorstr='{0}', endColorstr='{1}')"";
  @ie8gradient: ""\""progid:DXImageTransform.Microsoft.gradient(startColorstr='{0}', endColorstr='{1}')\"""";

  background-image: formatString(@ffgradient, @from, @to);  // FF3.6
  background-image: formatString(@wkgradient, @from, @to);  // Saf4+, Chrome
  filter:           formatString(@iegradient, @from, @to);  // IE6,IE7
  -ms-filter:       formatString(@ie8gradient, @from, @to); // IE8
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

        [Fact]
        public void EscapeFunction()
        {
            var input = @"
#built-in {
  escaped: e(""-Some::weird(#thing, y)"");
}
";
            var expected = @"
#built-in {
  escaped: -Some::weird(#thing, y);
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ShortFormatFunction()
        {
            var input = @"
#built-in {
  @r: 32;
  format: %(""rgb(%d, %d, %d)"", @r, 128, 64);
}
";
            var expected = @"
#built-in {
  format: ""rgb(32, 128, 64)"";
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ShortFormatFunctionAcceptingString()
        {
            var input = @"
#built-in {
  format-string: %(""hello %s"", ""world"");
}
";
            var expected = @"
#built-in {
  format-string: ""hello world"";
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ShortFormatFunctionUrlEncode()
        {
            var input = @"
#built-in {
  format-url-encode: %('red is %A', #ff1000);
}
";
            var expected = @"
#built-in {
  format-url-encode: 'red is %23ff1000';
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void EscapeAndShortFormatFunction()
        {
            var input = @"
#built-in {
  @r: 32;
  eformat: e(%(""rgb(%d, %d, %d)"", @r, 128, 64));
}
";
            var expected = @"
#built-in {
  eformat: rgb(32, 128, 64);
}";

            AssertLess(input, expected);
        }
    }
}