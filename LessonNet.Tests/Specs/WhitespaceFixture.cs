// ReSharper disable ConvertToConstant.Local

using Xunit;

namespace LessonNet.Tests.Specs
{
    

    public class WhitespaceFixture : SpecFixtureBase
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

            var expected =
                @"
.whitespace {
  color: white;
}
.whitespace {
  color: white;
}
.whitespace {
  color: white;
}
.whitespace {
  color: white;
}
.whitespace {
  color: white;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void Whitespace2()
        {
            var input = @"
.white,
.space, .mania
{ 
 color
      :
        white;
}";

            var expected = @"
.white,
.space,
.mania {
  color: white;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NoSemiColon1()
        {
            var input = ".no-semi-colon { color: white }";

            var expected = @"
.no-semi-colon {
  color: white;
}";

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

            var expected = @"
.no-semi-colon {
  color: white;
  white-space: pre;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NoSemiColon3()
        {
            var input = ".no-semi-colon {border: 2px solid white}";

            var expected = @"
.no-semi-colon {
  border: 2px solid white;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NewLines()
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

            var expected =
                @"
.newlines {
  background: the, great, wall;
  border: 2px solid black;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void NewLines2()
        {
            var input =
                @"
.new
.lines {
  background: new
line;
}";

            var expected =
                @"
.new .lines {
  background: new line;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void Empty()
        {
            var input = @"
.empty {
  
}";

            var expected = "";

            AssertLess(input, expected);
        }

        [Fact]
        public void Tabs()
        {
            var input = "div\ta\t{\tdisplay:\tnone;\t}";

            var expected = @"
div a {
  display: none;
}";

            AssertLess(input, expected);
        }
    }
}