using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinDefinition : Declaration {
		private readonly SelectorList selectors;
		private readonly RuleBlock block;

		public MixinDefinition(SelectorList selectors, RuleBlock block) {
			this.selectors = selectors;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			return Enumerable.Empty<LessNode>();
		}

		public override void DeclareIn(Scope scope) {
			
		}
	}
}