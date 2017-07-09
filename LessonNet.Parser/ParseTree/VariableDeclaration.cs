using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class VariableDeclaration : Declaration {
		public string Name { get; }
		public ListOfExpressionLists Values { get; }

		public VariableDeclaration(string name, ListOfExpressionLists expressionLists) {
			Values = expressionLists;
			Name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			return Enumerable.Empty<LessNode>();
		}

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareVariable(this);
		}
	}
}