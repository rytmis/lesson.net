using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class CharsetAtRule : AtRule {
		private readonly LessString charset;

		public CharsetAtRule(LessString charset) {
			this.charset = charset;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new CharsetAtRule(charset.EvaluateSingle<LessString>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("@charset ");
			context.Append(charset);
			context.AppendLine(";");
		}
	}
}