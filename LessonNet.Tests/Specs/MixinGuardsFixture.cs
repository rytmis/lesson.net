using Xunit;

namespace LessonNet.Tests.Specs
{
    

    public class MixinGuardsFixture : SpecFixtureBase
    {
        [Fact]
        public void StackingFunctions()
        {
            var input =
                @"
.light (@a) when (lightness(@a) > 50%) {
  color: white;
}
.light (@a) when (lightness(@a) < 50%) {
  color: black;
}
.light (@a) {
  margin: 1px;
}

.light1 { .light(#ddd) }
.light2 { .light(#444) }
";

            var expected =
                @"
.light1 {
  color: white;
  margin: 1px;
}
.light2 {
  color: black;
  margin: 1px;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ArgumentsAgainstEachOther()
        {
            var input =
                @"
.max (@a, @b) when (@a > @b) {
  width: @a;
}
.max (@a, @b) when (@a < @b) {
  width: @b;
}

.max1 { .max(3, 6) }
.max2 { .max(8, 1) }
";

            var expected =
                @"
.max1 {
  width: 6;
}
.max2 {
  width: 8;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void GlobalsInsideGuardsPositive()
        {
            var input =
                @"
@g: auto;

.glob (@a) when (@a = @g) {
  margin: @a @g;
}
.glob1 { .glob(auto) }
";

            var expected =
                @"
.glob1 {
  margin: auto auto;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void GlobalsInsideGuardsNegative()
        {
            var input =
                @"
@g: auto;

.glob (@a) when (@a = @g) {
  margin: @a @g;
}

.glob2 { .glob(default) }";

            var expected = @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void OtherOperators()
        {
            var input =
                @"
.ops (@a) when (@a >= 0) {
  height: gt-or-eq;
}
.ops (@a) when (@a =< 0) {
  height: lt-or-eq;
}
.ops (@a) when not(@a = 0) {
  height: not-eq;
}
.ops1 { .ops(0) }
.ops2 { .ops(1) }
.ops3 { .ops(-1) }";

            var expected =
                @"
.ops1 {
  height: gt-or-eq;
  height: lt-or-eq;
}
.ops2 {
  height: gt-or-eq;
  height: not-eq;
}
.ops3 {
  height: lt-or-eq;
  height: not-eq;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ScopeAndDefaultValuesPositive()
        {
            var input =
                @"
@a: auto;

.default (@a: inherit) when (@a = inherit) {
  content: default;
}
.default1 { .default }
";

            var expected =
                @"
.default1 {
  content: default;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void ScopeAndDefaultValuesNegative()
        {
            var input =
                @"
@a: auto;

.default (@a: inherit) when (@a = inherit) {
  content: default;
}
.default2 { .default(@a) }
";

            var expected = @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void TrueAndFalse()
        {
            var input = @"
.test (@a) when (@a) {
    content: ""true."";
}
.test (@a) when not (@a) {
    content: ""false."";
}

.test1 { .test(true) }
.test2 { .test(false) }
.test3 { .test(1) }
.test4 { .test(boo) }
.test5 { .test(""true"") }
";
            var expected = @"
.test1 {
  content: ""true."";
}
.test2 {
  content: ""false."";
}
.test3 {
  content: ""false."";
}
.test4 {
  content: ""false."";
}
.test5 {
  content: ""false."";
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression1()
        {
            var input =
                @"
.bool () when (true) and (false)                             { content: true and false } // FALSE
.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression2()
        {
            var input =
                @"
.bool () when (true) and (true)                              { content: true and true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: true and true;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression3()
        {
            var input =
                @"
.bool () when (true)                                         { content: true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: true;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression4()
        {
            var input =
                @"
.bool () when (false) and (false)                            { content: true } // FALSE

.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression5()
        {
            var input =
                @"
.bool () when (false), (true)                                { content: false, true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: false, true;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression6()
        {
            var input =
                @"
.bool () when (false) and (true) and (true),  (true)         { content: false and true and true, true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: false and true and true, true;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression7()
        {
            var input =
                @"
.bool () when (true)  and (true) and (false), (false)        { content: true and true and false, false } // FALSE

.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression8()
        {
            var input =
                @"
.bool () when (false), (true) and (true)                     { content: false, true and true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: false, true and true;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression9()
        {
            var input =
                @"
.bool () when (false), (false), (true)                       { content: false, false, true } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: false, false, true;
}";

            AssertLess(input, expected);
        }


        [Fact]
        public void BooleanExpression10()
        {
            var input =
                @"
.bool () when (false), (false) and (true), (false)           { content: false, false and true, false } // FALSE

.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression11()
        {
            var input =
                @"
.bool () when (false), (true) and (true) and (true), (false) { content: false, true and true and true, false } // TRUE

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: false, true and true and true, false;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression12()
        {
            var input =
                @"
.bool () when not (false)                                    { content: not false }

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: not false;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression13()
        {
            var input =
                @"
.bool () when not (true) and not (false)                     { content: not true and not false }

.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression14()
        {
            var input =
                @"
.bool () when not (true) and not (true)                      { content: not true and not true }

.bool1 { .bool }";

            var expected =
                @"";

            AssertLess(input, expected);
        }

        [Fact]
        public void BooleanExpression15()
        {
            var input =
                @"
.bool () when not (false) and (false), not (false)           { content: not false and false, not false }

.bool1 { .bool }";

            var expected =
                @"
.bool1 {
  content: not false and false, not false;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ComparisonAgainstEqualIgnoresUnits()
        {
            var input =
                @"
.light (@a) when (@a > 0) {
  color: big @a;
}
.light (@a) when (@a < 0) {
  color: small @a;
}

.light1 { 
  .light(1px);
  .light(0px); 
  .light(-1px); 
}
";

            var expected =
                @"
.light1 {
  color: big 1px;
  color: small -1px;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void ComparisonAgainstFunctionCall()
        {
            var input =
                @"
.light (@a) when (alpha(@a) > 0.5) {
  color: solid @a;
}
.light (@a) when (0.5 > alpha(@a)) {
  color: trans @a;
}

.light1 { 
  .light(red);
  .light(transparent); 
}
";

            var expected =
                @"
.light1 {
  color: solid red;
  color: trans transparent;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void ColorCompare()
        {
            var input =
                @"
.light (@a) when (@a = transparent) {
  color: red;
}
.light (@a) when (@a < grey) {
  color: white;
}
.light (@a) when (@a = grey) {
  color: grey;
}
.light (@a) when (@a > grey) {
  color: black;
}

.light1 { .light(black) }
.light2 { .light(#eee) }
.light3 { .light(grey) }
";

            var expected =
                @"
.light1 {
  color: black;
}
.light2 {
  color: white;
}
.light3 {
  color: grey;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void ColorCompareAlpha()
        {
            var input =
                @"
.light (@a) when (@a = transparent) {
  color: red;
}
.light (@a) when (@a < grey) {
  color: white;
}
.light (@a) when (@a = grey) {
  color: grey;
}
.light (@a) when (@a > grey) {
  color: black;
}

.light1 { .light(transparent) }
.light2 { .light(fadein(black, -10)) }
.light3 { .light(fadein(black, -90)) }

";

            var expected =
                @"
.light1 {
  color: red;
  color: white;
}
.light2 {
  color: black;
}
.light3 {
  color: white;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void StringCompare()
        {
            var input =
                @"
.light (@a) when (@a = ""test1"") {
  color: red;
}
.light (@a) when not (@a = ""test2"") {
  color: white;
}
.light (@a) when (""test1"" = @a) {
  color: grey;
}
.light (@a) when not (""test2"" = @a) {
  color: black;
}
.light (@a) when (~""test1"" = @a) {
  color: blue;
}

.light1 { .light(""test1"") }
.light2 { .light(""test2"") }
";

            var expected =
                @"
.light1 {
  color: red;
  color: white;
  color: grey;
  color: black;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void KeywordCompare()
        {
            var input =
                @"
.light (@a) when (@a = block) {
  color: red;
}
.light (@a) when not (@a = inline) {
  color: white;
}
.light (@a) when (~""inline"" = @a) {
  color: blue;
}
.light (@a) when (@a = ~""inline"") {
  color: pink;
}

.light1 { .light(inline) }
.light2 { .light(block) }
";

            var expected =
                @"
.light1 {
  color: blue;
  color: pink;
}
.light2 {
  color: red;
  color: white;
}
";

            AssertLess(input, expected);
        }

        [Fact]
        public void TestCompareArgsInGuard()
        {
            var input = @"
.mixin(@c: bar, @a: red, @b: blue) when (@a = @b) {
  foo: @c;
}
.test {
  .mixin(@b:red, @a:red);
}";
            var expected = @"
.test {
  foo: bar;
}
";
            AssertLess(input, expected);
        }

        [Fact]
        public void GuardsVariableBug1()
        {
            var input = @"
.backgroundColorFn (@a) when (lightness(@a) >= 50%) {
    color:          black;
}
.backgroundColorFn (@a) when (lightness(@a) < 50%) {
    color:          white;
}

.test1 {
    .backgroundColorFn(red);
}

@calculatedVariable: 1em;

.testClass { font-size: @calculatedVariable; }";
            var expected = @"
.test1 {
  color: black;
}
.testClass {
  font-size: 1em;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void MixinsUsingNestedGuards()
        {
            var input =
                @"
.light (@a) {
  & when (lightness(@a) > 50%) {
    color: white;
  }
  & when (lightness(@a) < 50%) {
    color: black;
  }
  margin: 1px;
}

.light1 { .light(#ddd) }
.light2 { .light(#444) }
";

            var expected =
                @"
.light1 {
  margin: 1px;
}
.light1 {
  color: white;
}
.light2 {
  margin: 1px;
}
.light2 {
  color: black;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void DefaultFunctionInGuard() {
            var input = @"
.mix(@i) when (@i > 0) {
  color: blue;
}

.mix(@i) when (default()) {
  color: black;
}

.test {
  .mix(0);
}
";

            var expected = @"
.test {
  color: black;
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void DefaultFunctionMatchesIfNothingElseDoes() {
            var input = @"
// ............................................................
// .for

.for(@i, @n) {.-each(@i)}
.for(@n)     when (isnumber(@n)) {.for(1, @n)}
.for(@i, @n) when not (@i = @n)  {
    .for((@i + (@n - @i) / abs(@n - @i)), @n);
}

// ............................................................
// .for-each

.for(@array)   when (default()) {.for-impl_(length(@array))}
.for-impl_(@i) when (@i > 1)    {.for-impl_((@i - 1))}
.for-impl_(@i) when (@i > 0)    {.-each(extract(@array, @i))}

#icon {
  .for(home ok); .-each(@name) {
    &-@{name} {
      background-image: url(""../images/@{name}.png"");
    }
  }
}";

            var expected = @"
#icon-home {
  background-image: url(""../images/home.png"");
}
#icon-ok {
  background-image: url(""../images/ok.png"");
}";

            AssertLess(input, expected);
        }

        [Fact]
        public void DefaultFunctionDoesNotMatchIfSomethingElseDoes() {
            var input = @"
// ............................................................
// .for

.for(@i, @n) {.-each(@i)}
.for(@n)     when (isnumber(@n)) {.for(1, @n)}
.for(@i, @n) when not (@i = @n)  {
    .for((@i + (@n - @i) / abs(@n - @i)), @n);
}

// ............................................................
// .for-each

.for(@array)   when (default()) {.for-impl_(length(@array))}
.for-impl_(@i) when (@i > 1)    {.for-impl_((@i - 1))}
.for-impl_(@i) when (@i > 0)    {.-each(extract(@array, @i))}

.for(3); .-each(@i) {
  .xxx {
    color: red;
  }
}
";

            var expected = @"
.xxx {
  color: red;
}
.xxx {
  color: red;
}
.xxx {
  color: red;
}";

            AssertLess(input, expected);
        }
    }
}