using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
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
}