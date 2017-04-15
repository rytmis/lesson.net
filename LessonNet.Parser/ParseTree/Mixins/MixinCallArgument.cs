using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Mixins
{
	public class MixinCallArgument : LessNode
	{
		private readonly ListOfExpressionLists value;

		public MixinCallArgument(ExpressionList expressionList)
		{
			value = new ListOfExpressionLists(new[] { expressionList });
		}

		public MixinCallArgument(ListOfExpressionLists value)
		{
			this.value = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context)
		{
			foreach (var expressionList in value.Evaluate(context))
			{
				yield return expressionList;
			}
		}

		protected override string GetStringRepresentation()
		{
			return value.ToString();
		}
	}
}
