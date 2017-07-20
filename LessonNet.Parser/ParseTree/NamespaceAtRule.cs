using System.Collections.Generic;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class NamespaceAtRule : AtRule {
		private readonly Identifier identifier;
		private readonly Expression ns;

		public NamespaceAtRule(Identifier identifier, Expression ns) {
			this.identifier = identifier;
			this.ns = ns;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}
}