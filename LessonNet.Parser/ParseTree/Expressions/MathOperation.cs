using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class MathOperation : Expression {
		public Expression LeftOperand { get; }
		public string Operator { get; }
		public Expression RightOperand { get; }

		public MathOperation(Expression lhs, string op, Expression rhs) {
			this.LeftOperand = lhs;
			this.Operator = op;
			this.RightOperand = rhs;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			if (context.StrictMath) {
				yield return new MathOperation(EvaluateSingleValue(LeftOperand, context), Operator, EvaluateSingleValue(RightOperand, context));
			} else {
				yield return ForceEvaluateExpression(context);
			}
		}

		public Expression ForceEvaluateExpression(EvaluationContext context) {
			return MathOperations.Operate(Operator, EvaluateSingleValue(LeftOperand, context), EvaluateSingleValue(RightOperand, context));
		}

		private static Expression EvaluateSingleValue(Expression expr, EvaluationContext context) {
			return expr.EvaluateSingle<Expression>(context);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(LeftOperand);
			context.Append(' ');
			context.Append(Operator);
			context.Append(' ');
			context.Append(RightOperand);
		}

		protected override string GetStringRepresentation() {
			return $"{LeftOperand} {Operator} {RightOperand}";
		}

		protected bool Equals(MathOperation other) {
			return Equals(LeftOperand, other.LeftOperand)
				&& string.Equals(Operator, other.Operator)
				&& Equals(RightOperand, other.RightOperand);
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
				hashCode = (hashCode * 397) ^ (LeftOperand != null ? LeftOperand.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Operator != null ? Operator.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (RightOperand != null ? RightOperand.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}