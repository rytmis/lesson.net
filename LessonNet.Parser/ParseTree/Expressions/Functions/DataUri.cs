using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LessonNet.Parser.ParseTree.Expressions.Functions {
	[FunctionName("data-uri")]
	public class DataUri : LessFunction {
		public DataUri(Expression arguments) : base(arguments) { }

		protected override Expression EvaluateFunction(Expression arguments, EvaluationContext context) {
			var (type, path) = ParseArguments(context);

			try {
				using (var input = context.CurrentScope.FileResolver.GetContent(path))
				using (var memory = new MemoryStream(new byte[input.Length])) {
					input.CopyTo(memory);

					var base64 = Convert.ToBase64String(memory.ToArray());

					var encoding = $"data:{type};base64";

					return new Url(new LessString('"', new LessStringLiteral($"{encoding},{base64}")));
				}
			} catch (IOException ex) {
				throw new EvaluationException($"Error converting file {path} to data URI: {ex.Message}", ex);
			}
		}

		private (string path, string encoding) ParseArguments(EvaluationContext context) {
			if (Arguments is LessString str) {
				var path = str.GetUnquotedValue();

				var type = ContentTypeMap.GetContentType(path);
				return (type, path);

			}

			var (encArg, pathArg) = UnpackArguments<LessString, LessString>();

			return (encArg.GetUnquotedValue().Split(';').First(), pathArg.GetUnquotedValue());
		}
	}
}
