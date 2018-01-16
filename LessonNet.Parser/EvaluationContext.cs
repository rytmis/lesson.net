﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree;
using LessonNet.Parser.ParseTree.Mixins;

namespace LessonNet.Parser
{
	public class EvaluationContext {
		public ExtenderRegistry Extenders { get; private set; } = new ExtenderRegistry();

		private Stack<Scope> scopeStack = new Stack<Scope>();

		public LessTreeParser Parser { get; }
		public IFileResolver FileResolver { get; }
		public bool StrictMath { get; }
		public Scope CurrentScope => scopeStack.Peek();

		public EvaluationContext(LessTreeParser parser, IFileResolver fileResolver, bool strictMath = false) {
			Parser = parser;
			FileResolver = fileResolver;
			StrictMath = strictMath;

			scopeStack.Push(new Scope(this));
		}

		public EvaluationContext GetImportContext(string importedLessFileName) {
			return new EvaluationContext(Parser, FileResolver.GetResolverFor(importedLessFileName), StrictMath) {
				scopeStack = scopeStack,
				Extenders = Extenders,
				RewriteRelativeUrls = RewriteRelativeUrls
			};
		}

		public Stylesheet ParseCurrentStylesheet(bool isReference) {
			using (var stream = FileResolver.GetContent()) {
				return Parser.Parse(FileResolver.CurrentFile, stream, isReference);
			}
		}

		public string GetFileContent() {
			using (var reader =  new StreamReader(FileResolver.GetContent())) {
				return reader.ReadToEnd();
			}
		}

		public OutputContext GetOutputContext(char indent, int indentationCount) {
			return new OutputContext(Extenders, indent, indentationCount);
		}

		public IDisposable EnterScope(SelectorList selectors) {
			scopeStack.Push(CurrentScope.CreateChildScope(selectors));

			return new ScopeGuard(scopeStack);
		}

		public IDisposable EnterClosureScope(Scope closure, IEnumerable<VariableDeclaration> localVariables = null) {
			scopeStack.Push(new ClosureScope(this, closure, CurrentScope, localVariables ));

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

		public bool IsReference { get; private set; }
		public bool RewriteRelativeUrls { get; set; } = true;

		public IDisposable BeginReferenceScope(bool isReference) {
			return new ReferenceScope(this, isReference);
		}

		private class ReferenceScope : IDisposable {
			private readonly EvaluationContext ctx;
			private readonly bool wasReference;

			public ReferenceScope(EvaluationContext ctx, bool isReference) {
				this.ctx = ctx;
				this.wasReference = ctx.IsReference;

				ctx.IsReference = isReference;
			}

			public void Dispose() => ctx.IsReference = wasReference;
		}
	}

	public class Scope {
		public SelectorList Selectors { get; }
		private SelectorList SelectorsWithoutCombinators;

		public Scope Parent { get; protected set; }

		public bool IsRoot => Parent == null;

		public EvaluationContext Context { get; }

		private IList<Scope> children = new List<Scope>();
		private IDictionary<string, VariableDeclaration> variables = new Dictionary<string, VariableDeclaration>();

		private IList<(MixinDefinition Mixin, Scope Closure)> mixinDefinitions = new List<(MixinDefinition, Scope)>();
		private IList<Ruleset> rulesets = new List<Ruleset>();


		public Scope(EvaluationContext context, SelectorList selectors = null, Scope parent = null) {
			this.Context = context;
			this.Selectors = selectors ?? SelectorList.Empty;
			this.SelectorsWithoutCombinators = selectors?.DropCombinators() ?? SelectorList.Empty;
			Parent = parent;
		}

		public virtual void DeclareMixin(MixinDefinition mixin) {
			DeclareMixinCore(mixin, null);
		}

		internal void DeclareMixinCore(MixinDefinition mixin, Scope closure) {
			mixinDefinitions.Add((mixin, closure));
		}

		public virtual void DeclareVariable(VariableDeclaration variable) {
			variables[variable.Name] = variable;
		}

		public virtual void DeclareRuleset(Ruleset ruleset) {
			rulesets.Add(ruleset);
		}

		public virtual IEnumerable<InvocationResult> ResolveMatchingMixins(MixinCall call, bool throwOnError = true) {
			var resolvedMixins = ResolveMixinsCore(call);
			if (!resolvedMixins.Any()) {
				if (call.Arguments.Count == 0) {
					var matchingRulesets = ResolveRulesetsCore(new RulesetCall(call.Selector, call.Important));
					if (matchingRulesets.Any()) {
						return matchingRulesets;
					}
				}

				if (throwOnError) {
					throw new EvaluationException($"No mixin found: {call} ");
				}
				return null;
			}


			return resolvedMixins;
		}

		public virtual IEnumerable<InvocationResult> ResolveMatchingRulesets(RulesetCall call, bool throwOnError = true) {
			var resolvedRulesets = ResolveRulesetsCore(call);
			if (!resolvedRulesets.Any()) {
				if (throwOnError) {
					throw new EvaluationException($"No matching ruleset found: {call} ");
				}
				return null;
			}

			return resolvedRulesets;
		}

