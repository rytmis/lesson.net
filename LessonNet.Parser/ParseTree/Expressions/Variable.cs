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
				var result = variable.EvaluateSingle<ListOfExpressionLists>(context).Single<Expression>();
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

		private ListOfExpressionLists EvaluateVariable(EvaluationContext context, string variableName) {
			var declaration = context.CurrentScope.ResolveVariable(variableName);

			var values = declaration.Values.EvaluateSingle<ListOfExpressionLists>(context);

			return new ListOfExpressionLists(values, values.Separator, Important || values.Important);
		}

		protected override string GetStringRepresentation() {
			return $"@{Name}";
		}


		protected bool Equals(Variable other) {
			return string.Equals(Name, other.Name);
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj);
		}

		public override int GetHashCode() {
			return 397 ^ (Name != null ? Name.GetHashCode() : 0);
		}
	}
}