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
		public SelectorList Selectors { get; }
		public RuleBlock Block { get; }

		public Ruleset(SelectorList selectors, RuleBlock block) {
			this.Selectors = selectors;
			this.Block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IList<Rule> generatedRules = new List<Rule>();
			IList<Ruleset> generatedRulesets = new List<Ruleset>();

			foreach (var lessNode in Block.Evaluate(context)) {
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
				new Ruleset(Selectors.EvaluateSingle<SelectorList>(context), evaluatedBlock) {IsEvaluated = true};

			yield return evaluatedRuleset;

			foreach (var generatedRuleset in generatedRulesets) {
				var combinedSelectors = generatedRuleset.Selectors.Inherit(Selectors);
				yield return new Ruleset(combinedSelectors, generatedRuleset.Block) { IsEvaluated = true };
			}
		}

		protected override string GetCss() {
			var builder = new StringBuilder();
			builder.Append(Selectors.ToCss());
			builder.AppendLine(" {");
			builder.Append(Block.ToCss());
			builder.AppendLine("}");
			return builder.ToString();
		}
	}
}
