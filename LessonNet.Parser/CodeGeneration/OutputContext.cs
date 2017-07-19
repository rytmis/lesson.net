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
		public ExtenderRegistry Extensions { get; }
		private readonly char indentChar;
		private readonly int indentCount;
		private short indentLevel = 0;

		private readonly StringBuilder outputBuffer = new StringBuilder();

		internal OutputContext(ExtenderRegistry extensions, char indentChar, int indentCount) {
			this.Extensions = extensions ?? new ExtenderRegistry();
			this.indentChar = indentChar;
			this.indentCount = indentCount;
		}

		public OutputContext(char indentChar, int indentCount) : this(null, indentChar, indentCount) {
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

		public void TrimTrailingWhitespace() {
			while (outputBuffer.Length > 0 && Char.IsWhiteSpace(outputBuffer[outputBuffer.Length - 1])) {
				outputBuffer.Remove(outputBuffer.Length - 1, 1);
			}
		}

		public string GetCss() => outputBuffer.ToString();
	}

	public class ExtenderRegistry {
		private readonly IList<(Extender Extender, Selector Selector)> extensions = new List<(Extender Extender, Selector Selector)>();

		public void Add(Extender target, Selector extension) {
			extensions.Add((target, extension));
		}

		public IEnumerable<Selector> GetExtensions(Selector candidate) {
			if (extensions == null) {
				yield break;
			}

			foreach (var extension in extensions) {
				if (!extension.Extender.PartialMatch && candidate.Equals(extension.Extender.Target)) {
					yield return extension.Selector;
				} else if (extension.Extender.PartialMatch && candidate.Contains(extension.Extender.Target)) {
					yield return candidate.Replace(extension.Extender.Target, extension.Selector);
				}
			}
		}
	}
}
