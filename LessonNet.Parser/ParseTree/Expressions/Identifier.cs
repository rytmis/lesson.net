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

		public IdentifierPart this[int index] => parts[index];

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
			foreach (var identifierPart in parts) {
				context.Append(identifierPart);
			}
		}

		public Identifier CombineConstantIdentifiers(Identifier another) {
			if (parts.Count == 1 && parts[0] is ConstantIdentifierPart cip1 && another.parts.Count == 1 && another.parts[0] is ConstantIdentifierPart cip2) {
				return new Identifier(new ConstantIdentifierPart(cip1.Value + cip2.Value));
			}

			throw new InvalidOperationException("Combining is only implemented for single-part constant identifiers");
		}
	}
}