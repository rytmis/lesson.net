using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ParenthesizedExpression : Expression {
		private readonly Expression expression;

		public ParenthesizedExpression(Expression expression) {
			this.expression = expression;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return expression.EvaluateSingle<Expression>(context);
		}
	}
}