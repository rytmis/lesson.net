namespace LessonNet.Parser.ParseTree.Mixins {
	public abstract class Condition : NoOutputNode {
		public abstract bool SatisfiedBy(EvaluationContext context);
	}
}