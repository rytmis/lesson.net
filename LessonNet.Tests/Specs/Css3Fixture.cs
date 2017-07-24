using System.Collections.Generic;
using Xunit;

namespace LessonNet.Tests.Specs
{


	public class Css3Fixture : SpecFixtureBase
	{
		[Fact]
		public void CommaDelimited()
		{
			var input =
				@"
.comma-delimited {
  background: url(bg.jpg) no-repeat, url(bg.png) repeat-x top left, url(bg);
  text-shadow: -1px -1px 1px red, 6px 5px 5px yellow;
  -moz-box-shadow: 0pt 0pt 2px rgba(255, 255, 255, 0.4) inset, 0pt 4px 6px rgba(255, 255, 255, 0.4) inset;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void FontFaceDirective()
		{
			var input =
				@"
@font-face {
  font-family: Headline;
  src: local(Futura-Medium), url(fonts.svg#MyGeometricModern) format(""svg"");
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void FontFaceDirectiveInClass()
		{
			var input = @"
#font {
  .test(){
    @font-face {
      font-family: 'MyFont';
      src: url('/css/fonts/myfont.eot');
      src: url('/css/fonts/myfont.eot?#iefix') format('embedded-opentype'),
           url('/css/fonts/myfont.woff') format('woff'),
           url('/css/fonts/myfont.ttf') format('truetype'),
           url('/css/fonts/myfont.svg#reg') format('svg');
    }
    .cl {
      background: red;
    }
  }
}

#font > .test();";

			var expected = @"
@font-face {
  font-family: 'MyFont';
  src: url('/css/fonts/myfont.eot');
  src: url('/css/fonts/myfont.eot?#iefix') format('embedded-opentype'), url('/css/fonts/myfont.woff') format('woff'), url('/css/fonts/myfont.ttf') format('truetype'), url('/css/fonts/myfont.svg#reg') format('svg');
}
.cl {
  background: red;
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void SupportMozDocument()
		{
			// see https://github.com/dotless/dotless/issues/73
			var input =
				@"
@-moz-document url-prefix(""https://github.com"") {
  h1 {
    color: red;
  }
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void SupportVendorXDocument()
		{
			var input =
				@"
@-x-document url-prefix(""github.com"") {
  h1 {
    color: red;
  }
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void SupportSupports()
		{
			// example from http://www.w3.org/TR/2011/WD-css3-conditional-20110901/
			var input =
				@"
@supports ( box-shadow: 2px 2px 2px black ) or
          ( -moz-box-shadow: 2px 2px 2px black ) or
          ( -webkit-box-shadow: 2px 2px 2px black ) or
          ( -o-box-shadow: 2px 2px 2px black ) {
  .outline {
    color: white;
    box-shadow: 2px 2px 2px black;
    -moz-box-shadow: 2px 2px 2px black;
    -webkit-box-shadow: 2px 2px 2px black;
    -o-box-shadow: 2px 2px 2px black;
  }
}";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void NamespaceDirective()
		{
			// see https://github.com/dotless/dotless/issues/27
			var input =
				@"
@namespace lq ""http://example.com/q-markup"";";

			AssertLessUnchanged(input);
		}


		[Fact]
		public void KeyFrameDirectiveWebKit()
		{
			var input = @"
@-webkit-keyframes fontbulger {
  0% {
    font-size: 10px;
  }
  30.5% {
    font-size: 15px;
  }
  100% {
    font-size: 12px;
  }
}
#box {
  -webkit-animation: fontbulger 2s infinite;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirectiveMoz()
		{
			var input = @"
@-moz-keyframes fontbulger {
  0% {
    font-size: 10px;
  }
  30% {
    font-size: 15px;
  }
  100% {
    font-size: 12px;
  }
}
#box {
  -moz-animation: fontbulger 2s infinite;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirective()
		{
			var input = @"
@keyframes fontbulger {
  0% {
    font-size: 10px;
  }
  30% {
    font-size: 15px;
  }
  100% {
    font-size: 12px;
  }
}
#box {
  animation: fontbulger 2s infinite;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirective2()
		{
			// would not happen but tests syntax
			var input = @"
@keyframes fontbulger1 {
  from {
    font-size: 10px;
  }
  to {
    font-size: 15px;
  }
  from, to {
    font-size: 12px;
  }
  0%, 100% {
    font-size: 12px;
  }
}
#box {
  animation: fontbulger1 2s infinite;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirective3()
		{
			var input = @"
@-webkit-keyframes rotate-this {
  0% {
    -webkit-transform: scale(0.6) rotate(0deg);
  }
  50.01% {
    -webkit-transform: scale(0.6) rotate(180deg);
  }
  100% {
    -webkit-transform: scale(0.6) rotate(315deg);
  }
}
#box {
  animation: rotate-this 2s infinite;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirective4()
		{
			var input = @"
@keyframes rotate-this {
  0%, 1%, 10%, 80%, to {
    -webkit-transform: scale(0.6) rotate(0deg);
  }
  50% {
    -webkit-transform: scale(0.6) rotate(180deg);
  }
}
";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void KeyFrameDirective5()
		{
			var input = @"
@keyframes rotate-this {
  0%, 1%, 10%, 80%, to {
  }
  50% {
  }
}
";

			var expected = @"
@keyframes rotate-this {
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void MozTransform()
		{
			var input = @"
.other {
  -moz-transform: translate(0, 11em) rotate(-90deg);
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void NotPseudoClass()
		{
			var input = @"
p:not([class*=""lead""]) {
  color: black;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void MultipleAttributeSelectors1()
		{
			var input = @"
input[type=""text""].class#id[attr=32]:not(1) {
  color: white;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void AttributeSelectorsInsideMixin()
		{
			// See github.com/dotless/issues/65
			var input = @"
.Grid {
    input[type=""checkbox""] {
        margin-right: 4px;
    }
}
";
			var expected = @"
.Grid input[type=""checkbox""] {
  margin-right: 4px;
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void MultipleAttributeSelectors2()
		{
			var input = @"
div#id.class[a=1][b=2].class:not(1) {
  color: white;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void BeforePseudoElement()
		{
			var input = @"
p::before {
  color: black;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void AfterPseudoElement()
		{
			var input =
				@"
ul.comma > li:not(:only-child)::after {
  color: white;
}
ol.comma > li:nth-last-child(2)::after {
  color: white;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void NthChildExpressions()
		{
			var input = @"
li:nth-child(4n+1),
li:nth-child(-5n),
li:nth-child(-n+2) {
  color: white;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void AttributeBeginsWith()
		{
			var input = @"
a[href^=""http://""] {
  color: black;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void AttributeEndsWith()
		{
			var input = @"
a[href$=""http://""] {
  color: black;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void SiblingSelector()
		{
			var input = @"
a ~ p {
  background: red;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void MultipleBackgroundProperty()
		{
			var input =
				@"
.class {
  background: url(bg1.gif) top left no-repeat, url(bg2.jpg) top right no-repeat;
}
";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void Css3UnitsSupported()
		{
			// see http://dev.w3.org/csswg/css3-values/

			List<string> units = new List<string>() { "em", "ex", "ch", "rem", "vw", "vh", "vmin", "vm", "vmax", "cm", "mm", "%", "in", "pt", "px", "pc", "deg", "grad", "rad", "s", "ms", "fr", "gr", "Hz", "kHz", "dpcm", "dppx" };

			foreach (string unit in units)
			{
				AssertExpression("1" + unit, "2" + unit + " / 2");
			}
		}

		[Fact]
		public void FontFaceMixin()
		{
			var input = @"
.def-font(@name) {
    @font-face {
        font-family: @name
    }
}

.def-font(font-a);
.def-font(font-b);";

			var expected = @"
@font-face {
  font-family: font-a;
}
@font-face {
  font-family: font-b;
}
";
			AssertLess(input, expected);
		}

		[Fact]
		public void NestedPseudoclassSelectorsWork()
		{
			var input = @"
:not(:nth-child(1)) {
  margin-top: 5px;
}";

			AssertLessUnchanged(input);
		}

		[Fact]
		public void PseudoclassParenMatchingWorks()
		{
			var input = @"
audio:not([controls]) {
  // this comment has parens ()
  margin-top: 5px;
}";

			var expected = @"
audio:not([controls]) {
  margin-top: 5px;
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void Css3FilterFunctionsPassThrough()
		{
			AssertRuleUnchanged("filter: url(resources.svg#c1)");
			AssertRuleUnchanged("filter: blur(5px)");
			AssertRuleUnchanged("filter: brightness(0.5)");
			AssertRuleUnchanged("filter: contrast(200%)");
			AssertRuleUnchanged("filter: drop-shadow(16px 16px 10px black)");
			AssertRuleUnchanged("filter: grayscale(100%)");
			AssertRuleUnchanged("filter: hue-rotate(90deg)");
			AssertRuleUnchanged("filter: invert(100%)");
			AssertRuleUnchanged("filter: opacity(50%)");
			AssertRuleUnchanged("filter: saturate(200%)");
			AssertRuleUnchanged("filter: sepia(100%)");
		}

		[Fact]
		public void VariablesInCss3Filters()
		{

			var input = @"
@brand-primary: #800080;
.hover-element.select {
  filter: drop-shadow( -15px -15px 15px @brand-primary );
}";

			var expected = @"
.hover-element.select {
  filter: drop-shadow(-15px -15px 15px #800080);
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void Css3FilterVendorPrefixes()
		{
			AssertRuleUnchanged("-webkit-filter: saturate(200%)");
			AssertRuleUnchanged("-moz-filter: saturate(200%)");
		}

		[Fact]
		public void Css3FilterMultipleFunctions()
		{
			AssertRuleUnchanged("filter: invert(100%) opacity(50%) saturate(200%)");
			AssertRule("filter: /* test */ invert(100%) /* test */ opacity(50%) /* test */ saturate(200%);", "filter: /* test */invert(100%)/* test */ opacity(50%)/* test */ saturate(200%)");
		}

		[Fact]
		public void Css3FilterWithEvaluatedValues()
		{
			AssertRule("filter: blur(10px + 5)", "filter: blur(15px)");
		}

		[Fact]
		public void FontFaceWithRangedUnicodeRangeValue()
		{
			var input = @"
@font-face {
  font-family: 'Open Sans';
  font-style: normal;
  font-weight: 400;
  src: url('OpenSans400.eot');
  unicode-range: U+0460-052F;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void FontFaceWithSingleUnicodeRangeValue()
		{
			var input = @"
@font-face {
  font-family: 'Open Sans';
  font-style: normal;
  font-weight: 400;
  src: url('OpenSans400.eot');
  unicode-range: U+20B4;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void FontFaceWithWildcardUnicodeRangeValue()
		{
			var input = @"
@font-face {
  font-family: 'Open Sans';
  font-style: normal;
  font-weight: 400;
  src: url('OpenSans400.eot');
  unicode-range: U+20??;
}";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void AnimationNameWithUnderscore()
		{
			var input = @"
.test {
  animation-name: bounce_loadingProgress;
}";
			AssertLessUnchanged(input);
		}

#if CSS3EXPERIMENTAL
        [Fact]
        public void GridRepeatingPatternSupported()
        {
            //see http://www.w3.org/TR/css3-grid/#example0

            AssertExpressionUnchanged("0 1em (0.5in 5rem 0)[2]");
            AssertExpressionUnchanged("(500px)[2]");
        }

        [Fact]
        public void GridRepeatingPatternSupportedWithVars()
        {
            var input = @"
@a : 1em;
@b : 2px;
@c : red;
@d : 10;
.test {
  background: 0 @a (@b 0 @c)[@d];
}
";
            var expected = @"
.test {
  background: 0 1em (2px 0 red)[10];
}
";
            AssertLess(input, expected);
        }
#endif
	}
}
