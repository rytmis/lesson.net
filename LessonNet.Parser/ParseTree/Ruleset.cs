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
			var evaluatedSelectors = Selectors.EvaluateSingle<SelectorList>(context).Inherit(context.CurrentScope.Selectors);

			using (context.EnterScope(evaluatedSelectors)) {
				(var mediaBlocks, var rulesets, var statements) = Block.Evaluate(context).Split<MediaBlock, Ruleset, Statement>();

				if (statements.Count > 0) {
					var ruleLookup = new HashSet<Rule>();

					for (var i = statements.Count - 1; i >= 0; i--) {
						if (statements[i] is Rule r) {
							if (ruleLookup.Contains(r)) {
								statements.RemoveAt(i);
							} else {
								ruleLookup.Add(r);
							}
						}
					}

					var evaluatedBlock = new RuleBlock(statements);

					var evaluatedRuleset =
						new Ruleset(evaluatedSelectors, evaluatedBlock);

					yield return evaluatedRuleset;
				}

				foreach (var generatedRuleset in rulesets) {
					context.CurrentScope.Parent.DeclareRuleset(generatedRuleset);

					yield return generatedRuleset;
				}

				foreach (var generatedMediaBlock in mediaBlocks) {
					foreach (var result in generatedMediaBlock.Evaluate(context).Cast<MediaBlock>()) {
						yield return result.Bubble(context);
					}
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
			context.AppendLine(" {");
			context.Append(Block);
			context.Indent();
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"{Selectors} {{ {Block.RuleCount} }}";
		}
	}
}
