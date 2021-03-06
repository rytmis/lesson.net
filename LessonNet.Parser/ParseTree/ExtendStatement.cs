using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree {
	public class ExtendStatement : Statement {
		private readonly Extend extend;

		public ExtendStatement(Extend extend) {
			this.extend = extend;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var extension = extend.EvaluateSingle<Extend>(context);

			foreach (var extender in extension.Extenders) {
				foreach (var selector in context.CurrentScope.Selectors.Selectors) {
					context.Extensions.Add(extender, selector, context.IsReference);
				}
			}

			yield break;
		}
	}
}