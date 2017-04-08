using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Grammar;
using LessonNet.Parser.Util;

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
			using (context.EnterScope(Selectors)) {
				(var rules, var rulesets, var mediaBlocks) = Block.Evaluate(context).Split<Rule, Ruleset, MediaBlock>();

				var evaluatedBlock = new RuleBlock(rules, null) {
					IsEvaluated = true
				};

				var evaluatedRuleset =
					new Ruleset(Selectors.EvaluateSingle<SelectorList>(context), evaluatedBlock) {IsEvaluated = true};

				yield return evaluatedRuleset;

				foreach (var generatedRuleset in rulesets) {
					var combinedSelectors = generatedRuleset.Selectors.Inherit(Selectors);
					yield return new Ruleset(combinedSelectors, generatedRuleset.Block) {IsEvaluated = true};
				}

				foreach (var generatedMediaBlock in mediaBlocks) {
					yield return generatedMediaBlock.EvaluateSingle<MediaBlock>(context);
				}
			}
		}

		protected override string GetCss() {
			if (Block.RuleCount == 0) {
				return string.Empty;
			}

			var builder = new StringBuilder();
			builder.Append(Selectors.ToCss());
			builder.AppendLine(" {");
			builder.Append(Block.ToCss());
			builder.AppendLine("}");
			return builder.ToString();
		}

		protected override string GetStringRepresentation() {
			return $"{Selectors} {{ {Block.RuleCount} }}";
		}
	}
}
