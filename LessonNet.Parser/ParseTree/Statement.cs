namespace LessonNet.Parser.ParseTree
{
	public abstract class Statement : LessNode {
		public virtual Statement ForceImportant() {
			return this;
		}
	}
}