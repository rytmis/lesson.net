using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree
{
	public class MediaBlock : Statement
	{
		private readonly IList<MediaQuery> mediaQueries;
		private readonly RuleBlock block;

		public MediaBlock(IEnumerable<MediaQuery> mediaQueries, RuleBlock block) {
			this.mediaQueries = mediaQueries.ToList();
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<MediaQuery> CombineQueries(IEnumerable<MediaQuery> outer, IEnumerable<MediaQuery> inner) {
				foreach (var outerQuery in outer) {
					foreach (var innerQuery in inner) {
						yield return new MediaQuery(outerQuery.FeatureQueries.Concat(innerQuery.FeatureQueries));
					}
				}
			}

			var evaluatedQueries = mediaQueries.Select(q => q.EvaluateSingle<MediaQuery>(context));
			(var mediaBlocks, var statements) = block.Evaluate(context).Split<MediaBlock, Statement>();

			if (context.CurrentScope.Selectors == null) {
				// No bubbling: we are at the top level
				yield return new MediaBlock(evaluatedQueries, new RuleBlock(statements));

				foreach (var mediaBlock in mediaBlocks) {
					yield return new MediaBlock(CombineQueries(this.mediaQueries, mediaBlock.mediaQueries), mediaBlock.block);
				}
			} else {
				// Wrap the rules in a ruleset that inherits selectors from the enclosing scope
				yield return Bubble(context, statements, evaluatedQueries);

				foreach (var mediaBlock in mediaBlocks) {
					yield return Bubble(context, mediaBlock.block.Statements, CombineQueries(mediaQueries, mediaBlock.mediaQueries));
				}
			}
		}

		private static MediaBlock Bubble(EvaluationContext context, IList<Statement> statements, IEnumerable<MediaQuery> evaluatedQueries) {
			var bubbledRuleset = new Ruleset(context.CurrentScope.Selectors, new RuleBlock(statements));
			var bubbledStatements = bubbledRuleset.Evaluate(context).Cast<Statement>();

			return new MediaBlock(evaluatedQueries, new RuleBlock(bubbledStatements));
		}

		public override void WriteOutput(OutputContext context) {
			if (block.Statements.Count == 0) {
				return;
			}

			context.AppendLine($"@media {string.Join(", ", mediaQueries)} {{");
			context.Append(block);
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"@media {string.Join(", ", mediaQueries)} {{ {block.RuleCount} }}";
		}
	}

	public class MediaQuery : LessNode {
		public IList<MediaFeatureQuery> FeatureQueries { get; }

		public MediaQuery(IEnumerable<MediaFeatureQuery> featureQueries) {
			this.FeatureQueries = featureQueries.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MediaQuery(FeatureQueries.Select(q => q.EvaluateSingle<MediaFeatureQuery>(context))) {
				IsEvaluated = true
			};
		}

		protected override string GetStringRepresentation() {
			return $"{string.Join(" and ", FeatureQueries)}";
		}
	}

	public enum MediaQueryModifier {
		None = 0,
		Not = 1,
		Only = 2
	}

	public abstract class MediaFeatureQuery : LessNode {
		public MediaQueryModifier Modifier { get; }

		protected MediaFeatureQuery(MediaQueryModifier modifier) {
			this.Modifier = modifier;
		}
	}

	public class MediaIdentifierQuery : MediaFeatureQuery {
		private readonly string identifier;

		public MediaIdentifierQuery(MediaQueryModifier modifier, string identifier) : base(modifier) {
			this.identifier = identifier;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return this;
		}

		protected override string GetStringRepresentation() {
			return identifier;
		}
	}

	public class MediaPropertyQuery : MediaFeatureQuery {
		private readonly Rule rule;

		public MediaPropertyQuery(MediaQueryModifier modifier, Rule rule) : base(modifier) {
			this.rule = rule;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MediaPropertyQuery(Modifier, rule.EvaluateSingle<Rule>(context));
		}

		protected override string GetStringRepresentation() {
			return $"({rule})";
		}
	}
}
