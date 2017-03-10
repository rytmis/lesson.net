using System;
using System.Collections.Generic;
using System.Text;

namespace LessonNet.Parser.SyntaxTree
{
	public abstract class LessNode {
		
		private Lazy<string> toStringCache;
		private Lazy<string> toCssCache;

		protected LessNode()
		{
			toStringCache = new Lazy<string>(GetStringRepresentation);
			toCssCache = new Lazy<string>(GetCss);
		}

		protected virtual string GetStringRepresentation() {
			return $"{GetType().Name}";
		}

		protected virtual string GetCss() {
			return "";
		}

		public string ToCss() => toCssCache.Value;


		public override string ToString() => toStringCache.Value;

		public string ToCss(EvaluationContext context) {
			StringBuilder builder = new StringBuilder();

			foreach (var childNode in Evaluate(context)) {
				builder.Append(childNode.ToCss());
			}

			return builder.ToString();
		}

		public abstract IEnumerable<LessNode> Evaluate(EvaluationContext context);
	}
}