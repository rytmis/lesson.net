using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.SyntaxTree;
using LessonNet.Parser.Util;

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

		public IEnumerable<Selector> Inherit(SelectorList parentSelectors) {
			IEnumerable<SelectorElement> SubstituteFirstParentSelector(Selector parentSelector, IList<SelectorElement> currentElements) {
				bool substituted = false;
				foreach (var selectorElement in currentElements) {

					if (!substituted && selectorElement is ParentReferenceSelectorElement parentRef) {
						// Add all parent selector elements except the last one
						for (int i = 0; i < parentSelector.elements.Count - 1; i++) {
							yield return parentSelector.elements[i];
						}

						var lastParentElement = parentSelector.elements.Last();

						yield return new SelectorElement(lastParentElement.Element) {
							HasTrailingWhitespace = parentRef.HasTrailingWhitespace
						};

						substituted = true;
					} else {
						yield return selectorElement;
					}
				}
			}

			IEnumerable<Selector> SubstituteParentReferences(IList<SelectorElement> currentElements, bool isRoot = false) {
				if (currentElements.HasAny<ParentReferenceSelectorElement>()) {
					foreach (var selector in parentSelectors.Selectors) {
						foreach (var generatedSelector in SubstituteParentReferences(
							SubstituteFirstParentSelector(selector, currentElements).ToList())) {
							yield return generatedSelector;
						}
					}
				} else if (isRoot) {
					foreach (var parentSelector in parentSelectors.Selectors) {
						yield return new Selector(parentSelector.elements.Concat(currentElements));
					}
				} else {
					yield return new Selector(currentElements);
				}
			}

			return SubstituteParentReferences(elements, true);
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