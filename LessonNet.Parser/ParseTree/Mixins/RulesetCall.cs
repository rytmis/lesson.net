using System.Collections.Generic;
using System.Linq;
using LessonNet.Grammar;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetCall : Statement {
		public bool Important { get; }
		public Selector Selector { get; }

		public RulesetCall(Selector selector, bool important) {
			this.Important = important;
			this.Selector = selector.DropCombinators();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var call = new RulesetCall(Selector.EvaluateSingle<Selector>(context), Important);

			foreach (var rulesetResult in context.CurrentScope.ResolveMatchingRulesets(call)) {
				foreach (var evaluationResult in rulesetResult.Evaluate(context).Cast<Statement>()) {
					if (Important) {
						yield return evaluationResult.ForceImportant();
					} else {
						yield return evaluationResult;
					}
				}
			}
		}

		public override Statement ForceImportant() {
			if (Important) {
				return this;
			}

			return new RulesetCall(Selector, important: true);
		}

		public bool Matches(Ruleset ruleset) {
			return ruleset.Selectors.Matches(Selector);
		}

		public bool Matches(MixinDefinition mixinDefinition) {
			return mixinDefinition.Arity == 0
				|| mixinDefinition.Parameters.All(p => p.HasDefaultValue);
		}

		protected override string GetStringRepresentation() {
			return Selector.ToString();
		}
	}
}