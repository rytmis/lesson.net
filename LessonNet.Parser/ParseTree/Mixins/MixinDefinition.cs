using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinDefinition : Declaration {
		private readonly List<MixinParameter> parameters;
		private readonly RuleBlock block;
		private readonly MixinGuard guard;
		public SelectorList Selectors { get; }
		public int Arity => parameters.Count;
		public IReadOnlyCollection<MixinParameter> Parameters => parameters.AsReadOnly();

		public MixinDefinition(SelectorList selectors, IEnumerable<MixinParameter> parameters, RuleBlock block, MixinGuard guard) {
			this.Selectors = selectors;
			this.parameters = parameters.ToList();
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

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareMixin(
				new MixinDefinition(Selectors.EvaluateSingle<SelectorList>(context), Parameters, block, guard));
		}

		public bool Guard(EvaluationContext context) {
			if (guard == null) {
				return true;
			}

			return guard.SatisfiedBy(context);
		}
	}

	public class MixinParameter : Declaration {
		public string Name { get; }
		public IList<ExpressionList> DefaultValue { get; }

		public MixinParameter(string name, IEnumerable<ExpressionList> defaultValue) {
			this.Name = name;
			this.DefaultValue = defaultValue?.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}

		public override void DeclareIn(EvaluationContext context) {
			if (DefaultValue != null) {
				context.CurrentScope.DeclareVariable(new VariableDeclaration(Name, DefaultValue));
			}
		}

		public bool HasDefaultValue => DefaultValue?.Count > 0;
	}
}