using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class Fraction : Expression {
		public decimal Numerator { get; }
		public decimal Denominator { get; }
		public string Unit { get; }

		public Fraction(decimal numerator, decimal denominator, string unit) {
			Numerator = numerator;
			Denominator = denominator;
			Unit = unit;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(((int)Numerator).ToString());
			context.Append("/");
			context.Append(((int)Denominator).ToString());

			if (!string.IsNullOrEmpty(Unit)) {
				context.Append((string) Unit);
			}
		}
	}
}