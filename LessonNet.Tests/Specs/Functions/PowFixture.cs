using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    
    
    public class PowFixture : SpecFixtureBase
    {
        [Fact]
        public void Abs()
        {
            AssertExpression("8", "pow(2, 3)");
            AssertExpression("0.25", "pow(2, -2)");
        }

        [Fact]
        public void AbsInfo()
        {
            var absInfo = "pow(number, number) is not supported by less.js, so this will work but not compile with other less implementations.";

            AssertExpressionLogMessage(absInfo, "pow(3, 5)");
        }
    }
}
