using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class PageAtRule : AtRule {
		private readonly Selector additionalSelector;
		private readonly RuleBlock block;

		public PageAtRule(Selector additionalSelector, RuleBlock block) {
			this.additionalSelector = additionalSelector;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new PageAtRule(
				additionalSelector?.EvaluateSingle<Selector>(context),
				new RuleBlock(block.Evaluate(context).Cast<Statement>()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append("@page");
			if (additionalSelector != null) {
				context.Append(' ');
				context.Append(additionalSelector);
			}
			context.Append(' ');
			context.AppendLine("{");
			context.Append(block);

			context.Indent();
			context.AppendLine("}");
		}
	}
}