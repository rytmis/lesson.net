using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using System.Reflection;
using LessonNet.Parser;
using Xunit;

namespace LessonNet.Tests.Specs {
	public class ImportFixture : SpecFixtureBase {
		protected override Dictionary<string, MockFileData> SetupImports() {
			var imports = new Dictionary<string, MockFileData> {
				[@"c:/absolute/file.less"] = @"
.windowz .dos {
  border: none;
}
",
				[@"import/error.less"] = @"
.windowz .dos {
  border: none;
}
.error_mixin {
  .throw_error();
}
",
				[@"import/error2.less"] = @"
.windowz .dos {
  border: none;
}
.error_mixin() {
  .throw_error();
}
",
				["import/other-protocol-test.less"] = @"
.first {
    background-image: url('http://some.com/file.gif');
}
.second {
    background-image: url('https://some.com/file.gif');
}
.third {
    background-image: url('ftp://some.com/file.gif');
}
.fourth {
    background-image: url('data:xxyhjgjshgjs');
}
",
				["import/twice/with/different/paths.less"] = @"
@import-once ""../twice.less"";
@import-once ""../other.less"";
",
				["import/twice/with/other.less"] = @"
@import-once ""twice.less"";
",
				["import/twice/with/twice.less"] = @"
body { background-color: foo; }
",
				["import/import-test-a.less"] = @"
@import ""import-test-b.less"";
@a: 20%;
",
				["import/import-test-b.less"] = @"
@import 'import-test-c';

@b: 100%;

.mixin {
  height: 10px;
  color: @c;
}
",
				["import/import-test-c.less"] = @"
@import ""import-test-d.css"";
@c: red;

#import {
  color: @c;
}
",
				["import/first.less"] = @"
@import ""sub1/second.less"";

@path: ""../image.gif"";
#first {
  background: url('../image.gif');
  background: url(../image.gif);
  background: url(@path);
}
",
				["import/sub1/second.less"] = @"
@pathsep: '/';
#second {
  background: url(../image.gif);
  background: url(image.gif);
  background: url(sub2/image.gif);
  background: url(/sub2/image.gif);
  background: url(~""@{pathsep}sub2/image2.gif"");
}
",
				["/import/absolute.less"] = @"body { background-color: black; }",
				["import/define-variables.less"] = @"@color: 'blue';",
				["import/use-variables.less"] = @".test { background-color: @color; }",
				["empty.less"] = @"",
				["rule.less"] = @".rule { color: black; }",
				["../import/relative-with-parent-dir.less"] = @"body { background-color: foo; }",
				["foo.less"] = @"@import ""foo/bar.less"";",
				["foo/bar.less"] = @"@import ""../lib/color.less"";",
				["lib/color.less"] = "body { background-color: foo; }",
				["247-2.less"] = @"
@color: red;
text {
  color: @color;
}",
				["247-1.less"] = @"
#nsTwoCss {
  .css() {
    @import '247-2.less';
  }
}",
				["foourl.less"] = @"@import url(""foo/barurl.less"");",
				["foo/barurl.less"] = @"@import url(""../lib/colorurl.less"");",
				["lib/colorurl.less"] = "body { background-color: foo; }",
				["something.css"] = @"body { background-color: foo; invalid ""; }",
				["isless.css"] = @"
@a: 9px;
body { margin-right: @a; }",
				["vardef.less"] = @"@var: 9px;",
				["css-as-less.css"] = @"@var1: 10px;",
				["arbitrary-extension-as-less.ext"] = @"@var2: 11px;",
				["simple-rule.less"] = ".rule { background-color: black; }",
				["simple-rule.css"] = ".rule { background-color: black; }",
				["media-scoped-rules.less"] = @"@media (screen) { 
    .rule { background-color: black; }
    .another-rule { color: white; }
}",
				["nested-rules.less"] = @"
.parent-selector {
    .rule { background-color: black; }
    .another-rule { color: white; }
}",
				["imports-simple-rule.less"] = @"
@import ""simple-rule.less"";
.rule2 { background-color: blue; }",
				["two-level-import.less"] = @"
@import ""simple-rule.less"";
.rule3 { background-color: red; }",
				["reference/main.less"] = @"
@import ""mixins/test.less"";

.mixin(red);
",
				["reference/mixins/test.less"] = @"
