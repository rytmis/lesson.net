using Xunit;

namespace LessonNet.Tests.Specs.Functions
{
    public class DefaultFixture : SpecFixtureBase
    {
        [Fact]
        public void DefaultFunctionInRuleResultsInText() {
            AssertLessUnchanged("rule: default();");
        }
    }
}
