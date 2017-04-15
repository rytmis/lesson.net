using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinEvaluationResult : LessNode {
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
				var arguments = mixin.Parameters
					.Zip(call.Arguments, 
						(param, argument) => new VariableDeclaration(param.Name, argument.Evaluate(context).Cast<ExpressionList>()));

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