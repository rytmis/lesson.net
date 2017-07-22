using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    

    public class RgbFixture : SpecFixtureBase
    {
        [Fact]
        public void TestRgb()
        {
            AssertExpression("#123456", "rgb(18, 52, 86)");
            AssertExpression("#beaded", "rgb(190, 173, 237)");
            AssertExpression("#00ff7f", "rgb(0, 255, 127)");
        }

        [Fact]
        public void TestRgbPercent()
        {
            AssertExpression("#123456", "rgb(7.1%, 20.4%, 33.7%)");
            AssertExpression("#beaded", "rgb(74.7%, 173, 93%)");
            AssertExpression("#beaded", "rgb(190, 68%, 237)");
            AssertExpression("#00ff80", "rgb(0%, 100%, 50%)");
        }

        [Fact]
        public void TestRgbOverflows()
        {
            AssertExpression("#ff0101", "rgb(256, 1, 1)");
            AssertExpression("#01ff01", "rgb(1, 256, 1)");
            AssertExpression("#0101ff", "rgb(1, 1, 256)");
            AssertExpression("#01ffff", "rgb(1, 256, 257)");
            AssertExpression("#000101", "rgb(-1, 1, 1)");
        }

        [Fact]
        public void TestRgbTestPercentBounds()
        {
            AssertExpression("#ff0000", "rgb(100.1%, 0, 0)");
            AssertExpression("#000000", "rgb(0, -0.1%, 0)");
            AssertExpression("#0000ff", "rgb(0, 0, 101%)");
        }

        [Fact]
        public void TestRgbTestsTypes()
        {
            AssertExpressionError("Expected number in function 'rgb', found \"foo\"", 0, "rgb(\"foo\", 10, 12)");
            AssertExpressionError("Expected number in function 'rgb', found \"foo\"", 0, "rgb(10, \"foo\", 12)");
            AssertExpressionError("Expected number in function 'rgb', found \"foo\"", 0, "rgb(10, 10, \"foo\")");
        }
    }
}