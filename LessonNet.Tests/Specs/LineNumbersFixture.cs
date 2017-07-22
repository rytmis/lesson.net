using System.Collections.Generic;
using Xunit;

namespace LessonNet.Tests.Specs
{
	public class LineNumbersFixture : SpecFixtureBase
    {
        protected Dictionary<string, string> Imports { get; set; }


        [Fact]
        public void LineNumbers()
        {
            var input = @"
.first {
  color: red;
}

.second {
  color: green;

  .inner {
    color: blue;
  }
}
";

            var expected= @"
/* test.less:L1 */
.first {
  color: red;
}
/* test.less:L5 */
.second {
  color: green;
}
/* test.less:L8 */
.second .inner {
  color: blue;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void LineNumbersAndFileNameWhenImporting()
        {
            Imports["import.less"] = @".import { color: blue; }";

            var input = @"
@import 'import.less';
";

            var expected = @"
/* import.less:L1 */
.import {
  color: blue;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void LineNumbersWhenCallingMixin()
        {
            var input = @"
.mixin() {
  color: red;

  .inner { // currently indicates this line
    color: blue;
  }
}

.test {
  .mixin(); // should maybe indicate this line! or both?
}
";

            var expected= @"
/* test.less:L9 */
.test {
  color: red;
}
/* test.less:L4 */
.test .inner {
  color: blue;
}
";

            AssertLess(input, expected);
        }
    }
}