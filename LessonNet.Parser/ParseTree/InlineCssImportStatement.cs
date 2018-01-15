using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class InlineCssImportStatement : Statement {
		private readonly string content;

		public InlineCssImportStatement(string content) {
			this.content = content;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.AppendLine(content);
		}

		protected bool Equals(InlineCssImportStatement other) {
			return string.Equals(content, other.content);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((InlineCssImportStatement) obj);
		}

		public override int GetHashCode() {
			return (content != null ? content.GetHashCode() : 0);
		}
	}
}