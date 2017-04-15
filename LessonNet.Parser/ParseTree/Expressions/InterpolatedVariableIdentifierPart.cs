using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class InterpolatedVariableIdentifierPart : IdentifierPart {
		private readonly string variableName;

		public InterpolatedVariableIdentifierPart(string variableName) {
			this.variableName = variableName;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<Expression> EvaluateVariable() {
				var variable = context.CurrentScope.ResolveVariable(variableName);
				foreach (var expressionList in variable.Values) {
					foreach (var expression in expressionList) {
						yield return expression.EvaluateSingle<Expression>(context);
					}
				}
			}

			yield return new ConstantIdentifierPart(string.Join("", EvaluateVariable()));
		}

		protected override string GetStringRepresentation() {
			return $"@{{{variableName}}}";
		}
	}
}