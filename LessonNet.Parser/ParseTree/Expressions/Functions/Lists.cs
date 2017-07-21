using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class LengthFunction : LessFunction {
		public LengthFunction(Expression arguments) : base(arguments) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var variable = Arguments as Variable;
			if (variable != null) {
				yield return EvaluateFunction(variable.EvaluateSingle<Expression>(context));
			} else {
				yield return EvaluateFunction(Arguments.EvaluateSingle<Expression>(context));
			}
		}

		protected override Expression EvaluateFunction(Expression arguments) {
			if (arguments is ExpressionList list) {
				return new Measurement(list.Values.Count, "");
			}

			return new Measurement(1, "");
		}
	}
}
