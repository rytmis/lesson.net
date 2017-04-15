using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class Rule : Statement {
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

		public override void WriteOutput(OutputContext context) {
			string cssValues = string.Join(", ", this.values.Select(v => v.ToCss()));

			context.AppendLine($"{property}: {cssValues};");
		}

		protected override string GetStringRepresentation() {
			return $"{property}: {string.Join(", ", values)}";
		}
	}
}