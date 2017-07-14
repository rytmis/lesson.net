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

		public void Append(char input) {
			outputBuffer.Append(input);
		}

		public void Append(string input) {
			outputBuffer.Append(input);
		}

		public void AppendLine(string input) {
			outputBuffer.Append(input);
			outputBuffer.Append(Environment.NewLine);
		}

		public void Append(LessNode node) {
			node.WriteOutput(this);
		}

		public void Append(IEnumerable<LessNode> nodes, string separator) {
			bool first = true;
			foreach (var node in nodes) {
				if (!first) {
					Append(separator);
				}
				node.WriteOutput(this);

				first = false;
			}
		}

		public void Indent() {
			int indentCharCount = indentLevel * indentCount;
			for (int i = 0; i < indentCharCount; i++) {
				outputBuffer.Append(indentChar);
			}
		}

		public void IncreaseIndentLevel() {
			indentLevel++;
		}

		public void DecreaseIndentLevel() {
			indentLevel--;
		}

		public string GetCss() => outputBuffer.ToString();
	}
}
