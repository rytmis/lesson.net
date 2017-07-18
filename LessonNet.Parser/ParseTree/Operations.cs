using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public static class MathOperations {
		public static Expression Operate(string op, Expression lhs, Expression rhs) {
			if (lhs is Measurement m1 && rhs is Measurement m2) {
				return Operate(op, m1, m2);
			}

			if (lhs is Color lc && rhs is Measurement rm) {
				return Operate(op, lc, new Color(rm));
			}

			if (lhs is Measurement lm && rhs is Color rc) {
				return Operate(op, new Color(lm), rc);
			}

			throw new EvaluationException($"Unsupported operation: {lhs} {op} {rhs}");
		}

		public static Expression Operate(string op, Color c1, Color c2) {
			switch (op) {
				case "-":
					return c1 - c2;
				case "+":
					return c1 + c2;
				case "*":
					return c1 * c2;
				case "/":
					return c1 / c2;
				default:
					throw new EvaluationException($"Unsupported operation: {c1} {op} {c2}");
			}
		}

		public static Expression Operate(string op, Measurement lhs, Measurement rhs) {
			switch (op) {
				case ">":
					return new BooleanValue(lhs.Number > rhs.Number);
				case "<":
					return new BooleanValue(lhs.Number > rhs.Number);
				case "-":
					return new Measurement(lhs.Number - rhs.Number, lhs.Unit ?? rhs.Unit);
				case "+":
					return new Measurement(lhs.Number + rhs.Number, lhs.Unit ?? rhs.Unit);
				case "*":
					return new Measurement(lhs.Number * rhs.Number, lhs.Unit ?? rhs.Unit);
				case "/":
					return new Measurement(lhs.Number / rhs.Number, lhs.Unit ?? rhs.Unit);
				default:
					throw new EvaluationException($"Unexpected operator: {op}");
			}
		}
	}
}
