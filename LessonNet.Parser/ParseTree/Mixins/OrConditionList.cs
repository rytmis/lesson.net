using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class OrConditionList : NoOutputNode {
		private readonly IList<AndConditionList> conditionLists;

		public OrConditionList(IEnumerable<AndConditionList> conditionLists) {
			this.conditionLists = conditionLists.ToList();
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditionLists.Any(cl => cl.SatisfiedBy(context));
		}
	}
}