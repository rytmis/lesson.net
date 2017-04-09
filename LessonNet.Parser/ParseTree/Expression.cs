using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {

	public abstract class Expression : LessNode {

	}

	public class ParenthesizedExpression : Expression {
		private readonly Expression expression;

		public ParenthesizedExpression(Expression expression) {
			this.expression = expression;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return expression.EvaluateSingle<Expression>(context);
		}
	}

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
				if (list.Count() != 1) {
					throw new EvaluationException($"{expr} did not evaluate to a single value");
				}

				var singleValue = list.Single();
				if (singleValue is Measurement m) {
					return m;
				}
			}

			throw new EvaluationException($"{expr} is not a numeric expression");
		}
	}

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

	public class Identifier : Expression {
		private readonly string name;

		public Identifier(string name) {
			this.name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return name;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(name);
		}
	}

	public class BooleanValue : Expression {
		public bool Value { get; }

		public BooleanValue(bool value) {
			this.Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}
}