using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public class ImportStatement : Statement
	{
		private readonly ImportOptions options;
		private readonly List<MediaQuery> mediaQueries;

		public ImportStatement(Expression url, ImportOptions options, IEnumerable<MediaQuery> mediaQueries) {
			this.options = options;
			this.mediaQueries = mediaQueries.ToList();
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

			var filePath = EvaluateFilePath();

			bool isExplicitCssImport = options.HasFlag(ImportOptions.Css);
			if (isExplicitCssImport || !IsImportableUri(filePath)) {
				return new[] {this};
			}

			var extension = Path.GetExtension(filePath);

			bool isCssFile = extension == ".css";
			bool isExplicitLessImport = options.HasFlag(ImportOptions.Less);
			bool isInlineImport = options.HasFlag(ImportOptions.Inline);

			if (isCssFile && !isExplicitLessImport && !isInlineImport) {
				return new[] {
					new ImportStatement(
						Url.EvaluateSingle<Expression>(context),
						options,
						mediaQueries.Select(mq => mq.EvaluateSingle<MediaQuery>(context))),
				};
			}

			var actualImportPath = string.IsNullOrEmpty(extension)
				? Path.ChangeExtension(filePath, "less")
				: filePath;


			IEnumerable<Statement> GetImportResults(bool optional) {
				EvaluationContext importContext;
				try {
					importContext = context.GetImportContext(actualImportPath);
				} catch (Exception ex) {
					if (optional) {
						return null;
					}

					throw new EvaluationException($"Failed to import {filePath}", ex);
				}

				if (isCssFile && isInlineImport) {
					return new Statement[] {new InlineCssImportStatement(importContext.GetFileContent())};
				}

				return importContext
					.ParseCurrentStylesheet(isReference: options.HasFlag(ImportOptions.Reference))
					.Evaluate(importContext)
					.OfType<Statement>();
			}

			var importResults = GetImportResults(options.HasFlag(ImportOptions.Optional));
			if (importResults == null) {
				return Enumerable.Empty<LessNode>();
			}

			if (mediaQueries.Any()) {
				return new[] {new MediaBlock(mediaQueries, new RuleBlock(importResults)).EvaluateSingle<MediaBlock>(context)};
			}

			return importResults;
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append("@import ");
			context.Append(Url);
			if (mediaQueries.Any()) {
				context.Append(' ');
				context.Append(mediaQueries, ", ");
			}
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
		Multiple = 32,
		Optional = 64
	}
}
