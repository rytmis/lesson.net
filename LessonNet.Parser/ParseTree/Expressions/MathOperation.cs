using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class MathOperation : Expression {
		private readonly Expression lhs;
		private readonly string op;
		private readonly Expression rhs;

		public MathOperation(Expression lhs, string op, Expression rhs) {
			this.lhs = lhs;
			this.op = op;
			this.rhs = rhs;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return MathOperations.Operate(op, EvaluateMeasurement(lhs, context),  EvaluateMeasurement(rhs, context));
		}

		private Measurement EvaluateMeasurement(Expression expr, EvaluationContext context) {
			var evaluatedExpression = expr.EvaluateSingle<LessNode>(context);

			if (evaluatedExpression is Measurement measurement) {
				return measurement;
			}

			if (evaluatedExpression is ExpressionList list) {
				if (list.Values.Count != 1) {
					throw new EvaluationException($"{expr} did not evaluate to a single value");
				}

				var singleValue = list.Values.Single();
				if (singleValue is Measurement m) {
					return m;
				}
			}

			throw new EvaluationException($"{expr} is not a numeric expression");
		}

		protected bool Equals(MathOperation other) {
			return Equals(lhs, other.lhs)
				&& string.Equals(op, other.op)
				&& Equals(rhs, other.rhs);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((MathOperation) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (lhs != null ? lhs.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (op != null ? op.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (rhs != null ? rhs.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}