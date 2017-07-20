using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	public class EFunction : LessFunction {
		public EFunction(ListOfExpressionLists arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(ListOfExpressionLists arguments) {
			var str = arguments.Single<LessString>();
			if (str == null) {
				throw new EvaluationException("Argument must be a string");
			}

			return new Identifier(new ConstantIdentifierPart(str.GetUnquotedValue()));
		}
	}
}
