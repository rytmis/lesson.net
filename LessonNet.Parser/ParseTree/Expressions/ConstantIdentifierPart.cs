using System;
using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class ConstantIdentifierPart : IdentifierPart {
		private readonly string part;

		public ConstantIdentifierPart(string part) {
			this.part = part;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return part;
		}

		protected bool Equals(ConstantIdentifierPart other) {
			return string.Equals((string) part, (string) other.part);
		}

		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(null, obj)) return false;
			if (Object.ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ConstantIdentifierPart) obj);
		}

		public override int GetHashCode() {
			return (part != null ? part.GetHashCode() : 0);
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(part, indent: false);
		}
	}
}