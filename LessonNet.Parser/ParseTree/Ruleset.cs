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
			var evaluatedSelectors = Selectors.Inherit(context.CurrentScope.Selectors).EvaluateSingle<SelectorList>(context);
			evaluatedSelectors.AddExtenders(context);

			using (context.EnterScope(evaluatedSelectors)) {
				(var rules, var others) = Block.Evaluate(context).Split<Rule, Statement>();

				if (rules.Count > 0) {
					var ruleLookup = new HashSet<Rule>();

					for (var i = rules.Count - 1; i >= 0; i--) {
						var r = rules[i];
						if (ruleLookup.Contains(r)) {
							rules.RemoveAt(i);
						} else {
							ruleLookup.Add(r);
						}
					}

					var evaluatedBlock = new RuleBlock(rules);

					var evaluatedRuleset =
						new Ruleset(evaluatedSelectors, evaluatedBlock);

					yield return evaluatedRuleset;
				}

				foreach (var statement in others) {
					if (statement is Ruleset rs) {
						context.CurrentScope.Parent.DeclareRuleset(rs);

						yield return rs;
					} else if (statement is MediaBlock media) {
						yield return media.Bubble(context);
					} else {
						throw new EvaluationException($"Unexpected statement type after evaluating rule block: {statement.GetType()}");
					}
				}
			}
		}

		public override Statement ForceImportant() {
			return new Ruleset(Selectors, Block.ForceImportant());
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
