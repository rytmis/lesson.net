using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinCall : Statement {
		private readonly SelectorList selectors;
		private readonly List<MixinCallArgument> arguments;
		public IReadOnlyCollection<MixinCallArgument> Arguments => arguments.AsReadOnly();

		public MixinCall(SelectorList selectors, IEnumerable<MixinCallArgument> arguments) {
			this.selectors = selectors;
			this.arguments = arguments.ToList();

			VerifyArgumentOrder();
		}

		private void VerifyArgumentOrder() {
			bool seenNamedArgument = false;
			foreach (var argument in arguments) {
				if (argument is NamedArgument) {
					seenNamedArgument = true;
				} else if (seenNamedArgument) {
					throw new EvaluationException("Invalid mixin call: named arguments must follow positional arguments.");
				}
			}
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var call = new MixinCall(selectors.EvaluateSingle<SelectorList>(context), arguments);

			foreach (var mixinResult in context.CurrentScope.ResolveMatchingMixins(call)) {
				foreach (var evaluationResult in mixinResult.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}

		public bool Matches(MixinDefinition mixinDefinition, EvaluationContext context) {
			if (!mixinDefinition.Selectors.MatchesAny(selectors)) {
				// Selectors don't match
				return false;
			}

			if (mixinDefinition.Parameters.Count < arguments.Count) {
				// No match: too many arguments
				return false;
			}

			var positionalArguments = arguments.OfType<PositionalArgument>().ToList();

			if (!PatternMatch(context, positionalArguments, mixinDefinition.Parameters)) {
				return false;
			}

			var namedArguments = arguments.OfType<NamedArgument>().ToList();

			var remainingParameters = mixinDefinition.Parameters.Skip(positionalArguments.Count).Cast<MixinParameter>().ToList();

			var matchedParams = remainingParameters
				.Where(p => namedArguments.Any(arg => string.Equals(p.Name, arg.ParameterName, StringComparison.OrdinalIgnoreCase)))
				.ToList();

			if (matchedParams.Count != namedArguments.Count) {
				// No match: one or more named arguments had a name that doesn't match a parameter
				return false;
			}

			// True if any remaining parameters have a default value
			return remainingParameters.Except(matchedParams).All(p => p.HasDefaultValue);
		}

		private bool PatternMatch(EvaluationContext context, List<PositionalArgument> positionalArguments, IReadOnlyList<MixinParameterBase> mixinDefinitionParameters) {
			for (var i = 0; i < mixinDefinitionParameters.Count; i++) {
				var param = mixinDefinitionParameters[i];

				if (!(param is PatternMatchParameter ip)) {
					continue;
				}

				// We don't have a positional argument that matches this position,
				// so it's an automatic fail
				if (positionalArguments.Count <= i) {
					return false;
				}

				// See if the argument evalutes to an Identifier that matches the pattern match identifier
				var identifierArgumentValue = positionalArguments[i].EvaluateSingleValue<Identifier>(context);
				if (!ip.Identifier.Equals(identifierArgumentValue)) {
					return false;
				}
			}
			return true;
		}

		protected override string GetStringRepresentation() {
			return $"{selectors}({string.Join(", ", arguments)})";
		}
	}
}