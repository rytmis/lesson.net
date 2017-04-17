using System.Collections.Generic;
using System.Linq;
using LessonNet.Grammar;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetCall : Statement {
		public SelectorList Selectors { get; }

		public RulesetCall(SelectorList selectors) {
			this.Selectors = selectors.DropCombinators();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var call = new RulesetCall(Selectors.EvaluateSingle<SelectorList>(context));

			foreach (var rulesetResult in context.CurrentScope.ResolveMatchingRulesets(call)) {
				foreach (var evaluationResult in rulesetResult.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}

		public bool Matches(Ruleset ruleset) {
			return ruleset.Selectors.MatchesAny(Selectors);
		}

		public bool Matches(MixinDefinition mixinDefinition) {
			return mixinDefinition.Arity == 0
				|| mixinDefinition.Parameters.All(p => p.HasDefaultValue);
		}

		protected override string GetStringRepresentation() {
			return Selectors.ToString();
		}
	}
}