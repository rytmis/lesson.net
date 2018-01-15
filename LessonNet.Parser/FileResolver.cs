using System.IO;

namespace LessonNet.Parser {
	public class FileResolver : IFileResolver {
		private readonly string lessFilePath;
		private readonly string rootPath;

		public FileResolver(string lessFilePath) {
			this.lessFilePath = lessFilePath;

			rootPath = Path.GetDirectoryName(lessFilePath);
		}

		public string CurrentFile => lessFilePath;
		public string BasePath { get; private set; }

		public Stream GetContent() {
			return File.OpenRead(lessFilePath);
		}

		public IFileResolver GetResolverFor(string lessFilePath, string basePathOverride = null) {
			string path = Path.GetFullPath(Path.Combine(rootPath, lessFilePath));

			return new FileResolver(path);
		}
	}
}