using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Grammar;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree
{
	public class Ruleset : Declaration
	{
		public SelectorList Selectors { get; }
		public RuleBlock Block { get; }

		public Ruleset(SelectorList selectors, RuleBlock block) {
			this.Selectors = selectors;
			this.Block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterScope(Selectors)) {
				(var mediaBlocks, var rulesets, var statements) = Block.Evaluate(context).Split<MediaBlock, Ruleset, Statement>();

				var evaluatedBlock = new RuleBlock(statements) {
					IsEvaluated = true
				};

				var evaluatedRuleset =
					new Ruleset(Selectors.EvaluateSingle<SelectorList>(context), evaluatedBlock) {IsEvaluated = true};

				yield return evaluatedRuleset;

				foreach (var generatedRuleset in rulesets) {
					var combinedSelectors = generatedRuleset.Selectors.Inherit(Selectors).EvaluateSingle<SelectorList>(context);
					yield return new Ruleset(combinedSelectors, generatedRuleset.Block) {IsEvaluated = true};
				}

				foreach (var generatedMediaBlock in mediaBlocks) {
					yield return generatedMediaBlock.EvaluateSingle<MediaBlock>(context);
				}
			}
		}

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareRuleset(new Ruleset(Selectors.EvaluateSingle<SelectorList>(context), Block));
		}

		public override void WriteOutput(OutputContext context) {
			if (Block.RuleCount == 0) {
				return;
			}

			context.Append(Selectors);
			context.AppendLine(" {", indent: false);
			context.Append(Block);
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"{Selectors} {{ {Block.RuleCount} }}";
		}
	}
}
