using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Url : Expression {
		private readonly string url;

		public Url(string url) {
			this.url = url;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append($"url({url})");
		}

		protected bool Equals(Url other) {
			return string.Equals(url, other.url);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Url) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (url != null ? url.GetHashCode() : 0);
		}
	}
}