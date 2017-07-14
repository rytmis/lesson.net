using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Url : Expression {
		private readonly LessString str;
		private readonly Variable var;
		private readonly string url;

		public Url(LessString str) {
			this.str = str;
		}

		public Url(Variable var) {
			this.var = var;
		}

		public Url(string url) {
			this.url = url;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			if (var != null) {
				yield return new Url(var.EvaluateSingle<ListOfExpressionLists>(context).Single<LessString>());
			} else if (str != null) {
				yield return new Url(str.EvaluateSingle<LessString>(context));
			} else {
				yield return this;
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("url(");

			if (str != null) {
				context.Append(str);
			} else {
				context.Append(url);
			}

			context.Append(")");
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