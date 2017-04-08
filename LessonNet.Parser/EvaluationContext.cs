﻿using System;
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

		public IDisposable EnterClosureScope(Scope closure) {
			scopeStack.Push(new ClosureScope(closure, CurrentScope));

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

		private IList<MixinDefinition> mixins = new List<MixinDefinition>();

		public Scope(SelectorList selectors = null, Scope parent = null) {
			this.selectors = selectors;
			Parent = parent;
		}

		public Scope Parent { get; protected set; }

		public virtual void DeclareMixin(MixinDefinition mixin) {
			mixins.Add(mixin);
		}

		public virtual void DeclareVariable(VariableDeclaration variable) {
			variables[variable.Name] = variable;
		}

		public virtual IEnumerable<MixinEvaluationResult> ResolveMatchingMixins(MixinCall call) {
			var resolvedMixins = ResolveMixinsCore(call);
			if (!resolvedMixins.Any()) {
				throw new EvaluationException($"No mixin found: {call} ");
			}

			return resolvedMixins;
		}

		private IList<MixinEvaluationResult> ResolveMixinsCore(MixinCall call) {
			// No namespace support or result caching yet
			var matchingMixins = mixins
				.Where(call.Matches)
				.Select(m => new MixinEvaluationResult(m, call, this));

			if (Parent == null) {
				return matchingMixins.ToList();
			}

			return Parent.ResolveMatchingMixins(call).Concat(matchingMixins).ToList();
		}

		public virtual VariableDeclaration ResolveVariable(string name, bool throwOnError = true) {
			if (variables.TryGetValue(name, out var variableDeclaration)) {
				return variableDeclaration;
			}

			var variableInParentScope = Parent?.ResolveVariable(name);
			if (variableInParentScope != null) {
				return variableInParentScope;
			}

			if (throwOnError) {
				throw new EvaluationException($"Undefined variable {name}");
			}

			return null;
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

	public class ClosureScope : Scope {
		private readonly Scope closure;

		public ClosureScope(Scope closure, Scope overlay) {
			this.closure = closure;

			Parent = overlay;
		}

		public override VariableDeclaration ResolveVariable(string name, bool throwOnError = true) {
			return base.ResolveVariable(name, throwOnError: false)
				?? closure.ResolveVariable(name, throwOnError: throwOnError);
		}
	}
}
