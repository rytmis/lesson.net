using System.IO;

namespace LessonNet.Parser {
	public interface IFileResolver {
		Stream GetContent();
		IFileResolver GetResolverFor(string lessFilePath);

		string CurrentFile { get; }
		string BasePath { get; }
		string ResolvePath(string basePath);
	}
}