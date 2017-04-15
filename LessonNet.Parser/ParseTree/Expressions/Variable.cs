using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Variable : Expression {
		private readonly string name;

		public Variable(string name) {
			this.name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var expressionList in context.CurrentScope.ResolveVariable(name).Values) {
				yield return expressionList.EvaluateSingle<ExpressionList>(context);
			}
		}

		protected override string GetStringRepresentation() {
			return $"@{name}";
		}
	}
}