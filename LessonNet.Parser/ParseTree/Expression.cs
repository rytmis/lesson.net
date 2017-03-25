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
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}

		protected override string GetStringRepresentation() {
			return value;
		}

		protected override string GetCss() {
			return value;
		}
	}

	public class ExpressionList : LessNode, IEnumerable<Expression> {
		private readonly IList<Expression> values;

		public ExpressionList(IEnumerable<Expression> values) {
			this.values = values.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}

		public IEnumerator<Expression> GetEnumerator() {
			return values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		protected override string GetCss() {
			return string.Join(" ", values.Select(v => v.ToCss()));
		}
	}

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