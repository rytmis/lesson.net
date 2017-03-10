using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class RuleBlock : LessNode {
		private readonly List<Rule> rules;
		private List<Statement> statements;

		public RuleBlock(IEnumerable<Rule> rules, IEnumerable<Statement> statements) {
			this.rules = rules.ToList();
			this.statements = statements.ToList();
		}

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}
}