using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Url : Expression {
		public Expression Content { get; }
		public Url(Expression content) {
			Content = content;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			string GetStringValue(Expression expr) {
				if (expr is LessString str) {
					return str.GetUnquotedValue();
				}

				if (expr is QuotedExpression quoted) {
					return quoted.Value.GetUnquotedValue();
				}

				return expr.ToString();
			}

			Expression GetCorrectedPath(Expression url) {
				var urlAsString = GetStringValue(url);
				if (!urlAsString.IsLocalFilePath()) {
					return url;
				}

				var resolved = context.FileResolver.ResolvePath(urlAsString);

				if (url is LessString str) {
					return new LessString(str.QuoteChar, new[] {new LessStringLiteral(resolved)});
				}

				if (url is QuotedExpression quoted) {
					return new QuotedExpression(new LessString(quoted.Value.QuoteChar, new[] {new LessStringLiteral(resolved)}));
				}

				return new LessStringLiteral(resolved);
			}

			var evaluatedValue = Content.EvaluateSingle<Expression>(context);

			var urlValue = context.RewriteRelativeUrls
				? GetCorrectedPath(evaluatedValue)
				: evaluatedValue;


			yield return new Url(urlValue);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("url(");
			context.Append(Content);
			context.Append(")");
		}

		protected bool Equals(Url other) {
			return Equals(Content, other.Content);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Url) obj);
		}

		public override int GetHashCode() {
			return Content?.GetHashCode() ?? 0;
		}
	}
}