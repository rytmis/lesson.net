using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LessonNet.Parser.SyntaxTree;

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
			return string.Join(",\n", Selectors.Select(s => s.ToString()));
		}

		protected override string GetCss() {
			return GetStringRepresentation();
		}
	}
}