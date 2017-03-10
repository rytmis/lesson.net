using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser
{
	public class EvaluationContext
	{
		public LessTreeParser Parser { get; }
		public IFileResolver FileResolver { get; }

		public EvaluationContext(LessTreeParser parser, IFileResolver fileResolver) {
			Parser = parser;
			FileResolver = fileResolver;
		}

		public EvaluationContext GetImportContext(string importedLessFileName) {
			return new EvaluationContext(Parser, FileResolver.GetResolverFor(importedLessFileName));
		}

		public Stylesheet ParseCurrentStylesheet() {
			using (var stream = FileResolver.GetContent()) {
				return Parser.Parse(FileResolver.CurrentFile, stream);
			}
		}

	}
}
