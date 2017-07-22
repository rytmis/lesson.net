using LessonNet.Parser;
using Xunit;

namespace LessonNet.Tests.Specs
{
    public class StrictMathFixture : SpecFixtureBase
    {
		
		protected override EvaluationContext CreateContext(string input) {
			return new EvaluationContext(new LessTreeParser(), new InMemoryFileResolver(input) {
				Imports = SetupImports()
			}, StrictMath);
		}

	    private bool StrictMath { get; set; }

        [Fact]
        public void StrictMathNoParenthesesLeavesExpressionUntouched() {
	        StrictMath = true;

            AssertExpressionUnchanged("10px / 12px");
            AssertExpressionUnchanged("calc(10px + 12px)");
        }

        [Fact]
        public void StrictMathParenthesesEvaluatesExpression()
        {
	        StrictMath = true;

			AssertExpression("1.2px / 2px", "(12px / 10px) / (8px / 4px)");
        }

        [Fact]
        public void NonStrictMathWithoutParenthesesEvaluatesExpression()
        {
            AssertExpression("1.2px", "12px / 10px");
            AssertExpression("0.6px", "(12px / 10px) / (8px / 4px)");
        }

        [Fact]
        public void StrictMathKeepsNegativeValuesIntact()
        {
	        StrictMath = true;

            AssertExpressionUnchanged("-1000px");
        }

        [Fact]
        public void ComplexStrictMathExpression()
        {
	        StrictMath = true;

            var input = @"
@formInputTextDefaultWidth: 300px;
@formElementHorizontalPadding: 20px;
@formTextAreaDefaultWidth: ceil((((@formInputTextDefaultWidth + @formElementHorizontalPadding) + 13px) * 2));
.test {
  width: @formTextAreaDefaultWidth;
}";

            var expected = @"
.test {
  width: 666px;
}";

            AssertLess(input, expected);
        }
    }
}
