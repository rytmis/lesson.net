using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime.Tree;
using LessonNet.Parser.ParseTree.Expressions;

namespace LessonNet.Parser.ParseTree.Mixins
{
	public abstract class MixinCallArgument : LessNode
	{
		public IList<ExpressionList> Value { get; }

		protected MixinCallArgument(ExpressionList expressionList)
		{
			Value = new List<ExpressionList> { expressionList };
		}

		protected MixinCallArgument(IEnumerable<ExpressionList> value)
		{
			Value = value.ToList();
		}

		public TExpr EvaluateSingleValue<TExpr>(EvaluationContext context) where TExpr : Expression {
			if (Value.Count != 1) {
				return null;
			}

			return EvaluateSingleValueCore<TExpr>(context, Value[0]);
		}

		private static TExpr EvaluateSingleValueCore<TExpr>(EvaluationContext context, ExpressionList expressionList)
			where TExpr : Expression {
			if (expressionList.Values.Count != 1) {
				return null;
			}

			var value = expressionList.Values[0].EvaluateSingle<LessNode>(context);
			if (value is TExpr result) {
				return result;
			}

			if (value is ExpressionList list) {
				return EvaluateSingleValueCore<TExpr>(context, list);
			}

			return null;
		}
	}

	public class PositionalArgument : MixinCallArgument
	{
		public PositionalArgument(ExpressionList expressionList) : base(expressionList)
		{
		}

		public PositionalArgument(IEnumerable<ExpressionList> value) : base(value) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context)
		{
			yield return new PositionalArgument(Value.Select(l => l.EvaluateSingle<ExpressionList>(context)));
		}

		protected override string GetStringRepresentation()
		{
			return string.Join("; ", Value);
		}
	}

	public class NamedArgument : MixinCallArgument
	{
		public string ParameterName { get; }

		public NamedArgument(string parameterName, IEnumerable<ExpressionList> value) : base(value)
		{
			this.ParameterName = parameterName;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context)
		{
			yield return new NamedArgument(ParameterName, Value.Select(el => el.EvaluateSingle<ExpressionList>(context)));
		}

		protected override string GetStringRepresentation()
		{
			return $"@{ParameterName}: {string.Join("; ", Value)}";
		}

		public void DeclareIn(EvaluationContext context)
		{
			context.CurrentScope.DeclareVariable(new VariableDeclaration(ParameterName, Value));
		}
	}

	public class IdentifierArgument : PositionalArgument
	{
		public Identifier Identifier { get; }

		public IdentifierArgument(Identifier identifier) : base(new ExpressionList(new[] { identifier }))
		{
			this.Identifier = identifier;
		}
	}
}
