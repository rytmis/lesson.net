using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Url : Expression {
		public LessString StringContent { get; }
		public Variable VariableContent { get; }
		public string RawUrl { get; }

		public Url(LessString str) {
			this.StringContent = str;
		}

		public Url(Variable var) {
			this.VariableContent = var;
		}

		public Url(string url) {
			this.RawUrl = url;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			if (VariableContent != null) {
				yield return new Url(VariableContent.EvaluateSingle<LessString>(context));
			} else if (StringContent != null) {
				yield return new Url(StringContent.EvaluateSingle<LessString>(context));
			} else {
				yield return this;
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("url(");

			if (StringContent != null) {
				context.Append(StringContent);
			} else if (RawUrl != null) {
				context.Append(RawUrl);
			}

			context.Append(")");
		}

		protected bool Equals(Url other) {
			return string.Equals(RawUrl, other.RawUrl);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Url) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (RawUrl != null ? RawUrl.GetHashCode() : 0);
		}
	}
}