using Xunit;

namespace LessonNet.Tests.Specs
{
    

    public class MixinsArgsFixture : SpecFixtureBase
    {
        [Fact]
        public void MixinsArgsHidden1()
        {
            var input =
                @"
.hidden() {
  color: transparent;
}

#hidden {
  .hidden();
}
";

            var expected =
                @"
#hidden {
  color: transparent;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsHidden2()
        {
            var input =
                @"
.hidden() {
  color: transparent;
}

#hidden {
  .hidden;
}
";

            var expected =
                @"
#hidden {
  color: transparent;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsTwoArgs()
        {
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.mixina (@style, @width, @color: black) {
    border: @width @style @color;
}

.two-args {
  color: blue;
  .mixin(2px, 100%);
  .mixina(dotted, 2px);
}
";

            var expected =
                @"
.two-args {
  color: blue;
  width: 10px;
  height: 99%;
  border: 2px dotted black;
}
";

            AssertLess(input, expected);
        }

		[Fact]
        public void MixinCallWithSemiColonSeparator()
        {
            // MixinsArgsTwoArgs_with_semi_colon_separator tests the a MixinDefinition will support semicolon-separated arguments while this
            // test that a MixinCall will support them
            var input = @".size(@width, @height) {
  width: @width;
  height: @height;
}
.container {
  .size(100px; 50px);
}";

            var expected = @".container {
  width: 100px;
  height: 50px;
}";

