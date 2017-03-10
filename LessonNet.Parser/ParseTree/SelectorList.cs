using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class SelectorList : LessNode {
		private readonly List<Selector> selectors;

		public SelectorList(IEnumerable<Selector> selectors) {
			this.selectors = selectors.ToList();
		}

		public IReadOnlyList<Selector> Selectors => selectors.AsReadOnly();

		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}

		protected override string GetStringRepresentation() {
			return string.Join(", ", Selectors.Select(s => s.ToString()));
		}
	}
}