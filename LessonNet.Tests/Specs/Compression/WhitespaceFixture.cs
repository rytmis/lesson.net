// ReSharper disable ConvertToConstant.Local

using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
    

    public class WhitespaceFixture : CompressedSpecFixtureBase
    {
        [Fact]
        public void Whitespace()
        {
            var input =
                @"
.whitespace { color: white; }
.whitespace{color:white;}
.whitespace { color: white ; }
.whitespace 
{
color:
white;
}
.whitespace { color : white; }
";

            var expected = ".whitespace{color:white}.whitespace{color:white}.whitespace{color:white}.whitespace{color:white}.whitespace{color:white}";

            AssertLess(input, expected);
        }

        [Fact]
        public void Whitespace2()
        {
            var input = @"
.white,
.space,
.mania
{ 
 color
      :
        white;
}";

            var expected = ".white,.space,.mania{color:white}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NoSemiColon1()
        {
            var input = ".no-semi-colon { color: white }";

            var expected = ".no-semi-colon{color:white}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NoSemiColon2()
        {
            var input = @"
.no-semi-colon {
  color: white;
  white-space: pre
}";

            var expected = ".no-semi-colon{color:white;white-space:pre}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NoSemiColon3()
        {
            var input = ".no-semi-colon { border: 2px solid white }";

            var expected = ".no-semi-colon{border:2px solid white}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NewLines1()
        {
            var input =
                @"
.newlines {
  background: the,
              
great,
              wall;
  border: 2px
          solid
          black;
}";

            var expected = ".newlines{background:the,great,wall;border:2px solid black}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NewLines2()
        {
            var input =
                @"
.newlines,
.are
.bad {
  foo: bar;
}";

            var expected = ".newlines,.are .bad{foo:bar}";

            AssertLess(input, expected);
        }

        [Fact]
        public void Empty()
        {
            var input = ".empty { }";

            var expected = "";

            AssertLess(input, expected);
        }

        //
        // CSS/LESS may contain empty declarations. Instead of throwing an exception,
        // one can simply ignore them, just like browsers would do.
        //
        [Fact]
        public void ConsecutiveSemicolons()
        {
            var input = ".semicolon { background:red;; color:blue; }";

            var expected = ".semicolon{background:red;color:blue}";

            AssertLess(input, expected);
        }

        //
        // CSS/LESS may contain multiple empty declarations. Instead of throwing an
        // exception, one can simply ignore them, just like browsers would do.
        //
        [Fact]
        public void ConsecutiveMultipleSemicolons()
        {
            var input = ".semicolon { background:red;;; color:blue; }";

            var expected = ".semicolon{background:red;color:blue}";

            AssertLess(input, expected);
        }

        //
        // CSS/LESS may contain declarations which have only whitespace characters
        // inside it. Instead of throwing an exception, one can simply ignore them,
        // just like browsers would do.
        //
        [Fact]
        public void ConsecutiveSemicolonsWithWhitespace()
        {
            var input = @".semicolon { background:red;   
  ; color:blue; }";

            var expected = ".semicolon{background:red;color:blue}";

            AssertLess(input, expected);
        }

        //
        // CSS/LESS may contain empty declaration at the end of the declaration block.
        // Instead of throwing an exception, one can simply ignore it, just like browsers
        // would do.
        //
        [Fact]
        public void ConsecutiveSemicolonsBeforeClosingCurlyBracket()
        {
            var input = ".semicolon { background:red; color:blue;; }";

            var expected = ".semicolon{background:red;color:blue}";

            AssertLess(input, expected);
        }
    }
}