using System.Collections.Generic;
using Xunit;

namespace LessonNet.Tests.Specs.Compression
{
	public class ImportFixture : CompressedSpecFixtureBase
    {
        protected override Dictionary<string, string> SetupImports()
        {
            var imports = new Dictionary<string, string>();

            imports["import/import-test-a.less"] = @"
@import ""import-test-b.less"";
@a: 20%;
";
            imports["import/import-test-b.less"] =
                @"
@import 'import-test-c';

@b: 100%;

.mixin {
  height: 10px;
  color: @c;
}
";
            imports["import/import-test-c.less"] =
                @"
@import ""import-test-d.css"";
@c: red;

#import {
  color: @c;
}
";

	        return imports;
        }

        [Fact]
        public void Imports()
        {
            var input =
                @"
@import url(""import/import-test-a.less"");
//@import url(""import/import-test-a.less"");

#import-test {
  .mixin;
  width: 10px;
  height: @a + 10%;
}
";

            var expected = "@import \"import-test-d.css\";#import{color:red}.mixin{height:10px;color:red}#import-test{height:10px;color:#ff0000;width:10px;height:30%}";

            AssertLess(input, expected);
        }

    }
}