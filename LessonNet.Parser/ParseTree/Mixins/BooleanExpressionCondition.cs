using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class BooleanExpressionCondition : Condition {
		private readonly bool negate;
		private readonly Expression expression;
		public BooleanExpressionCondition(bool negate, Expression expression) {
			this.negate = negate;
			this.expression = expression;
		}


		public override bool SatisfiedBy(EvaluationContext context) {
			bool result = expression.EvaluateSingle<BooleanValue>(context).Value;
			return negate ? !result : result;
		}
	}
}