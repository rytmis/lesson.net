using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions
{
	public class EscapeSequence : Expression

	{
		private readonly string escaped;

		public EscapeSequence(string escaped) {
			this.escaped = escaped;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(escaped);
		}
		protected bool Equals(EscapeSequence other) {
			return string.Equals(escaped, other.escaped);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((EscapeSequence) obj);
		}

		public override int GetHashCode() {
			return escaped?.GetHashCode() ?? 0;
		}
	}
}
