using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class EFunction : LessFunction {
		public EFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments) {
			var str = arguments as LessString;
			if (str == null) {
				throw new EvaluationException("Argument must be a string");
			}

			return new Identifier(new ConstantIdentifierPart(str.GetUnquotedValue()));
		}
	}

	[FunctionName("formatString")]
	public class FormatStringFunction : LessFunction {
		public FormatStringFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments) {
			if (arguments is LessString onlyStr) {
				var formatted = string.Format(onlyStr.GetUnquotedValue());

				return new QuotedExpression(LessString.FromString(formatted));
			}

			if (arguments is ExpressionList list && list.IsCommaSeparated && list.Values[0] is LessString str) {
				var formatted = string.Format(str.GetUnquotedValue(), ConvertToStrings(list.Values.Skip(1)).ToArray<object>());

				return new QuotedExpression(LessString.FromString(formatted));
			}

			throw new EvaluationException("First argument must be a string");
		}

		private IEnumerable<string> ConvertToStrings(IEnumerable<Expression> expressions) {
			foreach (var expression in expressions) {
				if (expression is LessString str) {
					yield return str.GetUnquotedValue();
				} else {
					yield return expression.ToString();
				}
			}
		}
	}
}
