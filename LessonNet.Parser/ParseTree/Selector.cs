using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;

namespace LessonNet.Parser.ParseTree {
	public class Selector : LessNode {
		private List<SelectorElement> elements;

		public Selector(IEnumerable<SelectorElement> elements) {
			this.elements = elements.ToList();
		}
		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}

		protected override string GetStringRepresentation() {
			return string.Join(" ", elements.Select(e => e.Element));
		}
	}

	public class SelectorElement : LessNode {
		public string Element { get; }

		public SelectorElement(string element) {
			Element = element;
		}
		public override IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}

	public class ParentReferenceSelectorElement : SelectorElement {
		public ParentReferenceSelectorElement(string element) : base(element) { }
	}
}