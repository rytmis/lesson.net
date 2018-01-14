using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LessonNet.Parser;

namespace LessonNet.Tests {
	public class InMemoryFileResolver : IFileResolver {
		private readonly string input;

		private string basePath = "";

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

			var resolvedPath = Path.Combine(basePath, lessFilePath).Replace('\\', '/');
			if (Imports.ContainsKey(resolvedPath) == false) {
				throw new ArgumentException($"Imported file not found: [{lessFilePath} -- tried {resolvedPath}]");
			}

			return new InMemoryFileResolver(Imports[resolvedPath]) {
				Imports = Imports,

				basePath = Path.Combine(basePath, Path.GetDirectoryName(lessFilePath))
			};
		}

		public string CurrentFile { get; }
		public Dictionary<string, string> Imports { get; set; }
	}
}