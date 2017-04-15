using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {
	public class Selector : LessNode {
		private readonly List<SelectorElement> elements;

		public Selector(IEnumerable<SelectorElement> elements) {
			this.elements = elements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Selector(elements.Select(e => e.EvaluateSingle<SelectorElement>(context)));
		}

		protected override string GetStringRepresentation() {
			return string.Join("", elements.Select(e => e.ToString())).Trim();
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(GetStringRepresentation());
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

						var lastParentElement = parentSelector.elements.LastOrDefault();
						if (lastParentElement is IdentifierSelectorElement ise) {
							yield return new IdentifierSelectorElement(ise.Identifier) {
								HasTrailingWhitespace = parentRef.HasTrailingWhitespace
							};
						}

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

		protected bool Equals(Selector other) {
			return elements.SequenceEqual(other.elements);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Selector) obj);
		}

		public override int GetHashCode() {
			return elements.Aggregate(37, (s, e) => s * e.GetHashCode());
		}

		public bool IsEmpty() {
			return elements.Count == 0;
		}
	}

	public abstract class SelectorElement : LessNode {
		public bool HasTrailingWhitespace { get; set; }

	}

	public class IdentifierSelectorElement : SelectorElement {
		public Identifier Identifier { get; }

		public IdentifierSelectorElement(Identifier identifier) {
			this.Identifier = identifier;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new IdentifierSelectorElement(Identifier.EvaluateSingle<Identifier>(context)) {
				HasTrailingWhitespace = HasTrailingWhitespace
			};
		}

		protected override string GetStringRepresentation() {
			return Identifier + (HasTrailingWhitespace ? " " : "");
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((IdentifierSelectorElement) obj);
		}

		protected bool Equals(IdentifierSelectorElement other) {
			return Equals(Identifier, other.Identifier);
		}

		public override int GetHashCode() {
			return (Identifier != null ? Identifier.GetHashCode() : 0);
		}
	}

	public class ParentReferenceSelectorElement : SelectorElement {
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return string.Empty;
		}

		protected bool Equals(ParentReferenceSelectorElement other) {
			return true;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ParentReferenceSelectorElement) obj);
		}

		public override int GetHashCode() {
			return '&'.GetHashCode();
		}
	}

	public class AttributeSelectorElement : SelectorElement {
		private readonly string attributeReference;

		public AttributeSelectorElement(string attributeReference) {
			this.attributeReference = attributeReference;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return attributeReference + (HasTrailingWhitespace ? " " : "");
		}

		public override int GetHashCode() {
			return attributeReference.GetHashCode() + HasTrailingWhitespace.GetHashCode();
		}
	}

	public class CombinatorSelectorElement : SelectorElement {
		private readonly string combinator;

		public CombinatorSelectorElement(string combinator) {
			this.combinator = combinator;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return combinator;
		}

		protected bool Equals(CombinatorSelectorElement other) {
			return string.Equals(combinator, other.combinator);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((CombinatorSelectorElement) obj);
		}

		public override int GetHashCode() {
			return (combinator != null ? combinator.GetHashCode() : 0);
		}
	}
}