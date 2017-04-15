namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinGuard : NoOutputNode {
		private readonly OrConditionList conditions;

		public MixinGuard(OrConditionList conditions) {
			this.conditions = conditions;
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditions.SatisfiedBy(context);
		}
	}
}