.mixin(@arg) {
    .test-ruleset {
        background-color: @arg;
    }
}
",
				["reference/ruleset-with-child-ruleset-and-rules.less"] = @"
.parent {
    .child {
        background-color: black;
    }

    background-color: blue;
}
",
				["two-level-import.less"] = @"
@import ""simple-rule.less"";
.rule3 { background-color: red; }",
				["directives.less"] = @"
@font-face {
  font-family: 'Glyphicons Halflings';
}
",
				["mixin-loop.less"] = @"
@grid-columns: 12;
.float-grid-columns(@class) {
  .col(@index) { // initial    
    .col((@index + 1), """");
  }
  .col(@index, @list) when (@index =< @grid-columns) { // general
    .col((@index + 1), """");
  }
  .col(@index, @list) when (@index > @grid-columns) { // terminal

  }
  .col(1); // kickstart it
}

// Create grid for specific class
.make-grid(@class) {
  .float-grid-columns(@class);
}


@media (screen) {
  .make-grid(sm);
}
",
				["partial-reference-extends-another-reference.less"] = @"
.parent {
  .test {
    color: black;
  }
}

.ext {
  &:extend(.test all);
}
",
				["exact-reference-extends-another-reference.less"] = @"
.test {
  color: black;
}

.ext {
  &:extend(.test);
}
",
				["reference-with-multiple-selectors.less"] = @"
.test,
.test2 {
  color: black;
}
",
				["comments.less"] = @"
/* This is a comment */
",
				["math.less"] = @"
.rule {
    width: calc(10px + 2px);
}
",
				["generated-selector.less"] = @"
@selector: ~"".rule"";
@{selector} {
  color: black;
}
",
				["multiple-generated-selectors.less"] = @"
@grid-columns: 12;

.float-grid-columns(@class) {
  .col(@index) { // initial
    @item: ~"".col-@{class}-@{index}"";
    .col((@index + 1), @item);
  }
  .col(@index, @list) when (@index =< @grid-columns) { // general
    @item: ~"".col-@{class}-@{index}"";
    .col((@index + 1), ~""@{list}, @{item}"");
  }
  .col(@index, @list) when (@index > @grid-columns) { // terminal
    @{list} {
      float: left;
    }
  }
  .col(1); // kickstart it
}

.float-grid-columns(xs);
",
				["import-in-mixin/mixin-definition.less"] = @"
.import() {
  @import ""relative-import.less"";
}
",
				["import-in-mixin/relative-import.less"] = @"
.rule {
  color: black;
}
",
				["reference-mixin-issue.less"] = @"
.mix-me
{
    color: red;
    @media (min-width: 100px)
    {
        color: blue;
    }
    .mix-me-child
    {
        background-color: black;
    }
}",
				["nested-import-interpolation-1.less"] = @"
@var: ""2"";
@import ""nested-import-interpolation-@{var}.less"";
",
				["nested-import-interpolation-2.less"] = @"
body { background-color: blue; }
"
			};









































			return imports;
		}

		[Fact]
		public void Test247() {
			var input = @"
@import '247-1.less';
#nsTwo {
  #nsTwoCss > .css();
}";
			var expected = @"
