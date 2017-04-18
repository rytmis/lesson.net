using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

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
			return $"{Number.ToString(CultureInfo.InvariantCulture)}{Unit}";
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(GetStringRepresentation());
		}
	}
}
