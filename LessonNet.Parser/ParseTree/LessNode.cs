using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser.CodeGeneration;

namespace LessonNet.Parser.ParseTree {
	public abstract class LessNode {

		protected virtual string GetStringRepresentation() {
			return $"{GetType().Name}";
		}

		public virtual void WriteOutput(OutputContext context) { }

		public string ToCss() {
			var outputContext = new OutputContext(' ', 4);
			WriteOutput(outputContext);

			return outputContext.GetCss();
		}


		public override string ToString() => GetStringRepresentation();

		internal bool IsEvaluated { get; set; }
		public IEnumerable<LessNode> Evaluate(EvaluationContext context) {
			if (IsEvaluated) {
				yield return this;
				yield break;
			}

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
	}
}