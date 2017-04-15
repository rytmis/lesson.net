using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public class SelectorList : LessNode {
		private readonly List<Selector> selectors;

		public SelectorList(IEnumerable<Selector> selectors) {
			this.selectors = selectors.ToList();
		}

		public IReadOnlyList<Selector> Selectors => selectors.AsReadOnly();

		public SelectorList Inherit(SelectorList parent) {
			IEnumerable<Selector> InheritSelectors() {
				foreach (var selector in selectors) {
					foreach (var generatedSelector in selector.Inherit(parent)) {
						yield return generatedSelector;
					}
				}
			}

			if (parent.IsEmpty()) {
				return this;
			}

			return new SelectorList(InheritSelectors());
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<Selector> EvaluateSelectors() {
				foreach (var selector in selectors) {
					foreach (var generatedSelector in selector.Evaluate(context)) {
						yield return (Selector) generatedSelector;
					}
				}
			}

			yield return new SelectorList(EvaluateSelectors());
		}

		protected override string GetStringRepresentation() {
			return string.Join($",{Environment.NewLine}", Selectors.Select(s => s.ToString()));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append(GetStringRepresentation());
		}

		public bool MatchesAny(SelectorList selectorList) {
			return Selectors.Any(s => selectorList.Selectors.Any(s.Equals));
		}

		protected bool Equals(SelectorList other) {
			return selectors.SequenceEqual(other.selectors);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((SelectorList) obj);
		}

		public override int GetHashCode() {
			return selectors.Aggregate(37, (s, sel) => s * sel.GetHashCode());
		}

		public bool IsEmpty() {
			return selectors.All(s => s.IsEmpty());
		}

		public static SelectorList Empty() {
			return new SelectorList(new[] {new Selector(Enumerable.Empty<SelectorElement>())});
		}
	}
}