#nsTwo text {
  color: red;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void Imports() {
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

			var expected =
				@"
@import ""import-test-d.css"";
#import {
  color: red;
}
.mixin {
  height: 10px;
  color: red;
}
#import-test {
  height: 10px;
  color: red;
  width: 10px;
  height: 30%;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void OtherProtocolImportTest1() {
			var input = @"
@import 'import/other-protocol-test.less';
";
			var expected = @"
.first {
  background-image: url('http://some.com/file.gif');
}
.second {
  background-image: url('https://some.com/file.gif');
}
.third {
  background-image: url('ftp://some.com/file.gif');
}
.fourth {
  background-image: url('data:xxyhjgjshgjs');
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void OtherProtocolImportTest2() {
			var input = @"
@import url(http://fonts.googleapis.com/css?family=Open+Sans:regular,bold);";


			AssertLessUnchanged(input);
		}

		[Fact]
		public void OtherProtocolImportTest3() {
			var input = @"
@import url('c:/absolute/file.less');";
			var expected = @"
.windowz .dos {
  border: none;
}
";

			AssertLess(input, expected);
		}

//		[Fact]
//		public void ErrorInImport()
//		{
//			var input = @"
//@import ""import/error.less"";";

//			AssertError(@"
//.throw_error is undefined on line 6 in file 'import/error.less':
//  [5]: .error_mixin {
//  [6]:   .throw_error();
//       --^
//  [7]: }", input);
//		}

//		[Fact]
//		public void ErrorInImport2()
//		{
//			var input = @"
//@import ""import/error2.less"";
//.a {
//  .error_mixin();
//}";

//			AssertError(@"
//.throw_error is undefined on line 6 in file 'import/error2.less':
//  [5]: .error_mixin() {
//  [6]:   .throw_error();
//       --^
//  [7]: }
//from line 3 in file 'test.less':
//  [3]:   .error_mixin();", input);
//		}

		[Fact]
		public void RelativeUrls() {
			var input =
				@"
@import url(""import/first.less"");
";

			var expected =
				@"
#second {
  background: url(import/image.gif);
  background: url(import/sub1/image.gif);
  background: url(import/sub1/sub2/image.gif);
  background: url(/sub2/image.gif);
  background: url(/sub2/image2.gif);
}
#first {
  background: url('image.gif');
  background: url(image.gif);
  background: url(""image.gif"");
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void RelativeUrlsWithRewritingOff() {
			var input =
				@"
@import url(""import/first.less"");
";

			var expected =
				@"
#second {
  background: url(../image.gif);
  background: url(image.gif);
  background: url(sub2/image.gif);
  background: url(/sub2/image.gif);
  background: url(/sub2/image2.gif);
}
#first {
  background: url('../image.gif');
  background: url(../image.gif);
  background: url(""../image.gif"");
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void AbsoluteUrls() {
			var input =
				@"
@import url(""/import/absolute.less"");
";

			var expected = @"body {
  background-color: black;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void RelativeUrlWithParentDirReference() {
			var input =
				@"
@import url(""../import/relative-with-parent-dir.less"");
";

			var expected = @"body {
  background-color: foo;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportFileExtensionNotNecessary() {
			var input = @"@import url(""import/import-test-c"");";

			var expected = @"
@import ""import-test-d.css"";
#import {
  color: red;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportForUrlGetsOutput() {
			var input =
				@"
@import url(""http://www.someone.com/external1.css"");
@import ""http://www.someone.com/external2.css"";
";

			AssertLessUnchanged(input);
		}

		//[Fact]
		//public void ImportForMissingLessFileThrowsError1()
		//{
		//	var input = @"@import url(""http://www.someone.com/external1.less"");";

		//	AssertError(".less cannot import non local less files [http://www.someone.com/external1.less].", input);
		//}

		//[Fact]
		//public void ImportForMissingLessFileThrowsError2()
		//{
		//	var input = @"@import ""external1.less"";";

		//	AssertError("You are importing a file ending in .less that cannot be found [external1.less].", input);
		//}

		//[Fact]
		//public void ImportForMissingLessFileThrowsError3()
		//{
		//	var input = @"@import ""http://www.someone.com/external1.less"";";

		//	AssertError(".less cannot import non local less files [http://www.someone.com/external1.less].", input);
		//}

		//[Fact]
		//public void ImportForMissingLessFileThrowsError4()
		//{
		//	var input = @"@import ""dll://someassembly#missing.less"";";

		//	AssertError("Unable to load resource [missing.less] in assembly [someassembly]", input);
		//}

		//[Fact]
		//public void ImportForMissingCssFileAsLessThrowsError()
		//{
		//	var input = @"@import ""dll://someassembly#missing.css"";";

		//	AssertError("Unable to load resource [missing.css] in assembly [someassembly]", input);
		//}

		[Fact]
		public void ImportForMissingLessFileThrowsExceptionThatIncludesFileName() {
			var input = @"@import ""external1.less"";";

			// TODO: Verify file name
			Assert.Throws<FileNotFoundException>(() => Evaluate(input));
		}

		[Fact]
		public void ImportCanNavigateIntoAndOutOfSubDirectory() {
			// Testing https://github.com/cloudhead/less.js/pull/514

			var input = @"@import ""foo.less"";";
			var expected = @"body {
  background-color: foo;
}";
			AssertLess(input, expected);
		}

		[Fact]
		public void ImportCanNavigateIntoAndOutOfSubDirectoryWithImport() {
			var input = @"@import url(""foourl.less"");";
			var expected = @"body {
  background-color: foo;
}";
			AssertLess(input, expected);
		}

		[Fact]
		public void ImportWithMediaSpecificationsSupported() {
			var input = @"
@import url(something.css) screen and (color) and (max-width: 600px);";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void ImportInlinedWithMediaSpecificationsSupported() {
			var input = @"
@import (inline) url(something.css) screen and (color) and (max-width: 600px);";

			var expected = @"
@media screen and (color) and (max-width: 600px) {
  body { background-color: foo; invalid ""; }
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void CanImportCssFilesAsLess() {
			var input = @"
@import (less) url(""isless.css"");
";
			var expected = @"
body {
  margin-right: 9px;
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void LessImportWithMediaSpecificationsConverted() {
			var input = @"
@import url(foo.less) screen and (color) and (max-width: 600px);";

			var expected = @"
@media screen and (color) and (max-width: 600px) {
  body {
    background-color: foo;
  }
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void LessImportWithMediaSpecificationsConvertedWithOnce() {
			var input = @"
@import-once url(foo.less) screen and (color) and (max-width: 600px);";

			var expected = @"
@media screen and (color) and (max-width: 600px) {
  body {
    background-color: foo;
  }
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void LessImportWithMediaSpecificationsConvertedMultipleRequirements() {
			var input = @"
@import url(foo.less) screen and (color) and (max-width: 600px), handheld and (min-width: 20em);";

			var expected = @"
@media screen and (color) and (max-width: 600px), handheld and (min-width: 20em) {
  body {
    background-color: foo;
  }
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportWithMediaSpecificationsSupportedWithVariable() {
			var input = @"
@maxWidth: 600px;
@requirement1: color;
@import url(something.css) screen and (@requirement1) and (max-width: @maxWidth);";

			var expected = @"
@import url(something.css) screen and (color) and (max-width: 600px);";

			AssertLess(input, expected);
		}

		[Fact]
		public void LessImportWithMediaSpecificationsConvertedWithVariable() {
			var input = @"
@maxWidth: 600px;
@requirement1: color;
@import url(foo.less) screen and (@requirement1) and (max-width: @maxWidth);";

			var expected = @"
@media screen and (color) and (max-width: 600px) {
  body {
    background-color: foo;
  }
}";

			AssertLess(input, expected);
		}

		[Fact(Skip = "Embedded resources are still WIP")]
		public void LessImportFromEmbeddedResource() {
			var input = @"
@import ""dll://dotless.Test.dll#dotless.Test.Resource.Embedded.less"";
@import ""dll://dotless.Test.dll#dotless.Test.Resource.Embedded2.less"";";

			var expected = @"
#import {
  color: red;
}
#import {
  color: blue;
}";

			AssertLess(input, expected);
		}

		//[Fact]
		//public void LessImportFromEmbeddedResourceWithDynamicAssembliesInAppDomain()
		//{
		//	Assembly assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("dotless.dynamic.dll"),
		//		AssemblyBuilderAccess.RunAndSave);

		//	Assembly.Load(new AssemblyName("dotless.Test.EmbeddedResource"));

		//	try
		//	{
		//		Evaluate(@"@import ""dll://dotless.Test.EmbeddedResource.dll#dotless.Test.EmbeddedResource.Embedded.less"";",
		//			GetEmbeddedParser(false, false, false));
		//	}
		//	catch (FileNotFoundException ex)
		//	{
		//		// If the import fails for the wrong reason (i.e., having the dynamic assembly loaded),
		//		// the failure will have a different exception message
		//		Assert.That(ex.Message, Is.EqualTo("You are importing a file ending in .less that cannot be found [nosuch.resource.less]."));
		//	}
		//}

		[Fact(Skip = "Embedded resources are still WIP")]
		public void CssImportFromEmbeddedResource() {
			var input = @"
@import ""dll://dotless.Test.dll#dotless.Test.Resource.Embedded.css"";";

			var expected = @"
.windowz .dos {
  border: none;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportTwiceImportsOnce() {
			var input = @"
@import ""lib/color.less"";
@import ""lib/color.less"";";

			var expected = @"
body {
  background-color: foo;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportOnceTwiceImportsOnce() {
			var input = @"
@import-once ""lib/color.less"";
@import-once ""lib/color.less"";";

			var expected = @"
body {
  background-color: foo;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportTwiceWithDifferentRelativePathsImportsOnce() {
			var input = @"
@import-once ""import/twice/with/different/paths.less"";";

			var expected = @"
body {
  background-color: foo;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void VariablesFromImportedFileAreAvailableToAnotherImportedFileWithinMediaBlock() {
			var input = @"
@import ""import/define-variables.less"";

@media only screen and (max-width: 700px)
{
    @import ""import/use-variables.less"";
}
";

			var expected = @"
@media only screen and (max-width: 700px) {
  .test {
    background-color: 'blue';
  }
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void EmptyImportDoesNotBreakSubsequentImports() {
			var input = @"
@import ""empty.less"";
@import ""rule.less"";

.test {
  .rule;
}
";

			var expected = @"
.rule {
  color: black;
}
.test {
  color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ExtendingNestedRulesFromReferenceImportsWorks() {
			var input = @"
@import (reference) ""nested-rules.less"";

.test:extend(.rule all) { }
";

			var expected = @"
.parent-selector .test {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ExtendingNestedReferenceRulesIgnoresRulesFromParentRuleset() {
			var input = @"
@import (reference) ""reference/ruleset-with-child-ruleset-and-rules.less"";

.test:extend(.child all) { }
";

			var expected = @"
.parent .test {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void VariableInterpolationInQuotedCssImport() {
			var input =
				@"
@var: ""foo"";

@import ""@{var}/bar.css"";
";

			var expected =
				@"
@import ""foo/bar.css"";
";

		}

		[Fact]
		public void VariableInterpolationInQuotedLessImport() {
			var input =
				@"
@component: ""color"";

@import ""lib/@{component}.less"";
";

			var expected =
				@"
body {
  background-color: foo;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void VariableInterpolationInNestedLessImport() {
			var input =
				@"
@import ""nested-import-interpolation-1.less"";
";

			var expected =
				@"
body {
  background-color: blue;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportMultipleImportsMoreThanOnce() {
			var input = @"
@import ""vardef.less"";
@var: 10px;
@import (multiple) ""vardef.less"";
.rule { width: @var; }
";

			var expected = @"
.rule {
  width: 9px;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportOptionalIgnoresFilesThatAreNotFound() {
			var input = @"
@var: 10px;
@import (optional) ""this-file-does-not-exist.less"";
.rule { width: @var; }
";

			var expected = @"
.rule {
  width: 10px;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportCssGeneratesImportDirective() {
			var input = @"
@import (css) ""this-file-does-not-exist.less"";
";

			var expected = @"
@import ""this-file-does-not-exist.less"";
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportLessParsesAnyExtensionAsLess() {
			var input = @"
@import (less) ""css-as-less.css"";
@import (less) ""arbitrary-extension-as-less.ext"";

.rule {
  width: @var1;
  height: @var2;
}
";

			var expected = @"
.rule {
  width: 10px;
  height: 11px;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportInlineIncludesContentsOfCssFile() {
			var input = @"
@import (inline) ""something.css"";
";

			var expected = @"
body { background-color: foo; invalid ""; }
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportReferenceAloneDoesNotProduceOutput() {
			var input = @"
@import (reference) ""simple-rule.less"";
";

			var expected = @"";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportReferenceDoesNotOutputMediaBlocks() {
			var input = @"
@import (reference) ""media-scoped-rules.less"";
";

			var expected = @"";

			AssertLess(input, expected);
		}


		[Fact]
		public void ImportReferenceDoesNotOutputRulesetsThatCallLoopingMixins() {
			var input = @"
@import (reference) ""mixin-loop.less"";
";

			var expected = @"";

			AssertLess(input, expected);
		}

		[Fact]
		public void PartialReferenceExtenderDoesNotCauseReferenceRulesetToBeOutput() {
			var input = @"
@import (reference) ""partial-reference-extends-another-reference.less"";
";

			AssertLess(input, @"");
		}

		[Fact]
		public void ExactReferenceExtenderDoesNotCauseReferenceRulesetToBeOutput() {
			var input = @"
@import (reference) ""exact-reference-extends-another-reference.less"";
";

			AssertLess(input, @"");
		}

		[Fact]
		public void ImportReferenceDoesNotOutputDirectives() {
			var input = @"
@import (reference) ""directives.less"";
";

			AssertLess(input, @"");
		}

		[Fact]
		public void ImportReferenceOutputsExtendedRulesFromMediaBlocks() {
			var input = @"
@import (reference) ""media-scoped-rules.less"";

.test:extend(.rule all) { }
";

			var expected = @"
@media (screen) {
  .test {
    background-color: black;
  }
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportReferenceDoesNotOutputMixinCalls() {
			var input = @"
@import (reference) ""reference/main.less"";
";

			AssertLess(input, @"");
		}

		[Fact]
		public void ExtendingReferencedImportOnlyOutputsExtendedSelector() {
			var input = @"
@import (reference) ""reference-with-multiple-selectors.less"";

.ext:extend(.test all) { }
";

			var expected = @"
.ext {
  color: black;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportReferenceDoesNotOutputComments() {
			var input = @"
@import (reference) ""comments.less"";
";

			AssertLess(input, @"");
		}

		[Fact]
		public void ImportReferenceWithMixinCallProducesOutput() {
			var input = @"
@import (reference) ""simple-rule.less"";

.caller {
  .rule
}
";

			var expected = @"
.caller {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportReferenceDoesNotPreventNonReferenceImport() {
			var input = @"
@import (reference) ""simple-rule.less"";
@import  ""simple-rule.less"";
";

			var expected = @"
.rule {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ExtendingReferenceImportsWorks() {
			var input = @"
@import (reference) ""simple-rule.less"";
.test:extend(.rule all) { }
";

			var expected = @"
.test {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportsFromReferenceImportsAreTreatedAsReferences() {
			var input = @"
@import (reference) ""imports-simple-rule.less"";

.test {
  .rule2;
}
";

			var expected = @"
.test {
  background-color: blue;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void RecursiveImportsFromReferenceImportsAreTreatedAsReferences() {
			var input = @"
@import (reference) ""two-level-import.less"";

.test {
  background-color: blue;
}
";

			var expected = @"
.test {
  background-color: blue;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportingReferenceAsLessWorks() {
			var input = @"
@import (reference, less) ""simple-rule.css"";
.test {
  .rule
}
";

			var expected = @"
.test {
  background-color: black;
}
";

			AssertLess(input, expected);
		}

//		[Fact]
//		public void ImportingReferenceAsCssFails()
//		{
//			var input = @"
//@import (reference, css) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (reference, css) -- specify either reference or css, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (reference, css) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void ImportingAsBothCssAndLessFails()
//		{
//			var input = @"
//@import (css, less) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (css, less) -- specify either css or less, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (css, less) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void ImportingAsBothInlineAndReferenceFails()
//		{
//			var input = @"
//@import (inline, reference) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (inline, reference) -- specify either inline or reference, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (inline, reference) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void ImportingAsBothInlineAndCssFails()
//		{
//			var input = @"
//@import (inline, css) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (inline, css) -- specify either inline or css, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (inline, css) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void ImportingAsBothInlineAndLessFails()
//		{
//			var input = @"
//@import (inline, less) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (inline, less) -- specify either inline or less, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (inline, less) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void ImportingAsBothOnceAndMultipleFails()
//		{
//			var input = @"
//@import (once, multiple) ""simple-rule.css"";
//";

//			var expectedError = @"
//invalid combination of @import options (once, multiple) -- specify either once or multiple, but not both on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (once, multiple) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}

//		[Fact]
//		public void UnrecognizedImportOptionFails()
//		{
//			var input = @"
//@import (invalid-option) ""simple-rule.css"";
//";

//			var expectedError = @"
//unrecognized @import option 'invalid-option' on line 1 in file 'test.less':
//   []: /beginning of file
//  [1]: @import (invalid-option) ""simple-rule.css"";
//       --------^
//  [2]: /end of file";
//			AssertError(expectedError, input);
//		}


		[Fact]
		public void ImportProtocolCssInsideMixinsWithNestedGuards() {
			var input = @"
.generateImports(@fontFamily) {
  & when (@fontFamily = Lato) {
    @import url(https://fonts.googleapis.com/css?family=Lato);
  }
  & when (@fontFamily = Cabin) {
    @import url(https://fonts.googleapis.com/css?family=Cabin);
  }
}
.generateImports(Lato);
.generateImports(Cabin);
";

			var expected = @"
@import url(https://fonts.googleapis.com/css?family=Lato);
@import url(https://fonts.googleapis.com/css?family=Cabin);
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportProtocolCssInsideMixinsWithGuards() {
			var input = @"
.generateImports(@fontFamily) when (@fontFamily = Lato) {
  @import url(https://fonts.googleapis.com/css?family=Lato);
}
.generateImports(@fontFamily) when (@fontFamily = Cabin) {
  @import url(https://fonts.googleapis.com/css?family=Cabin);
}
.generateImports(Lato);
.generateImports(Cabin);
";

			var expected = @"
@import url(https://fonts.googleapis.com/css?family=Lato);
@import url(https://fonts.googleapis.com/css?family=Cabin);
";

			AssertLess(input, expected);
		}

		[Fact]
		public void StrictMathIsHonoredInImports() {
			StrictMath = true;

			var input = @"
@import ""math.less"";
";

			var expected = @"
.rule {
  width: calc(10px + 2px);
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void ImportsWithinRulesets() {
			var input = @"
.test {
  @import ""math.less"";
}
";

			var expected = @"
.test .rule {
  width: calc(12px);
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void ImportsWithGeneratedSelectorsWithinRulesets() {
			var input = @"
.namespace {
  @import ""generated-selector.less"";
}
";

			var expected = @"
.namespace .rule {
  color: black;
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void NestedImportsWithinRulesets() {
			var input =
				@"
.namespace {
  @import url(""import/import-test-a.less"");

  #import-test {
    .mixin;
    width: 10px;
    height: @a + 10%;
  }
}
";

			var expected =
				@"
@import ""import-test-d.css"";
.namespace #import {
  color: red;
}
.namespace .mixin {
  height: 10px;
  color: red;
}
.namespace #import-test {
  height: 10px;
  color: red;
  width: 10px;
  height: 30%;
}
";


			AssertLess(input, expected);
		}

		[Fact]
		public void ImportsWithinRulesetsGenerateCallableMixins() {

			var input = @"
.namespace {
  @import ""reference/mixins/test.less"";
  .mixin(red);
}";

			var expected = @"
.namespace .test-ruleset {
  background-color: red;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ExtendedReferenceImportWithMultipleGeneratedSelectorsOnlyOutputsExtendedSelectors() {

			var input = @"
@import (reference) ""multiple-generated-selectors.less"";

.test:extend(.col-xs-12 all) { }
";

			var expected = @"
.test {
  float: left;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void RelativeImportsHonorCurrentDirectory() {
			var input = @"
@import ""import-test-a.less"";
";


			var expected =
				@"
@import ""import-test-d.css"";
#import {
  color: red;
}
.mixin {
  height: 10px;
  color: red;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void AbsolutePathImportsHonorsCurrentDirectory() {
			var input = @"
@import 'c:/absolute/file.less';";
			var expected = @"
.windowz .dos {
  border: none;
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void CssImportsAreHoistedToBeginningOfFile() {
			var input = @"
@font-face {
  font-family: ""Epsilon"";
  src: url('data:font/x-woff;base64,...')
}
@import url(//fonts.googleapis.com/css?family=PT+Sans+Narrow:400,700&subset=latin,cyrillic);
";


			var expected =
				@"
@import url(//fonts.googleapis.com/css?family=PT+Sans+Narrow:400,700&subset=latin,cyrillic);
@font-face {
  font-family: ""Epsilon"";
  src: url('data:font/x-woff;base64,...');
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void RelativeImportInMixinDefinition() {
			var input = @"
@import ""import-in-mixin/mixin-definition.less"";
.import();
";

			var expected = @"
.rule {
  color: black;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void ReferenceImportDoesNotOutputUnreferencedStyles() {
			var input = @"
@import (reference) ""reference-mixin-issue.less"";

.my-class
{
    .mix-me();
}";

			var expected = @"
.my-class {
  color: red;
}
@media (min-width: 100px) {
  .my-class {
    color: blue;
  }
}
.my-class .mix-me-child {
  background-color: black;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void MixinWithMediaBlock() {
			var input = @"
.mixin() {
  @media (min-width: 100px) {
    color: blue;
  }
}

.test {
  .mixin();
}
";

			var expected = @"
@media (min-width: 100px) {
  .test {
    color: blue;
  }
}";

			AssertLess(input, expected);
		}
	}
}