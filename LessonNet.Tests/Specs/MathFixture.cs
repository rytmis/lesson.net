using System.Collections.Generic;
using Xunit;

namespace LessonNet.Tests.Specs {
	public class MathFixture : SpecFixtureBase {
		[Fact]
		public void DivisionOperatorPrecedence() {
			var variables = new Dictionary<string, string>();
			variables["a"] = "2";

			AssertExpression("2", "1 + 2 / @a", variables);
			AssertExpression("0", "1 - 2 / @a", variables);

			AssertExpression("5", "6 / @a + @a", variables);
			AssertExpression("1", "6 / @a - @a", variables);
		}

		[Fact]
		public void MultiplicationOperatorPrecedence() {
			AssertExpression("5", "1 + 2 * 2");
			AssertExpression("-3", "1 - 2 * 2");

			AssertExpression("5", "2 * 2 + 1");
			AssertExpression("3", "2 * 2 - 1");
		}
	}
}
