using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    

    public class GrayscaleFixture : SpecFixtureBase
    {
        [Fact]
        public void TestGreyscale()
        {
            AssertExpression("#bbbbbb", "greyscale(#abc)");
            AssertExpression("#808080", "greyscale(#f00)");
            AssertExpression("#808080", "greyscale(#00f)");
            AssertExpression("#ffffff", "greyscale(#fff)");
            AssertExpression("#000000", "greyscale(#000)");
        }
    }
}