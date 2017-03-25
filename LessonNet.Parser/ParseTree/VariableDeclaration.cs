using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class VariableDeclaration : Statement {
		private readonly List<ExpressionList> expressionLists;

		public string Name { get; }
		public IEnumerable<ExpressionList> Values => expressionLists.AsReadOnly();

		public VariableDeclaration(string name, ExpressionList values) {
			this.expressionLists = new List<ExpressionList> {values};
			Name = name;
		}
		public VariableDeclaration(string name, IEnumerable<ExpressionList> expressionLists) {
			this.expressionLists = expressionLists.ToList();
			Name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}
	}
}