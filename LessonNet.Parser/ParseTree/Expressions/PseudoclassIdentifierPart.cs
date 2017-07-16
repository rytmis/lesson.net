using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class PseudoclassIdentifierPart : IdentifierPart {
		private readonly string prefix;
		private readonly string identifier;
		private readonly Expression value;

		public PseudoclassIdentifierPart(string prefix, string identifier, Expression value) {
			this.prefix = prefix;
			this.identifier = identifier;
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			Expression EvaluateValue() {
				if (value == null) {
					return null;
				}

				var evaluated = value.EvaluateSingle<Expression>(context);
				if (evaluated is ListOfExpressionLists list) {
					return list.Single<Expression>();
				}

				return evaluated;
			}

			var expressionValue = value != null
				? $"({EvaluateValue()})"
				: "";

			yield return new ConstantIdentifierPart(prefix + identifier + expressionValue);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(prefix);
			context.Append(identifier);

			if (value != null) {
				context.Append('(');
				context.Append(value);
				context.Append(')');
			}
		}
	}
}