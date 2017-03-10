using System.IO;

namespace LessonNet.Parser {
	public interface IFileResolver {
		Stream GetContent();
		IFileResolver GetResolverFor(string lessFilePath);

		string CurrentFile { get; }
	}
}