using System;
using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree {

	public abstract class Expression : LessNode {

	}

	public class Variable : Expression {
		private readonly string name;

		public Variable(string name) {
			this.name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var expressionList in context.CurrentScope.ResolveVariable(name).Values) {
				yield return expressionList.EvaluateSingle<ExpressionList>(context);
			}
		}

		protected override string GetStringRepresentation() {
			return $"@{name}";
		}
	}

	public class Identifier : Expression {
		private readonly string name;

		public Identifier(string name) {
			this.name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return name;
		}

		protected override string GetCss() {
			return name;
		}
	}
}