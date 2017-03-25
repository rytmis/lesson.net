using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Grammar;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree
{
	public class Ruleset : Statement
	{
		private readonly SelectorList selectors;
		private readonly RuleBlock block;

		public Ruleset(SelectorList selectors, RuleBlock block) {
			this.selectors = selectors;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IList<Rule> generatedRules = new List<Rule>();
			IList<Ruleset> generatedRulesets = new List<Ruleset>();

			foreach (var lessNode in block.Evaluate(context)) {
				switch (lessNode) {
					case Rule r:
						generatedRules.Add(r);
						break;
					case Ruleset rs: 
						generatedRulesets.Add(rs);
						break;
					default:
						throw new EvaluationException(
							$"Unexpected evaluation result: rule block produced node with type {lessNode.GetType().Name}");
				}
			}

			var evaluatedBlock = new RuleBlock(generatedRules, null) {
				IsEvaluated = true
			};

			var evaluatedRuleset =
				new Ruleset(selectors.EvaluateSingle<SelectorList>(context), evaluatedBlock) {IsEvaluated = true};

			yield return evaluatedRuleset;

			foreach (var generatedRuleset in generatedRulesets) {
				var combinedSelectors = generatedRuleset.selectors.Inherit(selectors);
				yield return new Ruleset(combinedSelectors, generatedRuleset.block) { IsEvaluated = true };
			}
		}

		protected override string GetCss() {
			var builder = new StringBuilder();
			builder.Append(selectors.ToCss());
			builder.AppendLine(" {");
			builder.Append(block.ToCss());
			builder.AppendLine("}");
			return builder.ToString();
		}
	}
}
