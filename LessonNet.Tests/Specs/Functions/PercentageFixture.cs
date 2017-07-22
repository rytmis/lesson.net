using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    

    public class PercentageFixture : SpecFixtureBase
    {
        [Fact]
        public void TestPercentage()
        {
            AssertExpression("2500%", "percentage(25)");
            AssertExpression("50%", "percentage(.5)");
            AssertExpression("100%", "percentage(1)");
            AssertExpression("25%", "percentage(25 / 100)");
            AssertExpression("25%", "percentage(0.25px)");
            AssertExpression("25%", "percentage(0.25%)");
        }

        [Fact]
        public void TestPercentageChecksTypes()
        {
            AssertExpressionError("Expected number in function 'percentage', found #ccc", 11, "percentage(#ccc)");
            AssertExpressionError("Expected number in function 'percentage', found 'string'", 11, "percentage('string')");
        }
    }
}