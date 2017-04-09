using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinDefinition : Declaration {
		private readonly List<string> parameterNames;
		private readonly RuleBlock block;
		private readonly MixinGuard guard;
		public SelectorList Selectors { get; }
		public int Arity => parameterNames.Count;
		public IReadOnlyCollection<string> Parameters => parameterNames.AsReadOnly();

		public MixinDefinition(SelectorList selectors, IEnumerable<string> parameterNames, RuleBlock block, MixinGuard guard) {
			this.Selectors = selectors;
			this.parameterNames = parameterNames.ToList();
			this.block = block;
			this.guard = guard;
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

		public bool Guard(EvaluationContext context) {
			return guard.SatisfiedBy(context);
		}
	}

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
					.Zip(call.Arguments, (param, argument) => new VariableDeclaration(param, argument.Evaluate(context).Cast<ExpressionList>()));

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

	public class MixinGuard : NoOutputNode {
		private readonly OrConditionList conditions;

		public MixinGuard(OrConditionList conditions) {
			this.conditions = conditions;
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditions.SatisfiedBy(context);
		}
	}

	public class OrConditionList : NoOutputNode {
		private readonly IList<AndConditionList> conditionLists;

		public OrConditionList(IEnumerable<AndConditionList> conditionLists) {
			this.conditionLists = conditionLists.ToList();
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditionLists.Any(cl => cl.SatisfiedBy(context));
		}
	}

	public class AndConditionList : NoOutputNode {
		private readonly IList<Condition> conditions;

		public AndConditionList(IEnumerable<Condition> conditions) {
			this.conditions = conditions.ToList();
		}

		public bool SatisfiedBy(EvaluationContext context) {
			return conditions.All(c => c.SatisfiedBy(context));
		}
	}

	public abstract class Condition : NoOutputNode {
		public abstract bool SatisfiedBy(EvaluationContext context);
	}

	public class BooleanExpressionCondition : Condition {
		private readonly bool negate;
		private readonly Expression expression;
		public BooleanExpressionCondition(bool negate, Expression expression) {
			this.negate = negate;
			this.expression = expression;
		}


		public override bool SatisfiedBy(EvaluationContext context) {
			bool result = expression.EvaluateSingle<BooleanValue>(context).Value;
			return negate ? !result : result;
		}
	}

	public class ComparisonCondition : Condition {
		private readonly bool negate;
		private MathOperation comparisonOperation;

		public ComparisonCondition(bool negate, Expression lhs, string comparison, Expression rhs) {
			this.negate = negate;
			this.comparisonOperation = new MathOperation(lhs, comparison, rhs);
		}

		public override bool SatisfiedBy(EvaluationContext context) {
			var result = comparisonOperation.EvaluateSingle<BooleanValue>(context).Value;
			return negate ? !result : result;
		}
	}
}