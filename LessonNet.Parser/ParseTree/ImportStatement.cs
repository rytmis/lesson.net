using System.Collections.Generic;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree
{
	public class ImportStatement : Statement
	{
		public ImportStatement(string url)
		{
			Url = url;
		}

		public string Url { get; }

		protected override string GetStringRepresentation()
		{
			return $"@import '{Url}'";
		}

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			var importContext = context.GetImportContext(Url);

			return importContext.ParseCurrentStylesheet().Evaluate(importContext);
		}
	}
}
