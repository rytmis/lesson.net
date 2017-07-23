using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class LengthFunction : LessFunction {
		public LengthFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {
			if (arguments is ExpressionList list) {
				return new Measurement(list.Values.Count, "");
			}

			return new Measurement(1, "");
		}

		protected override string GetStringRepresentation() {
			return $"length({Arguments})";
		}
	}
	public class ExtractFunction : LessFunction {
		public ExtractFunction(Expression arguments) : base(arguments) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var evaluatedArgs = Arguments.EvaluateSingle<Expression>(context);
			if (!(evaluatedArgs is ExpressionList list) || list.Values.Count < 2) {
				throw new EvaluationException("Extract requires at least two arguments");
			}

			if (list.Values[0] is ExpressionList target && list.Values[1] is Measurement position) {
				if (position.Number >= 1 || position.Number <= target.Values.Count) {
					yield return target.Values[((int) position.Number) - 1];
				} else {
					var remainingExpressions = new[]{target}.Concat(list.Values.Skip(1));
					var remainingList = new ExpressionList(remainingExpressions, ',');

					yield return new CssFunction("extract", remainingList);
				}
			} else {
				yield return new CssFunction("extract", evaluatedArgs);
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
