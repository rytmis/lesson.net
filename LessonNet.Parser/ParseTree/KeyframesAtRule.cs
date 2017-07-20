using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class KeyframesAtRule : AtRule {
		private readonly Identifier identifier;
		private readonly IList<Keyframe> keyframes;

		public KeyframesAtRule(Identifier identifier, IEnumerable<Keyframe> keyframes) {
			this.identifier = identifier;
			this.keyframes = keyframes.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}

	public class Keyframe : LessNode {
		private readonly string keyword;
		private readonly Measurement percentage;
		private readonly RuleBlock block;

		public Keyframe(Measurement percentage, RuleBlock block) {
			this.percentage = percentage;
			this.block = block;
		}

		public Keyframe(string keyword, RuleBlock block) {
			this.keyword = keyword;
			this.block = block;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new System.NotImplementedException();
		}
	}
}