using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree
{
	public class Measurement : Expression
	{
		public decimal Number { get; }
		public string Unit { get; }

		public Measurement(decimal number, string unit) {
			this.Number = number;
			this.Unit = unit;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return $"{Number}{Unit}";
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(GetStringRepresentation());
		}
	}
}
