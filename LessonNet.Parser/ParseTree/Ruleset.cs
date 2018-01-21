using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LessonNet.Grammar;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Mixins;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree
{
	public class Ruleset : Declaration
	{
		public SelectorList Selectors { get; }
		public RuleBlock Block { get; }

		public RulesetGuard Guard { get; }

		public Ruleset(SelectorList selectors, RulesetGuard guard, RuleBlock block) {
			this.Selectors = selectors;
			Guard = guard;
			this.Block = block;
		}

		public Ruleset(SelectorList selectors, RuleBlock block) {
			this.Selectors = selectors;
			Guard = null;
			this.Block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			if (Guard?.SatisfiedBy(context) == false) {
				yield break;
			}

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
					} else if (statement is Stylesheet || statement is ImportStatement) {
						yield return statement;
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

			
			if (context.IsReference) {
				var extenderSelectorList = new SelectorList(Selectors.Selectors.SelectMany(s => context.Extensions.GetExtensions(s, includeReferences: false)));
				if (extenderSelectorList.IsEmpty()) {
					return;
				}

				context.Append(extenderSelectorList);
			} else {
				context.Append(Selectors);
			}

			context.AppendOptional(' ');
			context.AppendLine("{");
			context.Append(Block);
			context.Indent();
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"{Selectors} {{ {Block.RuleCount} }}";
		}
	}
}
