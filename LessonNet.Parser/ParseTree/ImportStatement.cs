using System.Collections.Generic;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public class ImportStatement : Statement
	{
		public ImportStatement(Expression url)
		{
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
}
