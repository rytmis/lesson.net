using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinEvaluationResult : InvocationResult {
		private readonly MixinDefinition mixin;
		private readonly MixinCall call;
		private readonly Scope closure;
		private readonly MixinGuardScope guardScope;

		public bool Matched { get; private set; }

		public MixinEvaluationResult(MixinDefinition mixin, MixinCall call, Scope closure, MixinGuardScope guardScope) {
			this.mixin = mixin;
			this.call = call;
			this.closure = closure;
			this.guardScope = guardScope;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				// Evaluate all arguments prior to declaring default values and such in the scope
				var evaluatedArgs = call.Arguments.Select(arg => arg.EvaluateSingle<MixinCallArgument>(context)).ToArray();

				var namedParameters = mixin.Parameters.OfType<MixinParameter>().ToArray();

				var varargs = evaluatedArgs.Skip(namedParameters.Length).Select(arg => arg.Value).ToArray(); 

				// Resolve named parameters from variables so that default argument values re taken into account
				var allArguments = namedParameters
					.Select(p => new Variable(p.Name))
					.Concat(varargs);

				context.CurrentScope.DeclareVariable(new VariableDeclaration("arguments", new ExpressionList(allArguments, ' ')));

				foreach (var mixinParameter in namedParameters) {
					mixinParameter.DeclareIn(context);
				}

				if (mixin.Parameters.Count > 0 && mixin.Parameters.Last() is NamedVarargsParameter namedVarags) {
					context.CurrentScope.DeclareVariable(new VariableDeclaration(namedVarags.Name, new ExpressionList(varargs, ' ')));
				}

				(var namedArgs, var positionalArgs) = evaluatedArgs.Split<NamedArgument, PositionalArgument>();

				foreach (var namedArgument in namedArgs) {
					namedArgument.DeclareIn(context);
				}

				var parameterArgumentPairs = mixin.Parameters
					.Zip(positionalArgs, (param, argument) => new { Parameter = param, Argument = argument })
					.Where(pair => pair.Parameter is MixinParameter)
					.Select(pair => {
						var param = (MixinParameter) pair.Parameter;

						return new VariableDeclaration(param.Name, pair.Argument.EvaluateSingle<PositionalArgument>(context).Value);
					});

				foreach (var argument in parameterArgumentPairs) {
					context.CurrentScope.DeclareVariable(argument);
				}

				if (mixin.Guard(context, guardScope)) {
					foreach (var evaluationResult in mixin.Evaluate(context)) {
						yield return evaluationResult;
					}

					Matched = true;
				}
			}
		}
	}
}