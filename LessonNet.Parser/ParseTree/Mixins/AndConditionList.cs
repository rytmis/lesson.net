using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class AndConditionList : NoOutputNode {
		private readonly IList<Condition> conditions;

		public AndConditionList(IEnumerable<Condition> conditions) {
			this.conditions = conditions.ToList();
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditions.All(c => c.SatisfiedBy(context));
		}

		protected override string GetStringRepresentation() {
			return string.Join(" and ", conditions.Select(c => c.ToString()));
		}
	}
}