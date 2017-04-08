using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class ListOfExpressionLists : LessNode, IEnumerable<ExpressionList> {
		private List<ExpressionList> expressionLists;

		public ListOfExpressionLists(IEnumerable<ExpressionList> expressionLists) {
			this.expressionLists = expressionLists.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}

		public IEnumerator<ExpressionList> GetEnumerator() {
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}