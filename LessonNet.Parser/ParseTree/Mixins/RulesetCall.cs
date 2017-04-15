using System.Collections.Generic;
using LessonNet.Grammar;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetCall : Statement {
		private readonly SelectorList selectors;

		public RulesetCall(SelectorList selectors) {
			this.selectors = selectors;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var call = new RulesetCall(selectors.EvaluateSingle<SelectorList>(context));

			foreach (var rulesetResult in context.CurrentScope.ResolveMatchingRulesets(call)) {
				foreach (var evaluationResult in rulesetResult.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}

		public bool Matches(Ruleset ruleset) {
			return ruleset.Selectors.MatchesAny(selectors);
		}

		protected override string GetStringRepresentation() {
			return selectors.ToString();
		}
	}
}