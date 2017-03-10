using System;
using System.Collections.Generic;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class MixinDefinition : Statement {
		private readonly SelectorList selectors;
		private readonly RuleBlock block;

		public MixinDefinition(SelectorList selectors, RuleBlock block) {
			this.selectors = selectors;
			this.block = block;
		}
		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			yield return this;
		}
	}
}