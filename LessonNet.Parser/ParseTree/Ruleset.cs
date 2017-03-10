using System;
using System.Collections.Generic;
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

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context)
		{
			throw new NotImplementedException();
		}
	}
}
