﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class SelectorList : LessNode {
		public static readonly SelectorList Empty = new SelectorList(new Selector[] {new Selector(new SelectorElement[] { })});

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
			bool ShouldReparse(Selector selector) {
				return selector.Elements
					.OfType<IdentifierSelectorElement>()
					.Any(ise => ise.Identifier.Parts.OfType<ConstantIdentifierPart>().Any(cip => Regex.IsMatch(cip.Value, "[,\\s]")));
			}

			IEnumerable<Selector> EvaluateSelectors() {
				foreach (var selector in selectors) {

					var evaluatedSelector = selector.EvaluateSingle<Selector>(context);

					if (!ShouldReparse(evaluatedSelector)) {
						yield return evaluatedSelector;
					} else {
						var parsedSelectorList = context.Parser.ParseSelectorList(evaluatedSelector.ToString());

						foreach (var generatedSelector in parsedSelectorList.Selectors) {
							yield return generatedSelector.EvaluateSingle<Selector>(context);
						}
					}
				}
			}

			yield return new SelectorList(EvaluateSelectors());
		}

		public void AddExtenders(EvaluationContext context) {
			foreach (var selector in Selectors) {
				selector.AddExtenders(context);
			}
		}

		protected override string GetStringRepresentation() {
			return string.Join($",{Environment.NewLine}", Selectors.Select(s => s.ToString()));
		}

		public override void WriteOutput(OutputContext context) {
			var outputSelectors = Selectors
				.Concat(Selectors.SelectMany(s => context.Extensions.GetExtensions(s, includeReferences: true)))
				.Distinct()
				.ToList();

			for (var index = 0; index < outputSelectors.Count; index++) {
				context.Indent();
				var selector = outputSelectors[index];
				context.Append(selector);
				if (index < outputSelectors.Count - 1) {
					context.AppendLine(",");
				}
			}
		}

		public bool MatchesAny(SelectorList selectorList) {
			return Selectors.Any(s => selectorList.Selectors.Any(s.Matches));
		}

		public bool Matches(Selector selector) {
			return Selectors.Any(selector.Matches);
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

		public SelectorList DropCombinators() {
			return new SelectorList(Selectors.Select(s => s.DropCombinators()));
		}

		public SelectorList RemovePrefixes(SelectorList other) {
			IEnumerable<Selector> GetResultingSelectors() {
				foreach (var selector in Selectors) {
					foreach (var otherSelector in other.Selectors) {
						var resultingPrefix = selector.RemovePrefix(otherSelector);
						if (resultingPrefix != null) {
							yield return resultingPrefix;
						}
					}
				}
			}

			return new SelectorList(GetResultingSelectors());
		}
	}
}