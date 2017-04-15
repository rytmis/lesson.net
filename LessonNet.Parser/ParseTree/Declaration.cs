namespace LessonNet.Parser.ParseTree {
	public abstract class Declaration : Statement {
		public abstract void DeclareIn(EvaluationContext context);
	}
}