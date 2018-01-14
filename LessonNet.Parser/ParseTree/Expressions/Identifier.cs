using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Identifier : Expression {
		public IReadOnlyList<IdentifierPart> Parts { get; }

		public Identifier(IdentifierPart part) : this(new[] {part}) {
			
		}
		public Identifier(IEnumerable<IdentifierPart> parts) {
			this.Parts = parts.ToList().AsReadOnly();
		}

		public IdentifierPart this[int index] => Parts[index];

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			string identifier = string.Join("", Parts.Select(p => p.EvaluateSingle<ConstantIdentifierPart>(context)));

			yield return new Identifier(new ConstantIdentifierPart(identifier));
		}

		protected bool Equals(Identifier other) {
			return Parts.SequenceEqual(other.Parts);
		}

		public override bool Equals(object obj) {
			if (Object.ReferenceEquals(null, obj)) return false;
			if (Object.ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Identifier) obj);
		}

		public override int GetHashCode() {
			return Parts.Aggregate(37, (s, p) => s * p.GetHashCode());
		}

		protected override string GetStringRepresentation() {
			return string.Join("", Parts);
		}

		public override void WriteOutput(OutputContext context) {
			foreach (var identifierPart in Parts) {
				context.Append(identifierPart);
			}
		}

		public Identifier CombineConstantIdentifiers(Identifier another) {
			if (Parts.Count == 1 && Parts[0] is ConstantIdentifierPart cip1 && another.Parts.Count == 1 && another.Parts[0] is ConstantIdentifierPart cip2) {
				return new Identifier(new ConstantIdentifierPart(cip1.Value + cip2.Value));
			}

			throw new InvalidOperationException("Combining is only implemented for single-part constant identifiers");
		}
	}
}