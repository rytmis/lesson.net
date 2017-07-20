using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class DocumentAtRule : AtRule {
		private readonly RuleBlock block;
		private readonly List<Expression> specifiers;

		public DocumentAtRule(IEnumerable<Expression> specifiers, RuleBlock block) {
			this.block = block;
			this.specifiers = specifiers.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}
}