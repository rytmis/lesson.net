using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class GenericAtRule : AtRule {
		private readonly Identifier identifier;
		private readonly Expression value;
		private readonly RuleBlock block;

		public GenericAtRule(Identifier identifier, Expression value, RuleBlock block) {
			this.identifier = identifier;
			this.value = value;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new GenericAtRule(
				identifier.EvaluateSingle<Identifier>(context),
				value?.EvaluateSingle<Expression>(context),
				new RuleBlock(block.Evaluate(context).Cast<Statement>()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append("@");
			context.Append(identifier);
			if (value != null) {
				context.Append(' ');
				context.Append(value);
			}

			context.AppendOptional(' ');
			context.AppendLine("{");
			context.Append(block);

			context.Indent();
			context.AppendLine("}");
		}
	}
}