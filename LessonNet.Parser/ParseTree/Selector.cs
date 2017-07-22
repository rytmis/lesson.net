using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {
	public class Selector : Expression {
		public Extend Extend { get; }
		public IReadOnlyList<SelectorElement> Elements { get; }

		public Selector(IEnumerable<SelectorElement> elements) : this (elements, null) { }

		public Selector(IEnumerable<SelectorElement> elements, Extend extend) {
			this.Extend = extend;
			this.Elements = elements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var extend = Extend?.EvaluateSingle<Extend>(context);

			var result = new Selector(CombineSequentialConstantIdentifiers(Elements.Select(e => e.EvaluateSingle<SelectorElement>(context))), extend);

			yield return result;
		}

		public void AddExtenders(EvaluationContext context) {
			if (Extend != null) {
				foreach (var extender in Extend.Extenders) {
					context.Extenders.Add(extender, this);
				}
			}
		}

		public IEnumerable<Selector> Inherit(SelectorList parentSelectors) {
			IEnumerable<SelectorElement> SubstituteFirstParentSelector(Selector parentSelector,
				IReadOnlyList<SelectorElement> currentElements,
				int firstIndex) {
				bool substituted = false;
				int current = -1;
				foreach (var selectorElement in currentElements) {
					current += 1;

					if (current >= firstIndex && !substituted && selectorElement is ParentReferenceSelectorElement parentRef) {
						// Add all parent selector elements except the last one
						for (int i = 0; i < parentSelector.Elements.Count - 1; i++) {
							yield return parentSelector.Elements[i];
						}

						var lastParentElement = parentSelector.Elements.LastOrDefault();
						if (lastParentElement != null) {
							yield return lastParentElement.Clone(parentRef.HasTrailingWhitespace);
						}

						substituted = true;
					} else {
						yield return selectorElement;
					}
				}
			}

			IEnumerable<Selector> SubstituteParentReferences(IReadOnlyList<SelectorElement> currentElements, bool isRoot = false, int? startFrom = null) {
				int firstIndex = currentElements.FirstIndexOf<ParentReferenceSelectorElement>(startFrom ?? 0);
				if (firstIndex > -1) {
					foreach (var selector in parentSelectors.Selectors) {
						var substituted = SubstituteFirstParentSelector(selector, currentElements, firstIndex).ToList();
						foreach (var generatedSelector in SubstituteParentReferences(substituted, false, firstIndex + selector.Elements.Count)) {
							yield return generatedSelector;
						}
					}
				} else if (isRoot) {
					foreach (var parentSelector in parentSelectors.Selectors) {
						yield return parentSelector.Append(currentElements);
					}
				} else {
					yield return new Selector(currentElements, Extend);
				}
			}

			return SubstituteParentReferences(Elements, true);
		}

		private static List<SelectorElement> CombineSequentialConstantIdentifiers(IEnumerable<SelectorElement> elements) {
			var selectorElements = elements.ToList();

			if (selectorElements.Count <= 1) {
				return selectorElements;
			}

			for (var i = selectorElements.Count - 2; i >= 0; i--) {
				var element = selectorElements[i];
				var nextElement = selectorElements[i + 1];

				if (element.HasTrailingWhitespace) {
					continue;
				}

				if (element is IdentifierSelectorElement ise1 && nextElement is IdentifierSelectorElement ise2) {
					var nextElementIdentifierValue = ((ConstantIdentifierPart) ise2.Identifier[0]).Value;

					if (SelectorElementBreakingTokens.Any(t => nextElementIdentifierValue.StartsWith(t))) {
						continue;
					}

					selectorElements.RemoveAt(i + 1);
					selectorElements[i] = new IdentifierSelectorElement(ise1.Identifier.CombineConstantIdentifiers(ise2.Identifier), false);
				}
			}
			return selectorElements;
		}


		private static readonly string[] SelectorElementBreakingTokens = { ".", ":", "#" };

		protected override string GetStringRepresentation() {
			return string.Join("", Elements.Where(e => !(e is ParentReferenceSelectorElement)).Select(e => e.ToString() + (e.HasTrailingWhitespace ? " " : ""))).Trim();
		}

		public override void WriteOutput(OutputContext context) {
			foreach (var element in Elements) {
				if (!(element is ParentReferenceSelectorElement)) {
					context.Append(element);
				}
			}

			context.TrimTrailingWhitespace();
		}

		public bool Matches(Selector other) {
			// TODO: This is probably a hot path, so it would make sense
			// to avoid allocations here
			return DropCombinators().Equals(other.DropCombinators());
		}

		protected bool Equals(Selector other) {
			return Elements.SequenceEqual(other.Elements);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Selector) obj);
		}

		public override int GetHashCode() {
			return Elements.Aggregate(37, (s, e) => s * e.GetHashCode());
		}

		public bool IsEmpty() {
			return Elements.Count == 0;
		}

		public Selector DropCombinators() {
			return new Selector(Elements.Where(e => !(e is CombinatorSelectorElement)));
		}

		public bool IsPrefixOf(Selector other) {
			return IsPrefixOf(other.Elements);
		}

		private bool IsPrefixOf(IReadOnlyList<SelectorElement> targetElements, bool allowExactMatch = false) {
			if (Elements.Count == 0) {
				return false;
			}

			if (!allowExactMatch && Elements.Count >= targetElements.Count) {
				return false;
			}

			if (Elements.Count > targetElements.Count) {
				return false;
			}

			for (var index = 0; index < Elements.Count; index++) {
				var element = Elements[index];
				var otherElement = targetElements[index];
				if (!Equals(element, otherElement)) {
					return false;
				}
			}

			return true;
		}

		public Selector RemovePrefix(Selector maybePrefix) {
			IEnumerable<SelectorElement> GetRemainingElements() {
				for (var i = maybePrefix.Elements.Count; i < Elements.Count; i++) {
					yield return Elements[i];
				}
			}

			if (!maybePrefix.IsPrefixOf(this)) {
				return null;
			}

			return new Selector(GetRemainingElements());
		}

		public bool Contains(Selector other) {
			return IndexOf(other) >= 0;
		}

		public Selector Replace(Selector search, Selector replace) {
			int index = IndexOf(search);
			if (index == -1) {
				return this;
			}

			bool lastReplacedElementHasTrailingWhitespace = Elements[index + (search.Elements.Count - 1)].HasTrailingWhitespace;

			var targetElements = replace.Elements.ToList();
			targetElements[targetElements.Count - 1] =
				targetElements[targetElements.Count - 1].Clone(lastReplacedElementHasTrailingWhitespace);

			var elements = Elements.ToList();
			elements.RemoveRange(index, search.Elements.Count);
			elements.InsertRange(index, targetElements);

			return new Selector(elements);
		}

		public Selector Append(IEnumerable<SelectorElement> elements) {
			var elementList = Elements.ToList();
			elementList[elementList.Count - 1] = elementList[elementList.Count - 1].Clone(withTrailingWhitespace: true);

			return new Selector(elementList.Concat(elements));
		}

		private int IndexOf(Selector other) {
			int index = 0;

			var elements = Elements.ToList();
			while (elements.Count > 0) {
				if (other.IsPrefixOf(elements, allowExactMatch: true)) {
					return index;
				}

				index += 1;
				elements.RemoveAt(0);
			}

			return -1;
		}
	}

	public abstract class SelectorElement : LessNode {
		public bool HasTrailingWhitespace { get; }

		protected SelectorElement(bool hasTrailingWhitespace) {
			HasTrailingWhitespace = hasTrailingWhitespace;
		}

		public override void WriteOutput(OutputContext context) {
			base.WriteOutput(context);
			context.Append(ToString());

			if (HasTrailingWhitespace) {
				context.Append(' ');
			}
		}

		public abstract SelectorElement Clone(bool withTrailingWhitespace);
	}

	public class IdentifierSelectorElement : SelectorElement {
		public Identifier Identifier { get; }

		public IdentifierSelectorElement(Identifier identifier, bool hasTrailingWhitespace) : base(hasTrailingWhitespace) {
			this.Identifier = identifier;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new IdentifierSelectorElement(Identifier.EvaluateSingle<Identifier>(context), HasTrailingWhitespace);
		}

		protected override string GetStringRepresentation() {
			return Identifier.ToString();
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Identifier);

			if (HasTrailingWhitespace) {
				context.Append(' ');
			}
		}

		public override SelectorElement Clone(bool withTrailingWhitespace) {
			return new IdentifierSelectorElement(Identifier, withTrailingWhitespace);
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
		public ParentReferenceSelectorElement(bool hasTrailingWhitespace) : base(hasTrailingWhitespace) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return "&";
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

		public override SelectorElement Clone(bool withTrailingWhitespace) {
			return new ParentReferenceSelectorElement(withTrailingWhitespace);
		}
	}

	public class AttributeSelectorElement : SelectorElement {
		private readonly Identifier attributeName;
		private readonly string op;
		private readonly Expression value;

		public AttributeSelectorElement(Identifier attributeName, bool hasTrailingWhitespace) : this(attributeName, null, null, hasTrailingWhitespace) {
		}

		public AttributeSelectorElement(Identifier attributeName, string op, Expression value, bool hasTrailingWhitespace) : base(hasTrailingWhitespace) {
			this.attributeName = attributeName;
			this.op = op;
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var exprValue = value?.EvaluateSingle<Expression>(context);

			yield return new AttributeSelectorElement(attributeName.EvaluateSingle<Identifier>(context), op, exprValue, HasTrailingWhitespace);
		}

		protected override string GetStringRepresentation() {
			var attr = string.IsNullOrWhiteSpace(op) 
				? attributeName.ToString() 
				: attributeName + op + value;

			return $"[{attr}]";
		}

		protected bool Equals(AttributeSelectorElement other) {
			return Equals(attributeName, other.attributeName) 
				&& string.Equals(op, other.op) 
				&& Equals(value, other.value);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AttributeSelectorElement) obj);
		}

		public override int GetHashCode() {
			unchecked {
				var hashCode = (attributeName != null ? attributeName.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (op != null ? op.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (value != null ? value.GetHashCode() : 0);
				return hashCode;
			}
		}

		public override SelectorElement Clone(bool withTrailingWhitespace) {
			return new AttributeSelectorElement(attributeName, op, value, withTrailingWhitespace);
		}
	}

	public class CombinatorSelectorElement : SelectorElement {
		private readonly string combinator;

		public CombinatorSelectorElement(string combinator, bool hasTrailingWhitespace) : base(hasTrailingWhitespace) {
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

		public override SelectorElement Clone(bool withTrailingWhitespace) {
			return new CombinatorSelectorElement(combinator, withTrailingWhitespace);
		}
	}

	public class Extend : LessNode {
		public List<Extender> Extenders { get; }

		public Extend(IEnumerable<Extender> extenders) {
			this.Extenders = extenders.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Extend(Extenders.Select(e => e.EvaluateSingle<Extender>(context)));
		}
	}

	public class Extender : LessNode {
		public Selector Target { get; }
		public bool PartialMatch { get; }

		public Extender(Selector target, bool partialMatch) {
			this.Target = target;
			this.PartialMatch = partialMatch;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Extender(Target.EvaluateSingle<Selector>(context), PartialMatch);
		}

		protected bool Equals(Extender other) {
			return Equals(Target, other.Target) && PartialMatch == other.PartialMatch;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Extender) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((Target != null ? Target.GetHashCode() : 0) * 397) ^ PartialMatch.GetHashCode();
			}
		}
	}
}