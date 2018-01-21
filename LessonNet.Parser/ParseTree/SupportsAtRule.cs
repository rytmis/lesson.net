using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class SupportsAtRule : AtRule {
		private readonly SupportsCondition condition;
		private readonly RuleBlock block;

		public SupportsAtRule(SupportsCondition condition, RuleBlock block) {
			this.condition = condition;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new SupportsAtRule(condition.EvaluateSingle<SupportsCondition>(context), new RuleBlock(block.Evaluate(context).Cast<Statement>()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("@supports");
			context.Append(' ');
			context.Append(condition);
			context.AppendOptional(' ');

			context.AppendLine("{");
			context.Append(block);
			context.AppendLine("}");
		}
	}

	public abstract class SupportsCondition : LessNode {
		public bool Negate { get; }

		protected SupportsCondition(bool negate) {
			Negate = negate;
		}
	}

	public class PropertySupportsCondition : SupportsCondition {
		private readonly Rule property;

		public PropertySupportsCondition(bool negate, Rule property) : base(negate) {
			this.property = property;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new PropertySupportsCondition(Negate, property.EvaluateSingle<Rule>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append('(');
			context.Append(property);
			context.Append(')');
		}
	}

	public class ConjunctionSupportsCondition : SupportsCondition {
		private readonly IList<SupportsCondition> conditions;

		public ConjunctionSupportsCondition(bool negate, IEnumerable<SupportsCondition> conditions) : base(negate) {
			this.conditions = conditions.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new ConjunctionSupportsCondition(Negate, conditions.Select(c => c.EvaluateSingle<SupportsCondition>(context)));
		}

		public override void WriteOutput(OutputContext context) {
			for (var index = 0; index < conditions.Count; index++) {
				var supportsCondition = conditions[index];

				context.Append(supportsCondition);

				if (index < conditions.Count - 1) {
					context.Append(" and ");
				}
			}
		}
	}

	public class DisjunctionSupportsCondition : SupportsCondition {
		private readonly IList<SupportsCondition> conditions;

		public DisjunctionSupportsCondition(bool negate, IEnumerable<SupportsCondition> conditions) : base(negate) {
			this.conditions = conditions.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new DisjunctionSupportsCondition(Negate, conditions.Select(c => c.EvaluateSingle<SupportsCondition>(context)));
		}

		public override void WriteOutput(OutputContext context) {
			for (var index = 0; index < conditions.Count; index++) {
				var supportsCondition = conditions[index];

				context.Append(supportsCondition);

				if (index < conditions.Count - 1) {
					context.Append(" or ");
				}
			}
		}
	}
}