using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;
using LessonNet.Parser.Util;

namespace LessonNet.Parser.ParseTree
{
	public class MediaBlock : Statement
	{
		private readonly IList<MediaQuery> mediaQueries;
		public RuleBlock Block { get; }

		public MediaBlock(IEnumerable<MediaQuery> mediaQueries, RuleBlock block) {
			this.mediaQueries = mediaQueries.ToList();
			this.Block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			IEnumerable<MediaQuery> CombineQueries(IEnumerable<MediaQuery> outer, IEnumerable<MediaQuery> inner) {
				foreach (var innerQuery in inner) {
					foreach (var outerQuery in outer) {
						yield return new MediaQuery(outerQuery.FeatureQueries.Concat(innerQuery.FeatureQueries));
					}
				}
			}

			var evaluatedQueries = mediaQueries.Select(q => q.EvaluateSingle<MediaQuery>(context)).ToArray();
			(var mediaBlocks, var statements) = Block.Evaluate(context).Split<MediaBlock, Statement>();

			yield return new MediaBlock(evaluatedQueries, new RuleBlock(statements)) {
				IsEvaluated = true
			};

			foreach (var mediaBlock in mediaBlocks) {
				yield return new MediaBlock(CombineQueries(evaluatedQueries, mediaBlock.mediaQueries), mediaBlock.Block) {
					IsEvaluated = true
				};
			}
		}

		public MediaBlock Bubble(EvaluationContext context) {
			// Wrap the rules in a ruleset that inherits selectors from the enclosing scope
			var bubbledRuleset = new Ruleset(context.CurrentScope.Selectors, new RuleBlock(Block.Statements));
			var bubbledStatements = bubbledRuleset.Evaluate(context).Cast<Statement>();

			return new MediaBlock(mediaQueries, new RuleBlock(bubbledStatements) { IsEvaluated = true }) {
				IsEvaluated = true
			};
		}


		public override void WriteOutput(OutputContext context) {
			if (Block.Statements.Count == 0) {
				return;
			}

			context.Append("@media ");
			for (var index = 0; index < mediaQueries.Count; index++) {
				if (index > 0) {
					context.Append(", ");
				}
				var mediaQuery = mediaQueries[index];
				context.Append(mediaQuery);
			}

			context.AppendLine(" {");
			context.Append(Block);
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"@media {string.Join(", ", mediaQueries)} {{ {Block.RuleCount} }}";
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

		public override void WriteOutput(OutputContext context) {
			for (var i = 0; i < FeatureQueries.Count; i++) {
				if (i > 0) {
					context.Append(" and ");
				}

				var featureQuery = FeatureQueries[i];
				context.Append(featureQuery);
			}
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
		private readonly ExpressionList identifier;

		public MediaIdentifierQuery(MediaQueryModifier modifier, ExpressionList identifier) : base(modifier) {
			this.identifier = identifier;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MediaIdentifierQuery(Modifier, identifier.EvaluateSingle<ExpressionList>(context));
		}

		protected override string GetStringRepresentation() {
			return identifier.ToString();
		}

		public override void WriteOutput(OutputContext context) {
			if (Modifier != MediaQueryModifier.None) {
				context.Append(Modifier.ToString().ToLowerInvariant() + " ");
			}

			context.Append(identifier);
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

		public override void WriteOutput(OutputContext context) {
			if (Modifier != MediaQueryModifier.None) {
				context.Append(Modifier.ToString().ToLowerInvariant() + " ");
			}

			context.Append("(");
			context.Append(rule);
			context.Append(")");
		}
	}
}
