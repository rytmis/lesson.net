using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinCall : Statement {
		public bool Important { get; }
		public Selector Selector { get; }
		private readonly List<MixinCallArgument> arguments;
		public IReadOnlyCollection<MixinCallArgument> Arguments => arguments.AsReadOnly();

		public MixinCall(Selector selector, IEnumerable<MixinCallArgument> arguments, bool important) {
			this.Important = important;
			// Combinators (descendant selectors etc.) do not count in mixin calls.
			// E.g. #id > .class is equivalent to #id .class
			this.Selector = selector.DropCombinators();
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
			var call = new MixinCall(Selector.EvaluateSingle<Selector>(context), arguments, Important);

			foreach (var mixinResult in context.CurrentScope.ResolveMatchingMixins(call)) {
				foreach (var evaluationResult in mixinResult.Evaluate(context).Cast<Statement>()) {
					if (Important) {
						yield return evaluationResult.ForceImportant();
					} else {
						yield return evaluationResult;
					}
				}
			}
		}

		public override Statement ForceImportant() {
			if (Important) {
				return this;
			}

			return new MixinCall(Selector, arguments, important: true);
		}

		public bool Matches(MixinDefinition mixinDefinition, EvaluationContext context) {
			if (!mixinDefinition.Selector.Equals(Selector)) {
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
				var patternArgumentValue = positionalArguments[i].Value.EvaluateSingle<Expression>(context);
				if (!ip.Pattern.Equals(patternArgumentValue)) {
					return false;
				}
			}
			return true;
		}

		protected override string GetStringRepresentation() {
			return $"{Selector}({string.Join(", ", arguments)})";
		}
	}
}