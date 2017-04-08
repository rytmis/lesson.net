﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace LessonNet.Parser.ParseTree {
	public class MixinCall : Statement {
		private readonly SelectorList selectors;
		private readonly List<MixinCallArgument> arguments;

		public MixinCall(SelectorList selectors, IEnumerable<MixinCallArgument> arguments) {
			this.selectors = selectors;
			this.arguments = arguments.ToList();
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			yield break;
		}
	}

	public class MixinCallArgument : LessNode {
		private readonly ExpressionList expressionListValue;
		private readonly ListOfExpressionLists listOfExpressionListsValue;

		public MixinCallArgument(ExpressionList expressionList) {
			expressionListValue = expressionList;
		}

		public MixinCallArgument(ListOfExpressionLists value) {
			listOfExpressionListsValue = value;
		}

		protected override IEnumerable<LessNode> EvaluateCore(EvaluationContext context) {
			throw new NotImplementedException();
		}
	}
}