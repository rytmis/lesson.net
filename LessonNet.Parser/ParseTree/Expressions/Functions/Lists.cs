using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class LengthFunction : LessFunction {
		public LengthFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var variable = Arguments.Single<Variable>();
			if (variable != null) {
				yield return EvaluateFunction(variable.EvaluateSingle<ListOfExpressionLists>(context));
			} else {
				yield return EvaluateFunction(Arguments.EvaluateSingle<ListOfExpressionLists>(context));
			}
		}

		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			if (arguments.Count == 1) {
				return new Measurement(arguments[0].Values.Count, "");
			}

			return new Measurement(arguments.Count, "");
		}
	}
}
