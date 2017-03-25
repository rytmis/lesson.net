using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class Rule : LessNode {
		private readonly string property;
		private List<ExpressionList> values;

		public Rule(string property, IEnumerable<ExpressionList> values) {
			this.property = property;
			this.values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetCss() {
			string cssValues = string.Join(", ", this.values.Select(v => v.ToCss()));

			return $"{property}: {cssValues};";
		}
	}
}