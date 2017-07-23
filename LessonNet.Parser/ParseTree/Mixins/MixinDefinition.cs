using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins {
	public class MixinDefinition : Declaration {
		private readonly List<MixinParameterBase> parameters;
		private readonly RuleBlock block;
		private readonly MixinGuard guard;
		public bool IsDefaultOverload { get; }
		public Selector Selector { get; }
		public int Arity => parameters.Count;
		public IReadOnlyList<MixinParameterBase> Parameters => parameters.AsReadOnly();

		public MixinDefinition(Selector selector, IEnumerable<MixinParameterBase> parameters, RuleBlock block, MixinGuard guard) {
			// Combinators (descendant selectors etc.) do not count in mixin calls.
			// E.g. #id > .class is equivalent to #id .class
			this.Selector = selector.DropCombinators();
			this.parameters = parameters.ToList();
			this.block = block;
			this.guard = guard;
			this.IsDefaultOverload = guard is DefaultMixinGuard;

			var varargsCount = this.parameters.OfType<VarargsParameter>().Count();
			if (varargsCount > 1 ||varargsCount == 1 && !(this.parameters.Last() is VarargsParameter) ) {
				throw new ParserException("Only one varargs parameter at the end of the parameter list is allowed");
			}
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			foreach (var generatedNode in block.Evaluate(context)) {
				yield return generatedNode;
			}
		}

		public override Statement ForceImportant() {
			return new MixinDefinition(Selector, Parameters, block.ForceImportant(), guard);
		}

		public override void DeclareIn(EvaluationContext context) {
			context.CurrentScope.DeclareMixin(
				new MixinDefinition(Selector.EvaluateSingle<Selector>(context), Parameters, block, guard));
		}

		public bool Guard(EvaluationContext context, MixinGuardScope guardScope) {
			if (guard == null) {
				return true;
			}

			return guard.SatisfiedBy(context, guardScope);
		}

		protected override string GetStringRepresentation() {
			var guardStr = guard != null ? $" {guard}" : "";
			return $"{Selector} ({string.Join(",", Parameters)}){guardStr}";
		}
	}


	public abstract class MixinParameterBase : LessNode {
		public virtual bool HasDefaultValue => false;
	}

	public class MixinParameter : MixinParameterBase {
		public string Name { get; }
		public Expression DefaultValue { get; }

		public MixinParameter(string name, Expression defaultValue) {
			this.Name = name;
			this.DefaultValue = defaultValue;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
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
		public Expression Pattern { get; }

		public PatternMatchParameter(Expression pattern) {
			Pattern = pattern;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new PatternMatchParameter(Pattern.EvaluateSingle<Expression>(context));
		}

		protected override string GetStringRepresentation() {
			return Pattern.ToString();
		}
	}

	public class VarargsParameter : MixinParameterBase {
		public static readonly VarargsParameter Instance = new VarargsParameter();

		protected VarargsParameter() { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return "...";
		}
	}

	public class NamedVarargsParameter : VarargsParameter {
		public string Name { get; }

		public NamedVarargsParameter(string name) {
			Name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return $"@{Name}...";
		}
	}
}