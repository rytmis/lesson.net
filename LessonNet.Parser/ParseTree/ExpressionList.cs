﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class ExpressionList : LessNode, IEnumerable<Expression> {
		private readonly IList<Expression> values;

		public ExpressionList(IEnumerable<Expression> values) {
			this.values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<Expression> EvaluateExpressions() {
				foreach (var expression in values) {
					foreach (var evaluatedExpression in expression.Evaluate(context)) {
						if (evaluatedExpression is ExpressionList list) {
							foreach (var evaluatedValue in list.values) {
								yield return evaluatedValue;
							}
						} else if (evaluatedExpression is Expression expr) {
							yield return expr;
						} else {
							throw new EvaluationException($"Expected {nameof(ExpressionList)}, got {evaluatedExpression.GetType().Name}");
						}
					}
				}
			}

			yield return new ExpressionList(EvaluateExpressions());
		}

		public IEnumerator<Expression> GetEnumerator() {
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		protected override string GetCss() {
			return string.Join(" ", Enumerable.Select<Expression, string>(values, v => v.ToCss()));
		}
	}
}