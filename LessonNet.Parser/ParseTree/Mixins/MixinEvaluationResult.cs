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
				foreach (var mixinParameter in mixin.Parameters) {
					mixinParameter.DeclareIn(context);
				}

				(var namedArgs, var positionalArgs) = call.Arguments.Split<NamedArgument, PositionalArgument>();

				foreach (var namedArgument in namedArgs) {
					namedArgument.DeclareIn(context);
				}

				var arguments = mixin.Parameters
					.Zip(positionalArgs, (param, argument) => new VariableDeclaration(param.Name, argument.EvaluateSingle<PositionalArgument>(context).Value));

				foreach (var argument in arguments) {
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