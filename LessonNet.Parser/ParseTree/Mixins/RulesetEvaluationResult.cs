using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetEvaluationResult : InvocationResult {
		private readonly Ruleset ruleset;
		private readonly RulesetCall call;
		private readonly Scope closure;

		public bool Matched { get; private set; }

		public RulesetEvaluationResult(Ruleset ruleset, RulesetCall call, Scope closure) {
			// Drop selectors from the invoked ruleset
			this.ruleset = new Ruleset(SelectorList.Empty(), ruleset.Block);
			this.call = call;
			this.closure = closure;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				foreach (var evaluationResult in ruleset.Evaluate(context)) {
					yield return evaluationResult;
				}

				Matched = true;
			}
		}
	}
}