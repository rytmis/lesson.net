using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinCall : Statement {
		private readonly SelectorList selectors;
		private readonly List<MixinCallArgument> arguments;
		public IReadOnlyCollection<MixinCallArgument> Arguments => arguments.AsReadOnly();

		public MixinCall(SelectorList selectors, IEnumerable<MixinCallArgument> arguments) {
			this.selectors = selectors;
			this.arguments = arguments.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var mixinResult in context.CurrentScope.ResolveMatchingMixins(this)) {
				foreach (var evaluationResult in mixinResult.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}

		public bool Matches(MixinDefinition mixinDefinition) {
			return mixinDefinition.Arity == arguments.Count
				&& mixinDefinition.Selectors.MatchesAny(selectors);
		}

		protected override string GetStringRepresentation() {
			return $"{selectors}({string.Join(", ", arguments)})";
		}
	}

	public class MixinCallArgument : LessNode {
		private readonly ListOfExpressionLists value;

		public MixinCallArgument(ExpressionList expressionList) {
			value = new ListOfExpressionLists(new []{expressionList});
		}

		public MixinCallArgument(ListOfExpressionLists value) {
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var expressionList in value.Evaluate(context)) {
				yield return expressionList;
			}
		}

		protected override string GetStringRepresentation() {
			return value.ToString();
		}
	}
}