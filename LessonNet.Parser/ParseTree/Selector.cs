using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree {
	public class Selector : Expression {
		public List<SelectorElement> Elements { get; }

		public Selector(IEnumerable<SelectorElement> elements) {
			this.Elements = elements.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Selector(
				CombineSequentialConstantIdentifiers(Elements.Select(e => e.EvaluateSingle<SelectorElement>(context))));
		}

		public IEnumerable<Selector> Inherit(SelectorList parentSelectors) {
			IEnumerable<SelectorElement> SubstituteFirstParentSelector(Selector parentSelector,
				IList<SelectorElement> currentElements) {
				bool substituted = false;
				foreach (var selectorElement in currentElements) {

					if (!substituted && selectorElement is ParentReferenceSelectorElement parentRef) {
						// Add all parent selector elements except the last one
						for (int i = 0; i < parentSelector.Elements.Count - 1; i++) {
							yield return parentSelector.Elements[i];
						}

						var lastParentElement = parentSelector.Elements.LastOrDefault();
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
						yield return new Selector(parentSelector.Elements.Concat(currentElements));
					}
				} else {
					yield return new Selector(CombineSequentialConstantIdentifiers(currentElements));
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
					selectorElements[i] = new IdentifierSelectorElement(ise1.Identifier.CombineConstantIdentifiers(ise2.Identifier));
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
			if (Elements.Count == 0) {
				return false;
			}

			if (Elements.Count >= other.Elements.Count) {
				return false;
			}

			for (var index = 0; index < Elements.Count; index++) {
				var element = Elements[index];
				var otherElement = other.Elements[index];
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
	}

	public abstract class SelectorElement : LessNode {
		public bool HasTrailingWhitespace { get; set; }

		public override void WriteOutput(OutputContext context) {
			base.WriteOutput(context);
			context.Append(ToString());

			if (HasTrailingWhitespace) {
				context.Append(' ');
			}
		}
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
			return Identifier.ToString();
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(Identifier);

			if (HasTrailingWhitespace) {
				context.Append(' ');
			}
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
	}

	public class AttributeSelectorElement : SelectorElement {
		private readonly Identifier attributeName;
		private readonly string op;
		private readonly Expression value;

		public AttributeSelectorElement(Identifier attributeName) : this(attributeName, null, null) {
		}

		public AttributeSelectorElement(Identifier attributeName, string op, Expression value) {
			this.attributeName = attributeName;
			this.op = op;
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var expression = value?.EvaluateSingle<Expression>(context);
			var exprValue = expression is ListOfExpressionLists list
				? list.Single<Expression>()
				: expression;

			yield return new AttributeSelectorElement(attributeName.EvaluateSingle<Identifier>(context), op, exprValue) {
				HasTrailingWhitespace = HasTrailingWhitespace
			};
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