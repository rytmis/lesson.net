using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Variable : Expression {
		private readonly string name;

		public Variable(string name) {
			this.name = name;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			var variable = context.CurrentScope.ResolveVariable(name);

			yield return variable.Values.EvaluateSingle<ListOfExpressionLists>(context);
		}

		protected override string GetStringRepresentation() {
			return $"@{name}";
		}


		protected bool Equals(Variable other) {
			return string.Equals(name, other.name);
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Variable) obj);
		}

		public override int GetHashCode() {
			return 397 ^ (name != null ? name.GetHashCode() : 0);
		}
	}
}