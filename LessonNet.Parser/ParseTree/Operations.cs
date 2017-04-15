using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree
{
	public static class MathOperations {
		public static Expression Operate(string op, Measurement lhs, Measurement rhs) {
			switch (op.ToLowerInvariant()) {
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
