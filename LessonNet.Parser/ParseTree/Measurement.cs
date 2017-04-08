using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree
{
	public class Measurement : Expression
	{
		private readonly string number;
		private readonly string unit;

		public Measurement(string number, string unit) {
			this.number = number;
			this.unit = unit;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return $"{number}{unit}";
		}
	}
}
