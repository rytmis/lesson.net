using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class VariableDeclaration : Declaration {
		public Expression Value { get; }
		public string Name { get; }

		public VariableDeclaration(string name, Expression value) {
			this.Value = value;
			Name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			return Enumerable.Empty<LessNode>();
		}

		public override Statement ForceImportant() {
			if (Value is ImportantExpression) {
				return this;
			}

			return new VariableDeclaration(Name, new ImportantExpression(Value));
		}

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareVariable(this);
		}
	}
}