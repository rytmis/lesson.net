using System.Collections.Generic;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree {
	public class NamespaceAtRule : AtRule {
		private readonly Identifier identifier;
		private readonly Expression ns;

		public NamespaceAtRule(Identifier identifier, Expression ns) {
			this.identifier = identifier;
			this.ns = ns;
		}
		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield return new NamespaceAtRule(identifier.EvaluateSingle<Identifier>(context), ns.EvaluateSingle<Expression>(context));
		}

		public override void WriteOutput(OutputContext context) {
			context.Append("@namespace ");
			context.Append(identifier);
			context.Append(' ');
			context.Append(ns);
			context.Append(';');
		}
	}
}