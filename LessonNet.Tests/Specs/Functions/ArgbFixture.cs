using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
	public class ArgbFixture : SpecFixtureBase
    {
        [Fact]
        public void TestArgb()
        {
            AssertExpression("#ff123456", "argb(#123456)");
            AssertExpression("#00000000", "argb(transparent)");
            AssertExpression("#80ffffff", "argb(rgba(255, 255, 255, 0.5))");
        }
    }
}