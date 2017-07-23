using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

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
			if (context.StrictMath) {
				yield return new MathOperation(EvaluateSingleValue(lhs, context), op, EvaluateSingleValue(rhs, context));
			} else {
				yield return ForceEvaluateExpression(context);
			}
		}

		public Expression ForceEvaluateExpression(EvaluationContext context) {
			return MathOperations.Operate(op, EvaluateSingleValue(lhs, context), EvaluateSingleValue(rhs, context));
		}

		private static Expression EvaluateSingleValue(Expression expr, EvaluationContext context) {
			return expr.EvaluateSingle<Expression>(context);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(lhs);
			context.Append(' ');
			context.Append(op);
			context.Append(' ');
			context.Append(rhs);
		}

		protected override string GetStringRepresentation() {
			return $"{lhs} {op} {rhs}";
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