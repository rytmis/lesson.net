using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class ListOfExpressionLists : LessNode {
		private List<ExpressionList> expressionLists;

		public ListOfExpressionLists(IEnumerable<ExpressionList> expressionLists) {
			this.expressionLists = expressionLists.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var expressionList in this.expressionLists) {
				yield return expressionList.EvaluateSingle<ExpressionList>(context);
			}
		}

		protected override string GetStringRepresentation() {
			return string.Join("; ", expressionLists);
		}
	}
}