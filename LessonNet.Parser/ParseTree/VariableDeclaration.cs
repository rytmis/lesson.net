﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class VariableDeclaration : Declaration {
		public string Name { get; }
		public ListOfExpressionLists Values { get; }

		public VariableDeclaration(string name, ListOfExpressionLists expressionLists) {
			Name = name;
			Values = expressionLists;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			return Enumerable.Empty<LessNode>();
		}

		public override Statement ForceImportant() {
			if (Values.Important) {
				return this;
			}

			return new VariableDeclaration(Name, new ListOfExpressionLists(Values, Values.Separator, important: true));
		}

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareVariable(this);
		}
	}
}