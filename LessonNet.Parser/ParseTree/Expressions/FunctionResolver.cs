using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqExpr = System.Linq.Expressions.Expression;


namespace LessonNet.Parser.ParseTree.Expressions {
	public static class FunctionResolver {
		private static Lazy<Dictionary<string, Func<Expression, Expression>>> functionLookup 
			= new Lazy<Dictionary<string,Func<Expression,Expression>>>(CreateLookup);

		private static Dictionary<string, Func<Expression, Expression>> FunctionLookup =>
			functionLookup.Value;

		private static Dictionary<string, Func<Expression, Expression>> CreateLookup() {
			var baseType = typeof(LessFunction).GetTypeInfo();

			return typeof(FunctionResolver)
				.GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => baseType.IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract)
				.Select(t => t.GetTypeInfo())
				.ToDictionary(GetFunctionName, CreateFactoryFunction);
		}

		private static string GetFunctionName(TypeInfo t) {
			var attr = t.GetCustomAttributes<FunctionNameAttribute>().SingleOrDefault();
			return attr?.Name
				?? t.Name.ToLowerInvariant().Replace("function", "");
		}

		private static Func<Expression, Expression> CreateFactoryFunction(TypeInfo t) {
			var ctor = t.GetConstructor(new[] {typeof(Expression)});

			var param = LinqExpr.Parameter(typeof(Expression), "argumentList");

			return LinqExpr.Lambda<Func<Expression, Expression>>(LinqExpr.New(ctor, param), param).Compile();
		}

		public static Expression Resolve(string functionName, Expression arguments) {
			if (FunctionLookup.ContainsKey(functionName)) {
				return FunctionLookup[functionName](arguments);
			}

			return new CssFunction(functionName, arguments);
		}
	}

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class FunctionNameAttribute : Attribute {
		public string Name { get; }

		public FunctionNameAttribute(string name) {
			this.Name = name;
		}
	}
}