            AssertLess(input, expected);
        }

        [Fact]
		public void MixinsArgsTwoArgs_with_semi_colon_separator()
		{
			var input =
				@"
.mixin (@a: 1px; @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.mixina (@style, @width, @color: black) {
    border: @width @style @color;
}

.two-args {
  color: blue;
  .mixin(2px, 100%);
  .mixina(dotted, 2px);
}
";

			var expected =
				@"
.two-args {
  color: blue;
  width: 10px;
  height: 99%;
  border: 2px dotted black;
}
";

			AssertLess(input, expected);
		}


//        [Fact]
//        public void MixinCallError()
//        {
//            var input =
//                @"
//.mixin (@a, @b) {
//  width: @a;
//  height: @b;
//}

//@c: 1px;

//.one-arg {
//  .mixin(@c);
//}
//";

//            AssertError(@"
//No matching definition was found for `.mixin(1px)` on line 9 in file 'test.less':
//  [8]: .one-arg {
//  [9]:   .mixin(@c);
//       --^
// [10]: }", input);
//        }

        [Fact]
        public void MixinsArgsOneArg()
        {
            // Todo: split into separate atomic tests.
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.one-arg {
  .mixin(3px);
}
";

            var expected =
                @"
.one-arg {
  width: 15px;
  height: 49%;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsNoParens()
        {
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}
.no-parens {
  .mixin;
}
";

            var expected =
                @"
.no-parens {
  width: 5px;
  height: 49%;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsNoArgs()
        {
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.no-args {
  .mixin();
}
";

            var expected =
                @"
.no-args {
  width: 5px;
  height: 49%;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsVariableArgs()
        {
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.var-args {
  @var: 9;
  .mixin(@var, @var * 2);
}
";

            var expected =
                @"
.var-args {
  width: 45;
  height: 17%;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsMulti()
        {
            var input =
                @"
.mixin (@a: 1px, @b: 50%) {
  width: @a * 5;
  height: @b - 1%;
}

.mixiny
(@a: 0, @b: 0) {
  margin: @a;
  padding: @b;
}

.multi-mix {
  .mixin(2px, 30%);
  .mixiny(4, 5);
}
";

            var expected =
                @"
.multi-mix {
  width: 10px;
  height: 29%;
  margin: 4;
  padding: 5;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgs()
        {
            var input =
                @"
.maxa(@arg1: 10, @arg2: #f00) {
  padding: @arg1 * 2px;
  color: @arg2;
}

body {
  .maxa(15);
}
";

            var expected =
                @"
body {
  padding: 30px;
  color: #f00;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsScopeMix()
        {
            var input =
                @"
@glob: 5;
.global-mixin(@a:2) {
  width: @glob + @a;
}

.scope-mix {
  .global-mixin(3);
}
";

            var expected =
                @"
.scope-mix {
  width: 8;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsArgsNestedRuleset()
        {
            var input =
                @"
.nested-ruleset (@width: 200px) {
    width: @width;
    .column { margin: @width; }
}
.content {
    .nested-ruleset(600px);
}
";

            var expected =
                @"
.content {
  width: 600px;
}
.content .column {
  margin: 600px;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void CanEvaluateCommaSepearatedExpressions()
        {
            var input =
                @"
.fonts (@main: 'Arial') {
    font-family: @main, sans-serif;
}

.class {
    .fonts('Helvetica');
}";

            var expected = @"
.class {
  font-family: 'Helvetica', sans-serif;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void CanUseVariablesInsideMixins()
        {
            var input = @"
@var: 5px;
.mixin() {
    width: @var, @var, @var;
}
.div {
    .mixin;
}";

            var expected = @"
.div {
  width: 5px, 5px, 5px;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void CanUseSameVariableName()
        {
            var input =
                @"
.same-var-name2(@radius) {
  radius: @radius;
}
.same-var-name(@radius) {
  .same-var-name2(@radius);
}
#same-var-name {
  .same-var-name(5px);
}
";

            var expected = @"
#same-var-name {
  radius: 5px;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void MixinArgsHashMixin1()
        {
            var input = @"
#id-mixin () {
    color: red;
}
.id-class {
    #id-mixin();
}
";

            var expected = @"
.id-class {
  color: red;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void MixinArgsHashMixin2()
        {
            var input = @"
#id-mixin () {
    color: red;
}
.id-class {
    #id-mixin;
}
";

            var expected = @"
.id-class {
  color: red;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void MixinArgsExceptCurlyBracketString()
        {
            var input =
                @"
.mixin-arguments (@width: 0px) {
    border: @arguments;
}
.arguments2 {
    .mixin-arguments(""{"");
}";

            var expected = @"
.arguments2 {
  border: ""{"";
}";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs1()
        {
            var input = @"
.mixin-arguments (@width: 0px, ...) {
    border: @arguments;
    width: @width;
}

.arguments {
    .mixin-arguments(1px, solid, black);
}
.arguments2 {
    .mixin-arguments();
}
.arguments3 {
    .mixin-arguments;
}";

            var expected = @"
.arguments {
  border: 1px solid black;
  width: 1px;
}
.arguments2 {
  border: 0px;
  width: 0px;
}
.arguments3 {
  border: 0px;
  width: 0px;
}";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs2()
        {
            var input = @"
.mixin-arguments2 (@width, @rest...) {
    border: @arguments;
    rest: @rest;
    width: @width;
}
.arguments4 {
    .mixin-arguments2(0, 1, 2, 3, 4);
}";

            var expected = @"
.arguments4 {
  border: 0 1 2 3 4;
  rest: 1 2 3 4;
  width: 0;
}";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs3a()
        {
            var input = @"
.mixin (...) {
  variadic: true;
}
.mixin () {
    zero: 0;
}
.mixin (@a: 1px) {
    one: 1;
}
.mixin (@a) {
    one-req: 1;
}
.mixin (@a: 1px, @b: 2px) {
    two: 2;
}

.mixin (@a, @b, @c) {
    three-req: 3;
}

.mixin (@a: 1px, @b: 2px, @c: 3px) {
    three: 3;
}

.zero {
    .mixin();
}
";

            var expected = @"
.zero {
  variadic: true;
  zero: 0;
  one: 1;
  two: 2;
  three: 3;
}";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs3b()
        {
            var input = @"
.mixin (...) {
  variadic: true;
}
.mixin () {
    zero: 0;
}
.mixin (@a: 1px) {
    one: 1;
}
.mixin (@a) {
    one-req: 1;
}
.mixin (@a: 1px, @b: 2px) {
    two: 2;
}

.mixin (@a, @b, @c) {
    three-req: 3;
}

.mixin (@a: 1px, @b: 2px, @c: 3px) {
    three: 3;
}

.one {
    .mixin(1);
}
";

            var expected = @"
.one {
  variadic: true;
  one: 1;
  one-req: 1;
  two: 2;
  three: 3;
}";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs3c()
        {
            var input = @"
.mixin (...) {
  variadic: true;
}
.mixin () {
    zero: 0;
}
.mixin (@a: 1px) {
    one: 1;
}
.mixin (@a) {
    one-req: 1;
}
.mixin (@a: 1px, @b: 2px) {
    two: 2;
}

.mixin (@a, @b, @c) {
    three-req: 3;
}

.mixin (@a: 1px, @b: 2px, @c: 3px) {
    three: 3;
}

.two {
    .mixin(1, 2);
}";

            var expected = @"
.two {
  variadic: true;
  two: 2;
  three: 3;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void VariadicArgs3d()
        {
            var input = @"
.mixin (...) {
  variadic: true;
}
.mixin () {
    zero: 0;
}
.mixin (@a: 1px) {
    one: 1;
}
.mixin (@a) {
    one-req: 1;
}
.mixin (@a: 1px, @b: 2px) {
    two: 2;
}

.mixin (@a, @b, @c) {
    three-req: 3;
}

.mixin (@a: 1px, @b: 2px, @c: 3px) {
    three: 3;
}

.three {
    .mixin(1, 2, 3);
}";

            var expected = @"
.three {
  variadic: true;
  three-req: 3;
  three: 3;
}";
            AssertLess(input, expected);
        }

        #region GitHub issue 408
        /// <summary>
        /// This is the failure case reported at https://github.com/dotless/dotless/issues/408
        /// </summary>
        [Fact]
        public void ExpressionArguments() {
            var input = @"@tilebasewidth : 130px;
@tilemargin: 10px;

.test(@param1, @param2) {
 width: @param1;
 height: @param2;
}

.two-c {
    .test(@tilebasewidth, (@tilebasewidth + @tilemargin)*2);
}";


            var expected = @".two-c {
  width: 130px;
  height: 280px;
}";

            AssertLess(input, expected);
        }

        /// <summary>
        /// This case actually worked before, too. Included here to ensure that fixing one case doesn't break another.
        /// </summary>
        [Fact]
        public void ExpressionArguments2() {
            var input = @"@tilebasewidth : 130px;
@tilemargin: 10px;

.test(@param1, @param2) {
 width: @param1;
 height: @param2;
}

.two-c {
    .test((@tilebasewidth + @tilemargin) * 2, @tilebasewidth);
}";


            var expected = @".two-c {
  width: 280px;
  height: 130px;
}";

            AssertLess(input, expected);
        }

        /// <summary>
        /// This case actually worked before, too. Included here to ensure that fixing one case doesn't break another.
        /// </summary>
        [Fact]
        public void ExpressionArguments3() {
            var input = @"@tilebasewidth : 130px;
@tilemargin: 10px;

.test(@param1, @param2) {
 width: @param1;
 height: @param2;
}

.two-c {
    .test(5px, (@tilebasewidth + @tilemargin) * 2);
}";


            var expected = @".two-c {
  width: 5px;
  height: 280px;
}";

            AssertLess(input, expected);
        }
        #endregion

        [Fact]
        public void NamedArguments() {
            var input = @"
#gradient {
  .horizontal(@start-color, @end-color) {
    color: @start-color;
    background-color: @end-color;
  }
}

.test {
  #gradient > .horizontal(@start-color: rgba(0,0,0,.5); @end-color: rgba(0,0,0,.0001));
}
";

            var expected = @"
.test {
  color: rgba(0, 0, 0, 0.5);
  background-color: rgba(0, 0, 0, 0.0001);
}";

            AssertLess(input, expected);
        }
    }
}