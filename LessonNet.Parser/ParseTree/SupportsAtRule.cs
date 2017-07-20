using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class SupportsAtRule : AtRule {
		private readonly SupportsCondition condition;
		private readonly RuleBlock block;

		public SupportsAtRule(SupportsCondition condition, RuleBlock block) {
			this.condition = condition;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}

	public abstract class SupportsCondition : LessNode {
		protected SupportsCondition(bool negate) {
			
		}
	}

	public class PropertySupportsCondition : SupportsCondition {
		private readonly Rule property;

		public PropertySupportsCondition(bool negate, Rule property) : base(negate) {
			this.property = property;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}

	public class ConjunctionSupportsCondition : SupportsCondition {
		private readonly IList<SupportsCondition> conditions;

		public ConjunctionSupportsCondition(bool negate, IEnumerable<SupportsCondition> conditions) : base(negate) {
			this.conditions = conditions.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}

	public class DisjunctionSupportsCondition : SupportsCondition {
		private readonly IList<SupportsCondition> conditions;

		public DisjunctionSupportsCondition(bool negate, IEnumerable<SupportsCondition> conditions) : base(negate) {
			this.conditions = conditions.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}
}