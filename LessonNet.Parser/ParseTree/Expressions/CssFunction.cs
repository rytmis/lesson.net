using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class CssFunction : Expression {
		private readonly string functionName;
		private readonly Expression arguments;

		public CssFunction(string functionName, Expression arguments) {
			this.functionName = functionName;
			this.arguments = arguments;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new CssFunction(functionName, arguments?.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(functionName);
			context.Append('(');
			context.Append(arguments);
			context.Append(')');
		}

		protected bool Equals(CssFunction other) {
			return string.Equals(functionName, other.functionName) 
				&& arguments.Equals(other.arguments);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CssFunction) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (functionName != null ? functionName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (arguments != null ? arguments.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}