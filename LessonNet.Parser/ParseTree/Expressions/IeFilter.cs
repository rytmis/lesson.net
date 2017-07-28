using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class IeFilter : Expression {
		public string Filter { get; }
		public IList<IeFilterExpression> FilterExpressions { get; }

		public IeFilter(string filter, IEnumerable<IeFilterExpression> filterExpressions) {
			Filter = filter;
			FilterExpressions = filterExpressions.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new IeFilter(Filter, FilterExpressions.Select(fe => fe.EvaluateSingle<IeFilterExpression>(context)));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("progid:");
			context.Append(Filter);
			context.Append('(');

			for (var index = 0; index < FilterExpressions.Count; index++) {
				var filterExpression = FilterExpressions[index];
				context.Append(filterExpression);

				if (index < FilterExpressions.Count - 1) {
					context.Append(", ");
				}
			}

			context.Append(')');
		}

		protected bool Equals(IeFilter other) {
			return string.Equals(Filter, other.Filter) && FilterExpressions.SequenceEqual(other.FilterExpressions);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((IeFilter) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 397;
				hashCode = (hashCode * 397) ^ (Filter != null ? Filter.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (FilterExpressions?.Aggregate(hashCode, (i, expr) => (i*397) ^ expr.GetHashCode()) ?? 0);
				return hashCode;
			}
		}
	}

	public class IeFilterExpression : LessNode {
		public Identifier Name { get; }
		public Expression Value { get; }

		public IeFilterExpression(Identifier name, Expression value) {
			Name = name;
			Value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new IeFilterExpression(Name.EvaluateSingle<Identifier>(context), Value.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Name);
			context.Append('=');
			context.Append(Value);
		}

		protected bool Equals(IeFilterExpression other) {
			return Equals(Name, other.Name) && Equals(Value, other.Value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((IeFilterExpression) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Value != null ? Value.GetHashCode() : 0);
			}
		}
	}
}
