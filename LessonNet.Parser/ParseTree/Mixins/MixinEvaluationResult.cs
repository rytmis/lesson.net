using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinEvaluationResult : InvocationResult {
		private readonly MixinDefinition mixin;
		private readonly MixinCall call;
		private readonly Scope closure;

		public bool Matched { get; private set; }

		public MixinEvaluationResult(MixinDefinition mixin, MixinCall call, Scope closure) {
			this.mixin = mixin;
			this.call = call;
			this.closure = closure;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				var namedParameters = mixin.Parameters.OfType<MixinParameter>().ToList();
				foreach (var mixinParameter in namedParameters) {
					mixinParameter.DeclareIn(context);
				}

				(var namedArgs, var positionalArgs) = call.Arguments.Split<NamedArgument, PositionalArgument>();

				foreach (var namedArgument in namedArgs) {
					namedArgument.DeclareIn(context);
				}

				var parameterArgumentPairs = mixin.Parameters
					.Zip(positionalArgs, (param, argument) => new { Parameter = param, Argument = argument })
					.Where(pair => !(pair.Parameter is PatternMatchParameter))
					.Select(pair => {
						var param = (MixinParameter) pair.Parameter;

						return new VariableDeclaration(param.Name, pair.Argument.EvaluateSingle<PositionalArgument>(context).Value);
					});

				foreach (var argument in parameterArgumentPairs) {
					context.CurrentScope.DeclareVariable(argument);
				}

				if (mixin.Guard(context)) {
					foreach (var evaluationResult in mixin.Evaluate(context)) {
						yield return evaluationResult;
					}

					Matched = true;
				}
			}
		}
	}
}