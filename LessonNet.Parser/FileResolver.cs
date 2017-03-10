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

		public Stream GetContent() {
			return File.OpenRead(lessFilePath);
		}

		public IFileResolver GetResolverFor(string lessFilePath) {
			string path = Path.GetFullPath(Path.Combine(rootPath, lessFilePath));

			return new FileResolver(path);
		}
	}
}