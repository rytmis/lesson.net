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
		}

		public Stream GetContent() {
			var fixedPath =
				CurrentFile.Replace(fileSystem.Path.AltDirectorySeparatorChar, fileSystem.Path.DirectorySeparatorChar);

			return fileSystem.File.OpenRead(fixedPath);
		}

		public IFileResolver GetResolverFor(string lessFilePath, string basePathOverride = null) {
			string currentBasePath = !string.IsNullOrEmpty(basePathOverride)
				? basePathOverride
				: BasePath;

			var resolvedPath = ResolvePath(currentBasePath, lessFilePath);

			return new FileResolver(fileSystem, resolvedPath) {
				BasePath = Path.Combine(currentBasePath, Path.GetDirectoryName(lessFilePath))
			};
		}

		public string ResolvePath(string basePath, string relativePath) {
			var path = Path.Combine(basePath ?? "", relativePath).Replace('\\', '/');

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
		public string BasePath { get; private set; } = "";
	}
}