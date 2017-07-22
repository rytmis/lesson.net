using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
	public class KeepFirstCommentFixture : SpecFixtureBase
    {
        [Fact]
        public void KeepsFirstCommentOnly1()
        {
            var input = "/** Comment *//** Comment*/";
            var expected = "/** Comment */";

            AssertLess(input, expected);
        }

        [Fact]
        public void KeepsFirstCommentOnly2()
        {
            var input = @"/** Comment */
.mixin() {
  /** Comment*/
  foo: bar;
}
.foo {
  .mixin();
}
";
            var expected = @"/** Comment */.foo{foo:bar}";

            AssertLess(input, expected);
        }

        [Fact]
        public void KeepsDoubleStarCommentOnly1()
        {
            var input = "/* Comment1 *//** Comment2 *//** Comment3 */";
            var expected = "/** Comment2 */";

            AssertLess(input, expected);
        }

        [Fact]
        public void KeepsDoubleStarCommentOnly2()
        {
            var input = "// comment";
            var expected = "";

            AssertLess(input, expected);
        }

        [Fact]
        public void KeepsDoubleStarCommentOnly3()
        {
            var input = @"/* Comment1 */
/*************
 * (c) msg   *
 *************//** Comment3 */";

            var expected = @"
/*************
 * (c) msg   *
 *************/";

            AssertLess(input, expected);
        }
    }
}