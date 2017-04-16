using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Identifier : Expression {
		private readonly IList<IdentifierPart> parts;

		public Identifier(IdentifierPart part) : this(new[] {part}) {
			
		}
		public Identifier(IEnumerable<IdentifierPart> parts) {
			this.parts = parts.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			string identifier = string.Join("", parts.Select(p => p.EvaluateSingle<ConstantIdentifierPart>(context)));

			yield return new Identifier(new ConstantIdentifierPart(identifier));
		}

		protected bool Equals(Identifier other) {
			return parts.SequenceEqual(other.parts);
		}

		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(null, obj)) return false;
			if (Object.ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Identifier) obj);
		}

		public override int GetHashCode() {
			return parts.Aggregate(37, (s, p) => s * p.GetHashCode());
		}

		protected override string GetStringRepresentation() {
			return string.Join("", parts);
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();

			foreach (var identifierPart in parts) {
				context.Append(identifierPart);
			}
		}
	}
}