using System;
using System.Collections.Generic;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class ComparisonCondition : Condition {
		private readonly bool negate;
		private readonly Expression lhs;
		private readonly string comparison;
		private readonly Expression rhs;

		public ComparisonCondition(bool negate, Expression lhs, string comparison, Expression rhs) {
			this.negate = negate;
			this.lhs = lhs;
			this.comparison = comparison;
			this.rhs = rhs;
		}

		public override bool SatisfiedBy(EvaluationContext context) {
			bool result = ComparisonOperations.Compare(comparison, Evaluate(lhs, context), Evaluate(rhs, context));

			return negate ? !result : result;
		}

		private Expression Evaluate(Expression expr, EvaluationContext context) {
			return expr.EvaluateSingle<Expression>(context);
		}
	}

	public static class ComparisonOperations {
		public static bool Compare(string op, Expression lhs, Expression rhs) {
			switch (op.ToLowerInvariant()) {
				case ">":
					return NumericCompare(lhs, rhs, (l, r) => l.Number > r.Number);
				case "<":
					return NumericCompare(lhs, rhs, (l, r) => l.Number < r.Number);
				case ">=":
					return NumericCompare(lhs, rhs, (l, r) => l.Number >= r.Number);
				case "<=":
					return NumericCompare(lhs, rhs, (l, r) => l.Number <= r.Number);
				case "=<":
					return NumericCompare(lhs, rhs, (l, r) => l.Number <= r.Number);
				case "=":
					return Equals(lhs, rhs);
				default:
					throw new EvaluationException($"Unexpected operator: {op}");
			}
		}

		private static bool NumericCompare(Expression lhs, Expression rhs, Func<Measurement, Measurement, bool> compare) {
			if (!(lhs is Measurement m1) || !(rhs is Measurement m2)) {
				throw new InvalidOperationException("Comparison only works with numeric operands");
			}

			return compare(m1, m2);
		}
	}
}