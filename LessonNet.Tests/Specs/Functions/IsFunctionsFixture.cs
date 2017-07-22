using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    

    public class IsFunctionsFixture : SpecFixtureBase
    {
        [Fact]
        public void TestIsColor()
        {
            AssertExpression("true", "iscolor(#ddd)");
            AssertExpression("true", "iscolor(red)");
            AssertExpression("true", "iscolor(rgb(0, 0, 0))");
            AssertExpression("true", @"iscolor(color(""#FFF""))");
            AssertExpression("false", "iscolor(evil red)");
            AssertExpression("false", "iscolor(evilred)");
        }

        [Fact]
        public void TestIsKeyword()
        {
            AssertExpression("true", "iskeyword(evilred)");
            AssertExpression("false", "iskeyword(red)");
            AssertExpression("false", @"iskeyword(""hello"")");
            AssertExpression("true", "iskeyword(hello)");
        }

        [Fact]
        public void TestIsNumber()
        {

            AssertExpression("true", "isnumber(32)");
            AssertExpression("true", "isnumber(32px)");
            AssertExpression("false", "isnumber(red)");
            AssertExpression("false", @"isnumber(""hello"")");
        }

        [Fact]
        public void TestIsString()
        {

            AssertExpression("true", @"isstring(""hello"")");
            AssertExpression("true", @"isstring(""red"")");
            AssertExpression("false", "isstring(red)");
            AssertExpression("false", @"isstring(32px)");
        }

        [Fact]
        public void TestIsPixel()
        {
            AssertExpression("true", "ispixel(32px)");
            AssertExpression("true", "ispixel(0px)");
            AssertExpression("true", "ispixel(3px - 2px)");
            AssertExpression("false", "ispixel(0)");
            AssertExpression("false", "ispixel(1rem)");
            AssertExpression("false", "ispixel(red)");
        }

        [Fact]
        public void TestIsPercentage()
        {

            AssertExpression("true", "ispercentage(32%)");
            AssertExpression("true", "ispercentage(0%)");
            AssertExpression("true", "ispercentage(percentage(0.5))");
            AssertExpression("true", "ispercentage(3% - 2%)");
            AssertExpression("false", "ispercentage(0)");
            AssertExpression("false", "ispercentage(1rem)");
            AssertExpression("false", "ispercentage(red)");
        }

        [Fact]
        public void TestIsEm()
        {
            AssertExpression("true", "isem(32.4em)");
            AssertExpression("true", "isem(0em)");
            AssertExpression("true", "isem(3em + 2em)");
            AssertExpression("true", "isem(3em / 2)");
            AssertExpression("true", "isem(3em / 2em)");
            AssertExpression("false", "isem(0)");
            AssertExpression("false", "isem(1rem)");
            AssertExpression("false", "isem(red)");
        }
    }
}