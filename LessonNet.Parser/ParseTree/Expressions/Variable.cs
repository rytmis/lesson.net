using System.Collections.Generic;

namespace LessonNet.Parser.ParseTree.Expressions {
	public class Variable : Expression {
		private readonly Variable variable;
		public string Name { get; }

		public bool Important { get; }

		public Variable(string name) : this(name, false) { }
		public Variable(Variable variable) : this(variable, false) { }

		public Variable(Variable variable, bool important) {
			this.variable = variable;
			this.Important = important;
		}

		public Variable(string name, bool important) {
			this.Name = name;
			this.Important = important;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			if (variable != null) {
				var result = variable.EvaluateSingle<Expression>(context);
				if (result is Identifier id) {
					yield return EvaluateVariable(context, id.ToString());
				} else if (result is LessString str) {
					yield return EvaluateVariable(context, str.GetUnquotedValue());
				} else {
					throw new EvaluationException($"Expected @{variable.Name} to evaluate to a string or an identifier, but got {result.GetType().Name}");
				}

				
			} else {
				yield return EvaluateVariable(context, Name);
			}
		}

		private Expression EvaluateVariable(EvaluationContext context, string variableName) {
			var declaration = context.CurrentScope.ResolveVariable(variableName);

			return declaration.Value.EvaluateSingle<Expression>(context);
		}

		protected override string GetStringRepresentation() {
			return $"@{Name}";
		}

		protected bool Equals(Variable other) {
			return Equals(variable, other.variable) 
				&& string.Equals(Name, other.Name) 
				&& Important == other.Important;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Variable) obj);
		}

		public override int GetHashCode() {
			unchecked {
				int hashCode = 396;
				hashCode = (hashCode * 397) ^ (variable != null ? variable.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ Important.GetHashCode();
				return hashCode;
			}
		}
	}
}