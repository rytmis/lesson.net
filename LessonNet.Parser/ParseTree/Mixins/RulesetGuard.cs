namespace LessonNet.Parser.ParseTree.Mixins {
	public class RulesetGuard : NoOutputNode {
		private readonly OrConditionList conditions;

		public RulesetGuard(OrConditionList conditions) {
			this.conditions = conditions;
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditions.SatisfiedBy(context);
		}
	}
}