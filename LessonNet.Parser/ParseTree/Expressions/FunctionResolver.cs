using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LinqExpr = System.Linq.Expressions.Expression;


namespace LessonNet.Parser.ParseTree.Expressions {
	public static class FunctionResolver {
		private static Lazy<Dictionary<string, Func<ListOfExpressionLists, Expression>>> functionLookup 
			= new Lazy<Dictionary<string,Func<ListOfExpressionLists,Expression>>>(CreateLookup);

		private static Dictionary<string, Func<ListOfExpressionLists, Expression>> FunctionLookup =>
			functionLookup.Value;

		private static Dictionary<string, Func<ListOfExpressionLists, Expression>> CreateLookup() {
			var baseType = typeof(LessFunction).GetTypeInfo();

			return typeof(FunctionResolver)
				.GetTypeInfo()
				.Assembly
				.GetTypes()
				.Where(t => baseType.IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract)
				.ToDictionary(t => t.Name.ToLowerInvariant().Replace("function", ""), CreateFactoryFunction);
		}

		private static Func<ListOfExpressionLists, Expression> CreateFactoryFunction(Type t) {
			var typeInfo = t.GetTypeInfo();

			var ctor = typeInfo.GetConstructor(new[] {typeof(ListOfExpressionLists)});

			var param = LinqExpr.Parameter(typeof(ListOfExpressionLists), "argumentList");

			return LinqExpr.Lambda<Func<ListOfExpressionLists, Expression>>(LinqExpr.New(ctor, param), param).Compile();
		}

		public static Expression Resolve(string functionName, ListOfExpressionLists arguments) {
			if (FunctionLookup.ContainsKey(functionName)) {
				return FunctionLookup[functionName](arguments);
			}

			return new CssFunction(functionName, arguments);
		}
	}
}