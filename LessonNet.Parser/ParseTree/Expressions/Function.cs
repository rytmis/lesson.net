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
	}
}