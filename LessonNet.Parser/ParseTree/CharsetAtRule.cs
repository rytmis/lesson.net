using System.Collections.Generic;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class CharsetAtRule : AtRule {
		private readonly LessString charset;

		public CharsetAtRule(LessString charset) {
			this.charset = charset;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}
}