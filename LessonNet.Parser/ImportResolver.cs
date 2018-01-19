using System.IO;

namespace LessonNet.Parser {
	public interface IFileResolver {
		/// <summary>
		/// Gets the content of the current less file
		/// </summary>
		Stream GetContent();

		/// <summary>
		/// Gets the content of a file relative to the current less file
		/// </summary>
		Stream GetContent(string path);

		IFileResolver GetResolverFor(string lessFilePath);

		string CurrentFile { get; }
		string BasePath { get; }
		string ResolvePath(string basePath);
	}
}