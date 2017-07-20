using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ParenthesizedExpression : Expression {
		private readonly Expression expression;

		public ParenthesizedExpression(Expression expression) {
			this.expression = expression;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return expression.EvaluateSingle<Expression>(context);
		}

		protected bool Equals(ParenthesizedExpression other) {
			return Equals(expression, other.expression);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append('(');
			context.Append(expression);
			context.Append(')');
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ParenthesizedExpression) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (expression != null ? expression.GetHashCode() : 0);
		}
	}
}