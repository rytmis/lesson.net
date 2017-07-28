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

		protected override string GetStringRepresentation() {
			return $"({lhs} {comparison} {rhs})";
		}
	}

	public static class ComparisonOperations {
		public static bool Compare(string op, Expression lhs, Expression rhs) {
			if (lhs is Measurement m1 && rhs is Measurement m2) {
				return NumericCompare(op, m1, m2);
			}

			if (lhs is Color c1 && rhs is Color c2) {
				return ColorCompare(op, c1, c2);
			}

			if (op == "=") {
				return Equals(lhs, rhs);
			}

			throw new InvalidOperationException("Comparison only works with comparable operands");
		}

		private static bool NumericCompare(string op, Measurement lhs, Measurement rhs) {
			switch (op) {
				case ">":
					return lhs.Number > rhs.Number;
				case "<":
					return lhs.Number < rhs.Number;
				case ">=":
					return lhs.Number >= rhs.Number;
				case "<=":
					return lhs.Number <= rhs.Number;
				case "=<":
					return lhs.Number <= rhs.Number;
				case "=":
					return lhs.Number == rhs.Number;
				default:
					throw new EvaluationException($"Unexpected operator: {op}");
			}
		}

		private static bool ColorCompare(string op, Color lhs, Color rhs) {
			switch (op) {
				case ">":
					return lhs > rhs;
				case "<":
					return lhs < rhs;
				case ">=":
					return lhs >= rhs;
				case "<=":
					return lhs <= rhs;
				case "=<":
					return lhs <= rhs;
				case "=":
					return lhs == rhs;
				case "!=":
					return lhs != rhs;
				default:
					throw new EvaluationException($"Unexpected operator: {op}");
			}
		}
	}
}