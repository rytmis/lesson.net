using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetEvaluationResult : InvocationResult {
		private readonly Ruleset ruleset;
		private readonly RulesetCall call;
		private readonly Scope closure;

		public bool Matched { get; private set; }

		public RulesetEvaluationResult(Ruleset ruleset, RulesetCall call, Scope closure) {
			this.ruleset = ruleset;
			this.call = call;
			this.closure = closure;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				foreach (var evaluationResult in ruleset.Block.Evaluate(context)) {
					if (evaluationResult is Declaration decl) {
						decl.DeclareIn(context);
					}

					if (call.Important && evaluationResult is Statement s) {
						yield return s.ForceImportant();
					} else {
						yield return evaluationResult;
					}
				}

				Matched = true;
			}
		}
	}
}