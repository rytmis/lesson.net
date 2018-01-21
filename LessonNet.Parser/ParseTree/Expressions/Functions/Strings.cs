using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class EFunction : LessFunction {
		public EFunction(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments, EvaluationContext context) {
			if (arguments is LessString str) {
				return new Identifier(new ConstantIdentifierPart(str.GetUnquotedValue()));
			}

			return arguments;
		}
	}

	[FunctionName("%")]
	public class FormatStringFunction : LessFunction {
		public FormatStringFunction(Expression arguments) : base(arguments) { }
		protected override Expression EvaluateFunction(Expression arguments, EvaluationContext context) {
			if (arguments is LessString onlyStr) {
				return onlyStr;
			}

			if (arguments is ExpressionList list && list.IsCommaSeparated && list.Values[0] is LessString formatStr) {
				var formatted = ReplacePlaceholders(formatStr.GetUnquotedValue(), list.Values.Skip(1).ToArray());

				return LessString.FromString(formatted, formatStr.QuoteChar);
			}

			throw new EvaluationException("First argument must be a string");
		}

		private string ReplacePlaceholders(string formatString, Expression[] items) {
			return Regex.Replace(
				formatString,
				"%(d|a|s)",
				MakeEvaluator(),
				RegexOptions.IgnoreCase);

			MatchEvaluator MakeEvaluator() {
				int replacementItemIndex = 0;

				return match => {
					if (replacementItemIndex >= items.Length) {
						return match.Value;
					}

					char matched = match.Value[1];

					bool unquote = matched == 's' || matched == 'S';

					return GetStringValue(items[replacementItemIndex++], unquote, escape: char.IsUpper(matched));
				};
			}

			string GetStringValue(Expression expression, bool unquote, bool escape) {
				string value = expression is LessString str && unquote
					? str.GetUnquotedValue()
					: expression.ToString();

				return escape ? Uri.EscapeDataString(value) : value;
			}
		}
	}
}
