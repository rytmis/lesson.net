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
		public Expression Value { get; }

		protected MixinCallArgument(Expression value)
		{
			Value = value;
		}
	}

	public class PositionalArgument : MixinCallArgument
	{
		public PositionalArgument(Expression value) : base(value) { }

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context)
		{
			yield return new PositionalArgument(Value.EvaluateSingle<Expression>(context));
		}

		protected override string GetStringRepresentation()
		{
			return string.Join("; ", Value);
		}
	}

	public class NamedArgument : MixinCallArgument
	{
		public string ParameterName { get; }

		public NamedArgument(string parameterName, Expression value) : base(value)
		{
			this.ParameterName = parameterName;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context)
		{
			yield return new NamedArgument(ParameterName, Value.EvaluateSingle<Expression>(context));
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

		public IdentifierArgument(Identifier identifier) : base(identifier)
		{
			this.Identifier = identifier;
		}
	}
}
