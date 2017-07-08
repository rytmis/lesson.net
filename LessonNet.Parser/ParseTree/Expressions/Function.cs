using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Function : Expression {
		private readonly string functionName;
		private readonly List<ExpressionList> arguments;

		public Function(string functionName, IEnumerable<ExpressionList> arguments) {
			this.functionName = functionName;
			this.arguments = arguments.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}

		protected bool Equals(Function other) {
			return string.Equals(functionName, other.functionName) 
				&& arguments.SequenceEqual(other.arguments);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Function) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (functionName != null ? functionName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (arguments != null ? arguments.Aggregate(hashCode, (h, a) => (h * 397) ^ a.GetHashCode()) : 0);
				return hashCode;
			}
		}
	}
}