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
			return string.Join("", elements.Select(e => e.Element + (e.HasTrailingWhitespace ? " " : ""))).Trim();
		}

		protected override string GetCss() {
			return GetStringRepresentation();
		}

		public Selector RemoveParentReferences() {
			return new Selector(elements.Except(elements.OfType<ParentReferenceSelectorElement>()));
		}

		public Selector Inherit(Selector parentSelector) {

			IEnumerable<SelectorElement> SubstituteParentReferences() {
				foreach (var selectorElement in elements) {

					if (selectorElement is ParentReferenceSelectorElement parentRef) {
						// Propagate the parent ref upwards in the tree
						yield return new ParentReferenceSelectorElement() {
							HasTrailingWhitespace = true
						};

						// Add all parent selector elements except the last one
						for (int i = 0; i < parentSelector.elements.Count - 1; i++) {
							yield return parentSelector.elements[i];
						}

						var lastParentElement = parentSelector.elements.Last();

						yield return new SelectorElement(lastParentElement.Element) {
							HasTrailingWhitespace = parentRef.HasTrailingWhitespace
						};
					} else {
						yield return selectorElement;
					}
				}
			}

			if (elements.OfType<ParentReferenceSelectorElement>().Any()) {
				return new Selector(SubstituteParentReferences());
			}
			return new Selector(parentSelector.elements.Concat(elements));
		}
	}

	public class SelectorElement : LessNode {
		public string Element { get; }
		public bool HasTrailingWhitespace { get; set; }

		public SelectorElement(string element) {
			Element = element;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}

		protected override string GetStringRepresentation() {
			return Element;
		}
	}

	public class ParentReferenceSelectorElement : SelectorElement {
		public ParentReferenceSelectorElement() : base(string.Empty) { }
	}
}