using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
    

    public class SelectorsFixture : CompressedSpecFixtureBase
    {
        [Fact]
        public void ParentSelector1()
        {
            var input =
                @"
h1, h2, h3 {
  a, p {
    &:hover {
      color: red;
    }
  }
}
";

            var expected = "h1 a:hover,h2 a:hover,h3 a:hover,h1 p:hover,h2 p:hover,h3 p:hover{color:red}";

            AssertLess(input, expected);
        }

        [Fact]
        public void IdSelectors()
        {
            var input =
                @"
#all { color: blue; }
#the { color: blue; }
#same { color: blue; }
";

            var expected = "#all{color:blue}#the{color:blue}#same{color:blue}";

            AssertLess(input, expected);
        }

        [Fact]
        public void Tag()
        {
            var input = @"
td {
  margin: 0;
  padding: 0;
}
";

            var expected = "td{margin:0;padding:0}";

            AssertLess(input, expected);
        }

        [Fact]
        public void TwoTags()
        {
            var input = @"
td, input {
  line-height: 1em;
}
";

            var expected = "td,input{line-height:1em}";

            AssertLess(input, expected);
        }

        [Fact]
        public void MultipleTags()
        {
            var input =
                @"
ul, li, div, q, blockquote, textarea {
  margin: 0;
}
";

            var expected = "ul,li,div,q,blockquote,textarea{margin:0}";

            AssertLess(input, expected);
        }


        [Fact]
        public void DecendantSelectorWithTabs()
        {
            var input = "td \t input { line-height: 1em; }";

            var expected = "td input{line-height:1em}";

            AssertLess(input, expected);
        }

        [Fact]
        public void EmptySelectorRemoved()
        {
            var input = @"
.class {
}
";

            AssertLess(input, "");
        }
    }
}