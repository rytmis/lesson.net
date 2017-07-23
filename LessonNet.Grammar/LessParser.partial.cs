using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Grammar
{
	public partial class LessParser
	{
		private bool matchCommaSeparatedLists = true;

		private Stack<bool> commaModeStack = new Stack<bool>();

		private void EnterCommaMode(bool mode)
		{
			commaModeStack.Push(matchCommaSeparatedLists);

			matchCommaSeparatedLists = mode;
		}

		private void ExitCommaMode()
		{
			matchCommaSeparatedLists = commaModeStack.Pop();
		}
	}
}