		private IList<MixinEvaluationResult> ResolveMixinsCore(MixinCall call, bool resolveFromParents = true) {
			var guardScope = new MixinGuardScope();

			var matchingMixins = mixinDefinitions
				.Where(mixin => call.Matches(mixin.Mixin, Context))
				.OrderBy(mixin => mixin.Mixin.IsDefaultOverload)
				.Select(m => new MixinEvaluationResult(m.Mixin, call, m.Closure ?? this, guardScope))
				.Concat(ResolveInChildContexts(call));

			if (!resolveFromParents || Parent == null) {
				return matchingMixins.ToList();
			}

			return Parent.ResolveMixinsCore(call).Concat(matchingMixins).ToList();
		}

		private IEnumerable<MixinEvaluationResult> ResolveInChildContexts(MixinCall call) {
			foreach (var child in children) {
				foreach (var childSelector in child.SelectorsWithoutCombinators.Selectors) {
					if (childSelector.IsPrefixOf(call.Selector)) {
						var remainingSelectors = call.Selector.RemovePrefix(childSelector);

						foreach (var result in child.ResolveMixinsCore(new MixinCall(remainingSelectors, call.Arguments, call.Important), resolveFromParents: false)) {
							yield return result;
						}
					}
				}
			}
		}

		private IList<InvocationResult> ResolveRulesetsCore(RulesetCall call, bool resolveFromParents = true) {
			// No namespace support or result caching yet
			var matchingRulesets = rulesets
				.Where(call.Matches)
				.Select(m => new RulesetEvaluationResult(m, call, this));

			var guardScope = new MixinGuardScope();
			var matchingMixins = mixinDefinitions
				.Where(m => call.Matches(m.Mixin))
				.Select(m => new MixinEvaluationResult(m.Mixin, new MixinCall(call.Selector, Enumerable.Empty<PositionalArgument>(), call.Important), m.Closure ?? this, guardScope));

			var localResults = matchingRulesets
				.Concat<InvocationResult>(matchingMixins)
				.Concat(ResolveInChildContexts(call));

			if (!resolveFromParents || Parent == null) {
				return localResults.ToList();
			}

			return Parent.ResolveRulesetsCore(call).Concat(localResults).ToList();
		}

		private IEnumerable<InvocationResult> ResolveInChildContexts(RulesetCall call) {
			foreach (var child in children) {
				foreach (var childSelector in child.SelectorsWithoutCombinators.Selectors) {
					if (childSelector.IsPrefixOf(call.Selector)) {
						var remainingSelectors = call.Selector.RemovePrefix(childSelector);

						if (remainingSelectors != null && !remainingSelectors.IsEmpty()) {
							foreach (var result in child.ResolveRulesetsCore(new RulesetCall(remainingSelectors, call.Important),
								resolveFromParents: false)) {
								yield return result;
							}
						}
					}
				}
			}
		}


		public virtual VariableDeclaration ResolveVariable(string name, bool throwOnError = true) {
			if (variables.TryGetValue(name, out var variableDeclaration)) {
				return variableDeclaration;
			}

			var variableInParentScope = Parent?.ResolveVariable(name, throwOnError);
			if (variableInParentScope != null) {
				return variableInParentScope;
			}

			if (throwOnError) {
				throw new EvaluationException($"Undefined variable {name}");
			}

			return null;
		}

		public Scope CreateChildScope(SelectorList scopeSelectors) {
			var childScope = new Scope(Context, scopeSelectors, this);
			children.Add(childScope);
			return childScope;
		}

		public override string ToString() {
			if (Parent == null) {
				return "[root]";
			}

			return $"{Parent} -> {this.Selectors}";
		}
	}

	public class ClosureScope : Scope {
		private readonly Scope closure;

		public ClosureScope(EvaluationContext context, Scope closure, Scope overlay, IEnumerable<VariableDeclaration> localVariables) 
			: base(context, overlay.Selectors) {
			this.closure = closure;

			Parent = overlay;

			if (localVariables != null) {
				foreach (var variableDeclaration in localVariables) {
					base.DeclareVariable(variableDeclaration);
				}
			}
		}

		public override VariableDeclaration ResolveVariable(string name, bool throwOnError = true) {
			return base.ResolveVariable(name, throwOnError: false)
				?? closure.ResolveVariable(name, throwOnError: throwOnError);
		}

		public override IEnumerable<InvocationResult> ResolveMatchingMixins(MixinCall call, bool throwOnError = true) {
			return base.ResolveMatchingMixins(call, throwOnError: false)
				?? closure.ResolveMatchingMixins(call, throwOnError);
		}

		public override IEnumerable<InvocationResult> ResolveMatchingRulesets(RulesetCall call, bool throwOnError = true) {
			return base.ResolveMatchingRulesets(call, throwOnError: false)
				?? closure.ResolveMatchingRulesets(call, throwOnError);
		}

		public override void DeclareMixin(MixinDefinition mixin) {
			Parent.DeclareMixinCore(mixin, this);
		}

		public override void DeclareVariable(VariableDeclaration variable) {
			Parent.DeclareVariable(variable);
		}
	}
}
