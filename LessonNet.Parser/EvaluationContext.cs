using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LessonNet.Parser.ParseTree;

namespace LessonNet.Parser
{
	public class EvaluationContext
	{
		private Stack<Scope> scopeStack = new Stack<Scope>();

		public LessTreeParser Parser { get; }
		public IFileResolver FileResolver { get; }

		public Scope CurrentScope => scopeStack.Peek();

		public EvaluationContext(LessTreeParser parser, IFileResolver fileResolver) {
			Parser = parser;
			FileResolver = fileResolver;

			scopeStack.Push(new Scope());
		}

		public EvaluationContext GetImportContext(string importedLessFileName) {
			return new EvaluationContext(Parser, FileResolver.GetResolverFor(importedLessFileName));
		}

		public Stylesheet ParseCurrentStylesheet() {
			using (var stream = FileResolver.GetContent()) {
				return Parser.Parse(FileResolver.CurrentFile, stream);
			}
		}

		public IDisposable EnterScope(SelectorList selectors) {
			scopeStack.Push(CurrentScope.CreateChildScope(selectors));

			return new ScopeGuard(scopeStack);
		}

		private class ScopeGuard : IDisposable {
			private readonly Stack<Scope> stack;

			public ScopeGuard(Stack<Scope> stack) {
				this.stack = stack;
			}

			public void Dispose() {
				stack.Pop();
			}
		}
	}

	public class Scope {
		private readonly SelectorList selectors;

		private IList<Scope> children = new List<Scope>();
		private IDictionary<string, VariableDeclaration> variables = new Dictionary<string, VariableDeclaration>();

		public Scope(SelectorList selectors = null, Scope parent = null) {
			this.selectors = selectors;
			Parent = parent;
		}

		public Scope Parent { get; }

		public void DeclareMixin(string selector, MixinDefinition mixin) {
			
		}

		public void DeclareVariable(VariableDeclaration variable) {
			variables[variable.Name] = variable;
		}

		public VariableDeclaration ResolveVariable(string name) {
			if (variables.TryGetValue(name, out var variableDeclaration)) {
				return variableDeclaration;
			}

			return Parent?.ResolveVariable(name) ?? throw new EvaluationException($"Undefined variable {name}");
		}

		public Scope CreateChildScope(SelectorList selectors) {
			var childScope = new Scope(selectors, this);
			children.Add(childScope);
			return childScope;
		}

		public override string ToString() {
			if (Parent == null) {
				return "[root]";
			}

			return $"{Parent} -> {this.selectors}";
		}
	}
}
