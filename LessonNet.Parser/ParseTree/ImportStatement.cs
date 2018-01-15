using System;
using System.Collections.Generic;
using System.IO;
using LessonNet.Parser.CodeGeneration;
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

				return url.StringContent?.GetUnquotedValue()
					?? url.RawUrl;
			}

			bool IsImportableUri(string uri) {
				if (uri.StartsWith("//")) {
					// Protocol-relative URI
					return false;
				}

				if (!Uri.TryCreate(uri, UriKind.RelativeOrAbsolute, out Uri parsedUri)) {
					return false;
				}

				if (parsedUri.IsAbsoluteUri && parsedUri.Scheme != "file") {
					return false;
				}

				return true;
			}

			var isExplicitCssImport = options.HasFlag(ImportOptions.Css);
			var filePath = EvaluateFilePath();
			if (isExplicitCssImport || !IsImportableUri(filePath)) {
				return new[] {this};
			}

			var extension = Path.GetExtension(filePath);

			if (extension == ".css" && !options.HasFlag(ImportOptions.Less)) {
				return new[] {this};
			}


			var actualImportPath = string.IsNullOrEmpty(extension)
				? Path.ChangeExtension(filePath, "less")
				: filePath;

			var importContext = context.GetImportContext(actualImportPath);

			return importContext
				.ParseCurrentStylesheet(isReference: options.HasFlag(ImportOptions.Reference))
				.Evaluate(importContext);
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append("@import ");
			context.Append(Url);
			context.AppendLine(";");
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
