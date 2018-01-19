using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;

namespace LessonNet.Parser {
	public class FileResolver : IFileResolver {
		private readonly IFileSystem fileSystem;

		public FileResolver(IFileSystem fileSystem, string fileName) {
			this.fileSystem = fileSystem;

			CurrentFile = fileName;

			BasePath = Path.GetDirectoryName(fileName);
		}

		public Stream GetContent() {
			var fixedPath =
				CurrentFile.Replace(fileSystem.Path.AltDirectorySeparatorChar, fileSystem.Path.DirectorySeparatorChar);

			return fileSystem.File.OpenRead(fixedPath);
		}
		public Stream GetContent(string filePath) {
			var resolved = ResolvePath(filePath);

			var fixedPath =
				resolved.Replace(fileSystem.Path.AltDirectorySeparatorChar, fileSystem.Path.DirectorySeparatorChar);

			return fileSystem.File.OpenRead(fixedPath);
		}

		public IFileResolver GetResolverFor(string lessFilePath) {
			return new FileResolver(fileSystem, ResolvePath(lessFilePath));
		}

		public string ResolvePath(string relativePath) {
			var path = Path.Combine(BasePath, relativePath).Replace('\\', '/');

			Stack<string> pathStack = new Stack<string>();
			foreach (var pathComponent in path.Split('\\', '/')) {
				if (pathComponent == ".." && pathStack.Count > 0) {
					pathStack.Pop();
				} else if (pathComponent != ".") {
					pathStack.Push(pathComponent);
				}
			}

			return string.Join("/", pathStack.Reverse());
		}

		public string CurrentFile { get; }
		public string BasePath { get; }
	}
}