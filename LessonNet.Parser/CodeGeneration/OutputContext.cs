using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.ParseTree;

namespace LessonNet.Parser.CodeGeneration
{
	public class OutputContext
	{
		private readonly char indentChar;
		private readonly int indentCount;
		private short indentLevel = 0;

		private readonly StringBuilder outputBuffer = new StringBuilder();

		public OutputContext(char indentChar, int indentCount) {
			this.indentChar = indentChar;
			this.indentCount = indentCount;
		}

		public void Append(string input, bool indent = true) {
			AppendInternal(input, indent);
		}

		public void AppendLine(string input, bool indent = true) {
			AppendInternal(input, indent);
			AppendInternal(Environment.NewLine, indent: false);
		}

		public void Append(LessNode node) {
			node.WriteOutput(this);
		}

		public void Indent() {
			AppendInternal("", indent: true);
		}

		public void IncreaseIndentLevel() {
			indentLevel++;
		}

		public void DecreaseIndentLevel() {
			indentLevel--;
		}

		public string GetCss() => outputBuffer.ToString();

		private void AppendInternal(string input, bool indent = true) {
			if (indent) {
				int indentCharCount = indentLevel * indentCount;
				for (int i = 0; i < indentCharCount; i++) {
					outputBuffer.Append(indentChar);
				}
			}

			outputBuffer.Append(input);
		}
	}
}
