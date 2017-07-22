using System.Diagnostics;

namespace LessonNet.Parser.ParseTree.Mixins {
	public abstract class MixinGuard : NoOutputNode {
		public abstract bool SatisfiedBy(EvaluationContext context, MixinGuardScope guardScope);
	}

	public class ConditionMixinGuard : MixinGuard {
		private readonly OrConditionList conditions;

		public ConditionMixinGuard(OrConditionList conditions) {
			this.conditions = conditions;
		}

		public override bool SatisfiedBy(EvaluationContext context, MixinGuardScope guardScope) {
			guardScope.SetConditionGuard();

			return conditions.SatisfiedBy(context);
		}
	}

	public class DefaultMixinGuard : MixinGuard {
		public static readonly MixinGuard Instance = new DefaultMixinGuard();
		protected DefaultMixinGuard() { }

		public override bool SatisfiedBy(EvaluationContext context, MixinGuardScope guardScope) {
			if (guardScope.DefaultGuardMatched) {
				throw new EvaluationException("Ambiguous use of default()");
			}

			if (guardScope.ConditionGuardMatched) {
				return false;
			}

			guardScope.SetDefaultGuard();

			return true;
		}
	}

	public class MixinGuardScope {
		public bool ConditionGuardMatched { get; private set; }
		public bool DefaultGuardMatched { get; private set; }

		public void SetConditionGuard() {
			ConditionGuardMatched = true;
		}
		public void SetDefaultGuard() {
			ConditionGuardMatched = true;
		}
	}
}