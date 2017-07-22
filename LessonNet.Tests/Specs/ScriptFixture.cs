using Xunit;

namespace LessonNet.Tests.Specs
{
    

    public class ScriptFixture : SpecFixtureBase
    {
		// Unsupported
        // [Fact]
        public void ScriptExpressions()
        {
            AssertExpression("42", "`42`");
            AssertExpression("4", "`2 + 2`");
            AssertExpression("'hello world'", "`'hello world'`");
        }

        [Fact]
        public void ScriptUnsupported()
        {
            AssertExpression("[script unsupported]", "`42`");
            AssertExpression("[script unsupported]", "`2 + 2`");
            AssertExpression("[script unsupported]", "`'hello world'`");
        }

		// Unsupported
        // [Fact]
        public void ScriptHasAccessToVariablesInScope1()
        {
            var input = @"
.scope {
  @foo: 42;
  var: `this.foo`;
}";

            var expected = @"
.scope {
  var: 42;
}";

            AssertLess(input, expected);
        }

		// Unsupported
        // [Fact]
        public void ScriptHasAccessToVariablesInScope2()
        {
            var input = @"
@foo: 42;
.scope {
  var: `this.foo`;
}";

            var expected = @"
.scope {
  var: 42;
}";

            AssertLess(input, expected);
        }
    }
}
