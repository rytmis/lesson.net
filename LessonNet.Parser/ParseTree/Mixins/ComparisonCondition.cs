using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class ComparisonCondition : Condition {
		private readonly bool negate;
		private MathOperation comparisonOperation;

		public ComparisonCondition(bool negate, Expression lhs, string comparison, Expression rhs) {
			this.negate = negate;
			this.comparisonOperation = new MathOperation(lhs, comparison, rhs);
		}

		public override bool SatisfiedBy(EvaluationContext context) {
			var result = comparisonOperation.EvaluateSingle<BooleanValue>(context).Value;
			return negate ? !result : result;
		}
	}
}