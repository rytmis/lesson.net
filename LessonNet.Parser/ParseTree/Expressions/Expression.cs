using System.Linq;
using LessonNet.Parser.ParseTree.Mixins;

namespace LessonNet.Parser.ParseTree.Expressions {

	public abstract class Expression : LessNode {
		public abstract override bool Equals(object obj);
		public abstract override int GetHashCode();
	}
}