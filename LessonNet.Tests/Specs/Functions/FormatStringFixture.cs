using System.Collections.Generic;
using LessonNet.Parser;
using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
	public class FormatStringFixture : SpecFixtureBase
    {
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