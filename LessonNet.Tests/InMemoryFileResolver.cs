using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LessonNet.Parser;

namespace LessonNet.Tests {
	public class InMemoryFileResolver : IFileResolver {
		private readonly string input;

		public InMemoryFileResolver(string input) {
			this.input = input;
		}
		public Stream GetContent() {
			return new MemoryStream(Encoding.UTF8.GetBytes(input));
		}

		public IFileResolver GetResolverFor(string lessFilePath) {
			if (Imports == null) {
				throw new InvalidOperationException($"Cannot resolve imports -- no imports defined for {nameof(InMemoryFileResolver)}");
			}
			if (Imports.ContainsKey(lessFilePath) == false) {
				throw new ArgumentException($"Imported file not found: [{lessFilePath}]");
			}

			return new InMemoryFileResolver(Imports[lessFilePath]) {
				Imports = Imports
			};
		}

		public string CurrentFile { get; }
		public Dictionary<string, string> Imports { get; set; }
	}
}