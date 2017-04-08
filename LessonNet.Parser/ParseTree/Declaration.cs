namespace LessonNet.Parser.ParseTree {
	public abstract class Declaration : Statement {
		public abstract void DeclareIn(Scope scope);
	}
}