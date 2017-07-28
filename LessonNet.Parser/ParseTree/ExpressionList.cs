using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class ExpressionList : Expression {
		public char Separator { get; }
		public IList<Expression> Values { get; }

		public bool IsCommaSeparated => Separator == ',';

		public ExpressionList(Expression expr, char separator) : this(new[] {expr}, separator) { }

		public ExpressionList(IEnumerable<Expression> values, char separator) {
			this.Separator = separator;
			this.Values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			bool important = false;

			IEnumerable<Expression> EvaluateExpressions() {
				foreach (var expression in Values) {
					foreach (var evaluatedExpression in expression.Evaluate(context).Cast<Expression>()) {
						if (evaluatedExpression is ImportantExpression impExp) {
							important = true;
							yield return impExp.Value;
						} else {
							yield return evaluatedExpression;
						}
					}
				}
			}

			// Materializing this enumerable also sets important to true when applicable
			var expressions = EvaluateExpressions().ToList();

			var list = new ExpressionList(expressions, Separator);
			yield return important ? (LessNode) new ImportantExpression(list) : list;
		}

		public override void WriteOutput(OutputContext context) {
			string sep = GetSeparatorString();
			for (var index = 0; index < Values.Count; index++) {
				var expression = Values[index];
				context.Append(expression);

				if (index < Values.Count - 1) {
					context.Append(sep);
				}
			}
		}

		private string GetSeparatorString() {
			return !char.IsWhiteSpace(Separator) ? Separator + " " : " ";
		}

		protected override string GetStringRepresentation() {
			return string.Join(GetSeparatorString(), Values);
		}

		protected bool Equals(ExpressionList other) {
			return Values.SequenceEqual(other.Values);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ExpressionList) obj);
		}

		public override int GetHashCode() {
			return (Values != null ? Values.Aggregate(1, (h, e) => (h * 397) ^ e.GetHashCode()) : 0);
		}
	}
}