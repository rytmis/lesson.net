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
			var evaluatedQueries = mediaQueries.Select(q => q.EvaluateSingle<MediaQuery>(context));
			var statements = block.Evaluate(context).Cast<Statement>();

			if (context.CurrentScope.Selectors == null) {
				// No bubbling: we are at the top level
				yield return new MediaBlock(evaluatedQueries, new RuleBlock(statements));
			} else {
				// Wrap the rules in a ruleset that inherits selectors from the enclosing scope
				var bubbledRuleset = new Ruleset(context.CurrentScope.Selectors, new RuleBlock(statements));
				var bubbledStatements = bubbledRuleset.Evaluate(context).Cast<Statement>();

				yield return new MediaBlock(evaluatedQueries, new RuleBlock(bubbledStatements));
			}
		}

		public override void WriteOutput(OutputContext context) {
			context.AppendLine($"@media {string.Join(", ", mediaQueries)} {{");
			context.Append(block);
			context.AppendLine("}");
		}

		protected override string GetStringRepresentation() {
			return $"@media {string.Join(", ", mediaQueries)} {{ {block.RuleCount} }}";
		}
	}

	public class MediaQuery : LessNode {
		private readonly MediaQueryModifier modifier;
		private readonly IList<MediaFeatureQuery> featureQueries;

		public MediaQuery(MediaQueryModifier modifier, IEnumerable<MediaFeatureQuery> featureQueries) {
			this.modifier = modifier;
			this.featureQueries = featureQueries.ToList();
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MediaQuery(modifier, featureQueries.Select(q => q.EvaluateSingle<MediaFeatureQuery>(context))) {
				IsEvaluated = true
			};
		}

		protected override string GetStringRepresentation() {
			return $"{string.Join(" and ", featureQueries)}";
		}
	}

	public enum MediaQueryModifier {
		None = 0,
		Not = 1,
		Only = 2
	}

	public abstract class MediaFeatureQuery : LessNode {

	}

	public class MediaIdentifierQuery : MediaFeatureQuery {
		private readonly string identifier;

		public MediaIdentifierQuery(string identifier) {
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

		public MediaPropertyQuery(Rule rule) {
			this.rule = rule;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new MediaPropertyQuery(rule.EvaluateSingle<Rule>(context));
		}

		protected override string GetStringRepresentation() {
			return $"({rule})";
		}
	}
}
