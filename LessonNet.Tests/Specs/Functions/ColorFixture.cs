using Xunit;

namespace LessonNet.Tests.Specs.Functions
{

	public class ColorFixture : SpecFixtureBase
	{
		[Fact]
		public void TestColor()
		{
			AssertExpression("#ff0000", @"color(""#ff0000"")");
			AssertExpression("#0ff", @"color(""#0ff"")");
		}

		[Fact]
		public void AcceptsColorKeywords()
		{
			AssertExpression("red", "color(\"red\")");
			AssertExpression("green", "color(\"green\")");
			AssertExpression("blue", "color(\"blue\")");
			AssertExpression("black", "color(\"black\")");
			AssertExpression("white", "color(\"white\")");
		}
	}
}