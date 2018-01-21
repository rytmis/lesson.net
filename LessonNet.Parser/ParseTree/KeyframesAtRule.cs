using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class KeyframesAtRule : AtRule {
		private readonly string ruleIdentifier;
		private readonly Identifier identifier;
		private readonly IList<Keyframe> keyframes;

		public KeyframesAtRule(string ruleIdentifier, Identifier identifier, IEnumerable<Keyframe> keyframes) {
			this.ruleIdentifier = ruleIdentifier ?? "";
			this.identifier = identifier;
			this.keyframes = keyframes.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new KeyframesAtRule(ruleIdentifier, identifier.EvaluateSingle<Identifier>(context), keyframes.Select(kf => kf.EvaluateSingle<Keyframe>(context)));
		}

		protected override string GetStringRepresentation() {
			return $"@{ruleIdentifier} {identifier} {{ {keyframes.Count} }}";
		}

		public override void WriteOutput(OutputContext context) {
			context.Indent();
			context.Append('@');
			context.Append(ruleIdentifier);
			context.Append(' ');
			context.Append(identifier);
			context.AppendOptional(' ');
			context.AppendLine("{");
			foreach (var keyframe in keyframes) {
				context.Append(keyframe);
			}
			context.Indent();
			context.AppendLine("}");
		}
	}

	public class Keyframe : LessNode {
		private readonly string keyword;
		private readonly IList<Expression> keyframePositions;
		private readonly RuleBlock block;

		public Keyframe(IEnumerable<Expression> keyframePositions, RuleBlock block) {
			this.keyframePositions = keyframePositions.ToList();
			this.block = block;
		}

		public Keyframe(string keyword, RuleBlock block) {
			this.keyword = keyword;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new Keyframe(keyframePositions.Select(p => p.EvaluateSingle<Expression>(context)), 
				new RuleBlock(block.Evaluate(context).Cast<Statement>()));
		}

		protected override string GetStringRepresentation() {
			var positions = string.Join(", ", keyframePositions.Select(kp => kp.ToString()));
			return $"{positions} {{ {block.Statements.Count} }}";
		}

		public override void WriteOutput(OutputContext context) {
			if (block.Statements.Count == 0) {
				return;
			}

			context.IncreaseIndentLevel();

			context.Indent();

			for (var index = 0; index < keyframePositions.Count; index++) {
				var keyframePosition = keyframePositions[index];

				context.Append(keyframePosition);

				if (index < keyframePositions.Count - 1) {
					context.Append(", ");
				}
			}

			context.AppendOptional(' ');
			context.AppendLine("{");

			context.Append(block);

			context.Indent();
			context.AppendLine("}");
			
			context.DecreaseIndentLevel();
		}
	}
}