using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinDefinition : Declaration {
		private readonly RuleBlock block;
		public SelectorList Selectors { get; }

		public MixinDefinition(SelectorList selectors, RuleBlock block) {
			this.Selectors = selectors;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterScope(Selectors)) {
				foreach (var generatedNode in block.Evaluate(context)) {
					yield return generatedNode;
				}
			}
		}

		public override void DeclareIn(Scope scope) {
			scope.DeclareMixin(this);
		}
	}

	public class MixinEvaluationResult : LessNode {
		private readonly MixinDefinition mixin;
		private readonly Scope closure;

		public MixinEvaluationResult(MixinDefinition mixin, Scope closure) {
			this.mixin = mixin;
			this.closure = closure;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				foreach (var evaluationResult in mixin.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}
	}
}