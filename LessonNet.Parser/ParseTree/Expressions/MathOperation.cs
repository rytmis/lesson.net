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
	}
}