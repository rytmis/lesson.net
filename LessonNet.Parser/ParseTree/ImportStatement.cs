using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

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
			string EvaluateFilePath(Expression expr) {
				if (expr is LessString str) {
					return str.GetUnquotedValue();
				}

				if (expr is Url url) {
					return EvaluateFilePath(url.Content);
				}

				return expr.ToString();
			}

			// Import resolution takes care of relative paths regardless of the
			// rewrite setting, so while evaluating the import URL, we turn the
			// rewrite setting off.
			bool rewrite = context.RewriteRelativeUrls;
			context.RewriteRelativeUrls = false;
			var evaluatedUrl = Url.EvaluateSingle<Expression>(context);
			context.RewriteRelativeUrls = rewrite;

			var filePath = EvaluateFilePath(evaluatedUrl);

			bool isExplicitCssImport = options.HasFlag(ImportOptions.Css);
			if (isExplicitCssImport || !filePath.IsLocalFilePath()) {
				return new[] {this};
			}

			var extension = Path.GetExtension(filePath);

			bool isCssFile = extension == ".css";
			bool isExplicitLessImport = options.HasFlag(ImportOptions.Less);
			bool isInlineImport = options.HasFlag(ImportOptions.Inline);

			if (isCssFile && !isExplicitLessImport && !isInlineImport) {
				return new[] {
					new ImportStatement(
						evaluatedUrl,
						options,
						mediaQueries.Select(mq => mq.EvaluateSingle<MediaQuery>(context))),
				};
			}

			var actualImportPath = string.IsNullOrEmpty(extension)
				? Path.ChangeExtension(filePath, "less")
				: filePath;


			var importResults = GetImportResults(options.HasFlag(ImportOptions.Optional));

			if (importResults == null) {
				return Enumerable.Empty<LessNode>();
			}

			if (mediaQueries.Any()) {
				return new[] {new MediaBlock(mediaQueries, new RuleBlock(importResults)).EvaluateSingle<MediaBlock>(context)};
			}

			return importResults;

			IEnumerable<Statement> GetImportResults(bool optional) {
				if (context.SeenImport(actualImportPath))
				{
					return Enumerable.Empty<Statement>();
				}

				if (!options.HasFlag(ImportOptions.Reference)) {
					context.NoteImport(actualImportPath);
				}

				using (context.EnterImportScope(actualImportPath)) {
					try {
						if (isCssFile && isInlineImport) {
							return new Statement[] {new InlineCssImportStatement(context.GetFileContent())};
						}
						

						return context
							.ParseCurrentStylesheet(isReference: options.HasFlag(ImportOptions.Reference))
							.Evaluate(context)
							.OfType<Statement>()
							.ToList();

					} catch (IOException ex) {
						if (optional) {
							return null;
						}

						throw new EvaluationException($"Failed to import {filePath}", ex);
					}
				}
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append("@import");
			context.Append(' ');
			context.Append(Url);
			if (mediaQueries.Any()) {
				context.Append(' ');

				WriteQueries(context);
			}
			context.AppendLine(";");
		}

		private void WriteQueries(OutputContext context) {
			bool first = true;
			foreach (var node in (IEnumerable<LessNode>) mediaQueries) {
				if (!first) {
					context.Append(',');
					context.AppendOptional(' ');
				}

				node.WriteOutput(context);

				first = false;
			}
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
