using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
    

    public class ColorsFixture : CompressedSpecFixtureBase
    {
        [Fact]
        public void Colors()
        {
            AssertExpression("#fea", "#fea");
            AssertExpression("#0000ff", "#0000ff");
            AssertExpression("blue", "blue");
        }

        [Fact]
        public void Should_not_compress_IE_ARGB()
        {
            AssertExpressionUnchanged("#ffaabbcc");
            AssertExpressionUnchanged("#aabbccdd");
        }
        
        [Fact]
        public void Overflow()
        {
            AssertExpression("#000000", "#111111 - #444444");
            AssertExpression("#ffffff", "#eee + #fff");
            AssertExpression("#ffffff", "#aaa * 3");
            AssertExpression("#00ff00", "#00ee00 + #009900");
            AssertExpression("#ff0000", "#ee0000 + #990000");
        }

        [Fact]
        public void Gray()
        {
            AssertExpression("#888888", "rgb(136, 136, 136)");
            AssertExpression("#808080", "hsl(50, 0, 50)");
        }
    }
}