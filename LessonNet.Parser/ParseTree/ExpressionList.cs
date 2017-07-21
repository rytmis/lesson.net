using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class ExpressionList : Expression {
		private readonly char separator;
		public IList<Expression> Values { get; }

		public bool IsCommaSeparated => separator == ',';

		public ExpressionList(Expression expr, char separator) : this(new[] {expr}, separator) { }

		public ExpressionList(IEnumerable<Expression> values, char separator) {
			this.separator = separator;
			this.Values = values.ToList();
		}

		public ExpressionList Flatten() {
			return new ExpressionList(FlattenCore(), separator);
		}

		private IEnumerable<Expression> FlattenCore() {
			if (Values.Count == 2 && Values[1] is ExpressionList cdr && cdr.separator == separator) {
				yield return Values[0];

				foreach (var flattened in cdr.FlattenCore()) {
					yield return flattened;
				}
			} else {
				foreach (var expression in Values) {
					yield return expression;
				}
			}
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

			var list = new ExpressionList(expressions, separator);
			yield return important ? (LessNode) new ImportantExpression(list) : list;
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(string.Join(GetSeparatorString(), Values.Select(v => v.ToCss())));
		}

		private string GetSeparatorString() {
			return !char.IsWhiteSpace(separator) ? separator + " " : " ";
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