using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class Expression : LessNode {
		private readonly string value;

		public Expression(string value) {
			this.value = value;
		}
		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}

		protected override string GetStringRepresentation() {
			return value;
		}
	}

	public class ExpressionList : LessNode, IEnumerable<Expression> {
		private readonly IList<Expression> values;

		public ExpressionList(IEnumerable<Expression> values) {
			this.values = values.ToList();
		}
		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}

		public IEnumerator<Expression> GetEnumerator() {
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}

	public class ListOfExpressionLists : LessNode, IEnumerable<ExpressionList> {
		private List<ExpressionList> expressionLists;

		public ListOfExpressionLists(IEnumerable<ExpressionList> expressionLists) {
			this.expressionLists = expressionLists.ToList();
		}

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
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