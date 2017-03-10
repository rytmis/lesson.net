using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class Rule : LessNode {
		private readonly string property;
		private List<ExpressionList> values;

		public Rule(string property, IEnumerable<ExpressionList> values) {
			this.property = property;
			this.values = values.ToList();
		}

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}
}