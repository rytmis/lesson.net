using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinDefinition : Declaration {
		private readonly List<MixinParameterBase> parameters;
		private readonly RuleBlock block;
		private readonly MixinGuard guard;
		public SelectorList Selectors { get; }
		public int Arity => parameters.Count;
		public IReadOnlyList<MixinParameterBase> Parameters => parameters.AsReadOnly();

		public MixinDefinition(SelectorList selectors, IEnumerable<MixinParameterBase> parameters, RuleBlock block, MixinGuard guard) {
			// Combinators (descendant selectors etc.) do not count in mixin calls.
			// E.g. #id > .class is equivalent to #id .class
			this.Selectors = selectors.DropCombinators();
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

		protected override string GetStringRepresentation() {
			return $"{Selectors} ({string.Join(",", Parameters)})";
		}
	}


	public abstract class MixinParameterBase : LessNode {
		public virtual bool HasDefaultValue => false;
	}

	public class MixinParameter : MixinParameterBase {
		public string Name { get; }
		public ListOfExpressionLists DefaultValue { get; }

		public MixinParameter(string name, ListOfExpressionLists defaultValue) {
			this.Name = name;
			this.DefaultValue = defaultValue;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}

		public void DeclareIn(EvaluationContext context) {
			if (DefaultValue != null) {
				context.CurrentScope.DeclareVariable(new VariableDeclaration(Name, DefaultValue));
			}
		}

		public override bool HasDefaultValue => DefaultValue != null;

		protected override string GetStringRepresentation() {
			if (HasDefaultValue) {
				return $"@{Name}: {string.Join(",", DefaultValue)}";
			}

			return $"@{Name}";
		}
	}

	public class PatternMatchParameter : MixinParameterBase {
		public Identifier Identifier { get; }

		public PatternMatchParameter(Identifier identifier) {
			this.Identifier = identifier;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return  new PatternMatchParameter(Identifier.EvaluateSingle<Identifier>(context));
		}

		protected override string GetStringRepresentation() {
			return Identifier.ToString();
		}
	}
}