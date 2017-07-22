using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
    

    public class Css3Fixture : CompressedSpecFixtureBase
    {
        [Fact]
        public void MediaDirectiveEmpty1()
        {
            var input = @"
@media only screen and (min-width: 768px) and (max-width: 959px) {
}
";

            AssertLess(input, "");
        }

        [Fact]
        public void MediaDirectiveEmpty2()
        {
            // optimally this would compress to "" but this isn't implemented
            // so just test the output is at least valid.
            var input = @"
@media only screen and (min-width: 768px) and (max-width: 959px) {
  .class {
  }
}
";

            AssertLess(input, "");
        }
    }
}
