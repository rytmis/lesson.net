using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Url : Expression {
		public Expression Content { get; }
		public Url(Expression content) {
			Content = content;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Url(Content.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("url(");
			context.Append(Content);
			context.Append(")");
		}

		protected bool Equals(Url other) {
			return Equals(Content, other.Content);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Url) obj);
		}

		public override int GetHashCode() {
			return Content?.GetHashCode() ?? 0;
		}
	}
}