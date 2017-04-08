using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class Rule : LessNode {
		private readonly string property;
		private List<ExpressionList> values;

		public Rule(string property, IEnumerable<ExpressionList> values) {
			this.property = property;
			this.values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<ExpressionList> EvaluateValues() {
				foreach (var value in values) {
					yield return value.EvaluateSingle<ExpressionList>(context);
				}
			}

			yield return new Rule(property, EvaluateValues()) {IsEvaluated = true};
		}

		protected override string GetCss() {
			string cssValues = string.Join(", ", this.values.Select(v => v.ToCss()));

			return $"\t{property}: {cssValues};";
		}
	}
}