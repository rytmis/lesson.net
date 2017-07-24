using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	[DebuggerDisplay("{" + nameof(DebuggerDisplay) + "}")]
	public abstract class LessNode {

		protected virtual string GetStringRepresentation() {
			return $"{GetType().Name}";
		}

		public virtual void WriteOutput(OutputContext context) { }

		public override string ToString() => GetStringRepresentation();

		public IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			foreach (var resultNode in EvaluateCore(context)) {
				yield return resultNode;
			}
		}

		public TNode EvaluateSingle<TNode>(EvaluationContext context) where TNode : LessNode {
			var resultNodes = Evaluate(context).ToList();

			if (resultNodes.Count > 1) {
				throw new EvaluationException($"Expected single node but got {resultNodes.Count}");
			}

			LessNode firstNode = resultNodes.First();
			if (firstNode is TNode result) {
				return result;
			}

			throw new EvaluationException($"Expected node with type {typeof(TNode)} but got {firstNode.GetType().Name}");
		}

		protected abstract IEnumerable<LessNode> EvaluateCore(EvaluationContext context);

		private string DebuggerDisplay => $"({GetType().Name}: {ToString()})";
	}

	public abstract class NoOutputNode : LessNode {
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			return Enumerable.Empty<LessNode>();
		}
	}
}