using Xunit;

namespace LessonNet.Tests.Specs {


	public class StringsFixture : SpecFixtureBase {
		[Fact]
		public void Strings() {
			AssertExpressionUnchanged(@"""http://son-of-a-banana.com""");
			AssertExpressionUnchanged(@"""~"" ""~""");
			AssertExpressionUnchanged(@"""#*%:&^,)!.(~*})""");
			AssertExpressionUnchanged(@"""""");
		}

		[Fact]
		public void Comments() {
			AssertExpressionUnchanged(@"""/* hello */ // not-so-secret""");
		}

		[Fact]
		public void SingleQuotes() {
			AssertExpressionUnchanged(@"""'"" ""'""");
			AssertExpressionUnchanged(@"'""""#!&""""'");
			AssertExpressionUnchanged(@"''");
		}

		[Fact]
		public void EscapingQuotes() {
			AssertExpressionUnchanged(@"""\""""");
			AssertExpressionUnchanged(@"'\''");
			AssertExpressionUnchanged(@"'\'\""'");
		}

		[Fact]
		public void EscapingQuotesUnescaped() {
			AssertExpression(@"""", @"~""\""""");
			AssertExpression(@"'", @"~""\'""");
		}

		[Fact]
		public void EscapedWithTilde() {
			AssertExpression(@"DX.Transform.MS.BS.filter(opacity=50)", @"~""DX.Transform.MS.BS.filter(opacity=50)""");
		}

		[Fact]
		public void StringInterpolation1() {
			var input = @"
#interpolation {
  @var: '/dev';
  url: ""http://lesscss.org@{var}/image.jpg"";
}
";
			var expected = @"
#interpolation {
  url: ""http://lesscss.org/dev/image.jpg"";
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolation2() {
			var input = @"
#interpolation {
  @var2: 256;
  url2: ""http://lesscss.org/image-@{var2}.jpg"";
}
";
			var expected = @"
#interpolation {
  url2: ""http://lesscss.org/image-256.jpg"";
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolation3() {
			var input = @"
#interpolation {
  @var3: #456;
  url3: ""http://lesscss.org@{var3}"";
}
";
			var expected = @"
#interpolation {
  url3: ""http://lesscss.org#456"";
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolation4() {
			var input = @"
#interpolation {
  @var4: hello;
  url4: ""http://lesscss.org/@{var4}"";
}
";
			var expected = @"
#interpolation {
  url4: ""http://lesscss.org/hello"";
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolation5() {
			var input = @"
#interpolation {
  @var5: 54.4px;
  url5: ""http://lesscss.org/@{var5}"";
}
";
			var expected = @"
#interpolation {
  url5: ""http://lesscss.org/54.4px"";
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolationMultipleCalls() {
			var input = @"
.mix-mul (@a: green) {
    color: ~""@{a}"";
}
.mix-mul-class {
    .mix-mul(blue);
    .mix-mul(red);
    .mix-mul(orange);
}";
			var expected = @"
.mix-mul-class {
  color: blue;
  color: red;
  color: orange;
}";
			AssertLess(input, expected);
		}

		[Fact]
		public void StringInterpolationInUrl() {
			var input = @"
#interpolation {
  @var: '/dev';
  url: url(""http://lesscss.org@{var}/image.jpg"");
}
";
			var expected = @"
#interpolation {
  url: url(""http://lesscss.org/dev/image.jpg"");
}";

			AssertLess(input, expected);
		}

		[Fact]
		public void QuotedExpressionInterpolationInUrl() {
			var input = @"
@pathsep: '/';
#second {
  background: url(~""@{pathsep}sub2/image2.gif"");
}
";

			var expected = @"
#second {
  background: url(/sub2/image2.gif);
}
";

			AssertLess(input, expected);
		}

		[Fact]
		public void RawUrlWithTilde() {
			AssertExpressionUnchanged(@"url(~test/foo.gif)");
		}

		[Fact]
		public void BracesInQuotes() {
			AssertExpressionUnchanged(@"""{"" ""}""");
		}

		[Fact]
		public void BracesInQuotesUneven() {
			AssertExpressionUnchanged(@"""{"" """"");
		}

		[Fact]
		public void SemiColonInQuotes() {
			AssertExpressionUnchanged(@"';'");
		}

		[Fact]
		public void CloseBraceInsideStringAfterQuoteInsideString() {
			var input = @"
#test {
  prop: ""'test'"";
  prop: ""}"";
}
";
			AssertLessUnchanged(input);
		}

		[Fact]
		public void NoEndDoubleQuote() {
			var input = @"
.cla { background-image: ""my-image.jpg; }

.clb { background-image: ""my-second-image.jpg""; }";

			AssertError(
				"Missing closing quote (\")",
				".cla { background-image: \"my-image.jpg; }",
				1,
				25,
				input);
		}

		[Fact]
		public void NoEndDoubleQuote2() {
			var input =
				@"
.cla { background-image: ""my-image.jpg; }

.clb { background-position: 12px 3px; }";

			AssertError(
				"Missing closing quote (\")",
				".cla { background-image: \"my-image.jpg; }",
				1,
				25,
				input);
		}

		[Fact]
		public void NoEndSingleQuote() {
			var input = @"
.cla { background-image: 'my-image.jpg; }

.clb { background-image: 'my-second-image.jpg'; }";

			AssertError(
				"Missing closing quote (')",
				".cla { background-image: 'my-image.jpg; }",
				1,
				25,
				input);
		}

		[Fact]
		public void NoEndSingleQuote2() {
			var input = @"
.cla { background-image: 'my-image.jpg; }

.clb { background-position: 12px 3px; }";

			AssertError(
				"Missing closing quote (')",
				".cla { background-image: 'my-image.jpg; }",
				1,
				25,
				input);
		}

		[Fact]
		public void NoEndSingleQuote3() {
			var input = @"
.cla { background-image: 'my-image.jpg; } /* comment

.clb { background-position: 12px 3px; }*/";

			AssertError(
				"Missing closing quote (')",
				".cla { background-image: 'my-image.jpg; } /* comment",
				1,
				25,
				input);
		}

		[Fact]
		public void NoEndSingleQuote4() {
			var input = @".cla { background-image: 'my-image.jpg; }";

			AssertError(
				"Missing closing quote (')",
				".cla { background-image: 'my-image.jpg; }",
				1,
				25,
				input);
		}

		[Fact]
		public void EscapeCopesWithIE8Hack1() {
			var input = @"~""\9""";
			var expected = @"\9";

			AssertExpression(expected, input);
		}

		[Fact]
		public void EscapeCopesWithIE8Hack2() {
			var input = @"""\9""";
			var expected = @"""\9""";

			AssertExpression(expected, input);
		}

		[Fact]
		public void EscapeSlash() {
			// from less.js

			var input = @"""\"" \\""";
			var expected = @"""\"" \\""";

			AssertExpression(expected, input);
		}

		[Fact]
		public void NoEndComment() {
			var input = @"
.cla { background-image: 'my-image.jpg'; } /* My comment starts here but isn't closed

.clb { background-image: 'my-second-image.jpg'; }";

			AssertError(
				"Missing closing comment",
				".cla { background-image: 'my-image.jpg'; } /* My comment starts here but isn't closed",
				1,
				43,
				input);
		}
	}
}