using System;
using System.Collections.Generic;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public class ImportStatement : Statement
	{
		private readonly ImportOptions options;

		public ImportStatement(Expression url, ImportOptions options) {
			this.options = options;
			Url = url;
		}

		public Expression Url { get; }

		protected override string GetStringRepresentation()
		{
			return $"@import '{Url}'";
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			string EvaluateFilePath() {
				var expr = Url.EvaluateSingle<Expression>(context);
				if (expr is LessString str) {
					return str.GetUnquotedValue();
				}

				var url = (Url) expr;

				return url.StringContent.GetUnquotedValue();
			}

			var importContext = context.GetImportContext(EvaluateFilePath());

			return importContext.ParseCurrentStylesheet().Evaluate(importContext);
		}
	}

	[Flags]
	public enum ImportOptions {
		None = 0,
		Reference = 1,
		Inline = 2,
		Less = 4,
		Css = 8,
		Once = 16,
		Multiple = 32
	}
}
