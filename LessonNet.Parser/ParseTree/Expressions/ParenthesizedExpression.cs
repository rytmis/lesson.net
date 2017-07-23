using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ParenthesizedExpression : Expression {
		private readonly Expression expression;

		public ParenthesizedExpression(Expression expression) {
			this.expression = expression;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			// Support strict math mode: if the parenthesized expression is a math expression,
			// force-compute the value (this is correct regardless of mode)
			if (expression is MathOperation math) {
				yield return math.ForceEvaluateExpression(context);
			} else {
				yield return expression.EvaluateSingle<Expression>(context);
			}
		}

		protected bool Equals(ParenthesizedExpression other) {
			return Equals(expression, other.expression);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append('(');
			context.Append(expression);
			context.Append(')');
		}

		protected override string GetStringRepresentation() {
			return $"({expression})";
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