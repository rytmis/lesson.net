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

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return string.Join(" ", elements.Select(e => e.Element));
		}

		protected override string GetCss() {
			return GetStringRepresentation();
		}

		public Selector Inherit(Selector parentSelector) {
			return new Selector(parentSelector.elements.Concat(elements));
		}
	}

	public class SelectorElement : LessNode {
		public string Element { get; }

		public SelectorElement(string element) {
			Element = element;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}

	public class ParentReferenceSelectorElement : SelectorElement {
		public ParentReferenceSelectorElement(string element) : base(element) { }
	}
}