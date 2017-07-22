using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    

    public class HexFixture : SpecFixtureBase
    {
        [Fact]
        public void TestHex()
        {
            AssertExpression("00", "hex(0)");
            AssertExpression("99", "hex(153)");
            AssertExpression("F0", "hex(240)");
            AssertExpression("FF", "hex(255)");
        }

        [Fact]
        public void ValuesBelow_0_AreInterpretedAs_0()
        {
            AssertExpression("00", "hex(-1)");
        }

        [Fact]
        public void ValuesAbove_255_AreInterpretedAs_FF()
        {
            AssertExpression("FF", "hex(999)");
        }
    }
}