using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class FontSizeShorthand : Expression {
		public Expression FontSize { get; }
		public Expression LineHeight { get; }

		public FontSizeShorthand(Expression fontSize, Expression lineHeight) {
			FontSize = fontSize;
			LineHeight = lineHeight;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new FontSizeShorthand(FontSize.EvaluateSingle<Expression>(context), LineHeight.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(FontSize);
			context.Append("/");
			context.Append(LineHeight);
		}


		protected bool Equals(FontSizeShorthand other) {
			return Equals(FontSize, other.FontSize) 
				&& Equals(LineHeight, other.LineHeight);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((FontSizeShorthand) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (FontSize != null ? FontSize.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (LineHeight != null ? LineHeight.GetHashCode() : 0);
				return hashCode;
			}
		}
	}
}