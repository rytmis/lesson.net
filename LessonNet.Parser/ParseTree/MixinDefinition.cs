using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinDefinition : Declaration {
		private readonly List<string> parameterNames;
		private readonly RuleBlock block;
		public SelectorList Selectors { get; }
		public int Arity => parameterNames.Count;
		public IReadOnlyCollection<string> Parameters => parameterNames.AsReadOnly();

		public MixinDefinition(SelectorList selectors, IEnumerable<string> parameterNames, RuleBlock block) {
			this.Selectors = selectors;
			this.parameterNames = parameterNames.ToList();
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterScope(Selectors)) {
				foreach (var generatedNode in block.Evaluate(context)) {
					yield return generatedNode;
				}
			}
		}

		public override void DeclareIn(Scope scope) {
			scope.DeclareMixin(this);
		}
	}

	public class MixinEvaluationResult : LessNode {
		private readonly MixinDefinition mixin;
		private readonly MixinCall call;
		private readonly Scope closure;

		public MixinEvaluationResult(MixinDefinition mixin, MixinCall call, Scope closure) {
			this.mixin = mixin;
			this.call = call;
			this.closure = closure;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			using (context.EnterClosureScope(closure)) {
				var arguments = mixin.Parameters
					.Zip(call.Arguments, (param, argument) => new VariableDeclaration(param, argument.Evaluate(context).Cast<ExpressionList>()));

				foreach (var argument in arguments) {
					context.CurrentScope.DeclareVariable(argument);
				}

				foreach (var evaluationResult in mixin.Evaluate(context)) {
					yield return evaluationResult;
				}
			}
		}
	}
}