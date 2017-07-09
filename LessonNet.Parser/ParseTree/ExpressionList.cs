using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class ListOfExpressionLists : Expression, IEnumerable<ExpressionList> {
		private readonly char separator;
		private readonly IList<ExpressionList> expressionLists;

		public int Count => expressionLists.Count;
		public ExpressionList this[int index] => expressionLists[index];

		public ListOfExpressionLists(IEnumerable<ExpressionList> expressionLists, char separator) {
			this.separator = separator;
			this.expressionLists = expressionLists.ToList();
		}

		public ListOfExpressionLists(ExpressionList expressionList, char separator) : this(new []{expressionList}, separator) { }
		public ListOfExpressionLists(Expression expression, char separator) : this(new ExpressionList(new []{expression}), separator) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new ListOfExpressionLists(expressionLists.Select(l => l.EvaluateSingle<ExpressionList>(context)), separator);
		}

		public override void WriteOutput(OutputContext context) {
			string sep = this.separator != ' ' ? this.separator + " " : this.separator.ToString();

			context.Append(string.Join(sep, expressionLists.Select(el => el.ToCss())));
		}

		protected bool Equals(ListOfExpressionLists other) {
			return separator == other.separator && expressionLists.SequenceEqual(other.expressionLists);
		}

		public IEnumerator<ExpressionList> GetEnumerator() {
			return expressionLists.GetEnumerator();
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ListOfExpressionLists) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (separator.GetHashCode() * 397) ^ (expressionLists != null
					? expressionLists.Aggregate(397, (h, el) => (h * 397) ^ el.GetHashCode())
					: 0);
			}
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return ((IEnumerable) expressionLists).GetEnumerator();
		}
	}

	public class ExpressionList : LessNode {
		public IList<Expression> Values { get; }

		public ExpressionList(Expression expr) {
			Values = new List<Expression>() {expr};
		}
		public ExpressionList(IEnumerable<Expression> values) {
			this.Values = values.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<Expression> EvaluateExpressions() {
				foreach (var expression in Values) {
					foreach (var evaluatedExpression in expression.Evaluate(context)) {
						if (evaluatedExpression is ExpressionList list) {
							foreach (var evaluatedValue in list.Values) {
								yield return evaluatedValue;
							}
						} else if (evaluatedExpression is Expression expr) {
							yield return expr;
						} else {
							throw new EvaluationException($"Expected {nameof(ExpressionList)}, got {evaluatedExpression.GetType().Name}");
						}
					}
				}
			}

			yield return new ExpressionList(EvaluateExpressions());
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(string.Join(" ", Values.Select(v => v.ToCss())));
		}

		protected override string GetStringRepresentation() {
			return string.Join(", ", Values);
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