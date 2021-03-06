﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LessonNet.Parser.ParseTree;

namespace LessonNet.Parser.CodeGeneration
{
	public class OutputContext : IExtensionContext
	{
		public ExtenderRegistry Extensions { get; set; }
		private readonly char indentChar;
		private readonly int indentCount;
		private readonly bool compress;
		private short indentLevel = 0;

		private readonly StringBuilder outputBuffer = new StringBuilder();

		internal OutputContext(ExtenderRegistry extensions, char indentChar, int indentCount, bool compress) {
			this.Extensions = extensions ?? new ExtenderRegistry();
			this.indentChar = indentChar;
			this.indentCount = indentCount;
			this.compress = compress;
		}

		public OutputContext(char indentChar, int indentCount) : this(null, indentChar, indentCount, false) {
		}
		
		public bool IsReference { get; private set; }

		public void AppendOptional(char input) {
			if (!compress) {
				outputBuffer.Append(input);
			}
		}

		public void Append(char input) {
			outputBuffer.Append(input);
		}

		public void Append(string input) {
			outputBuffer.Append(input);
		}

		public void AppendLine(string input) {
			outputBuffer.Append(input);
			if (!compress) {
				outputBuffer.Append(Environment.NewLine);
			}
		}

		public void AppendLine() {
			if (!compress) {
				outputBuffer.Append(Environment.NewLine);
			}
		}

		public bool Append(LessNode node) {
			int previousLength = outputBuffer.Length;

			node.WriteOutput(this);

			return previousLength != outputBuffer.Length;
		}

		public void Indent() {
			if (compress) {
				return;
			}

			int indentCharCount = indentLevel * indentCount;
			for (int i = 0; i < indentCharCount; i++) {
				outputBuffer.Append(indentChar);
			}
		}

		public void IncreaseIndentLevel() {
			indentLevel++;
		}

		public void DecreaseIndentLevel() {
			indentLevel--;
		}

		public string GetCss() => outputBuffer.ToString();

		public UndoScope BeginUndoableScope() {
			return new UndoScope(this);
		}

		public class UndoScope : IDisposable {
			private readonly OutputContext context;
			private readonly int originalLength;
			private bool isCommitted = false;

			public UndoScope(OutputContext context) {
				this.context = context;
				this.originalLength = context.outputBuffer.Length;
			}

			public void KeepChanges() {
				this.isCommitted = true;
			}

			public void Dispose() {
				if (!isCommitted) {
					context.outputBuffer.Length = originalLength;
				}
			}
		}

		public IDisposable BeginReferenceScope(bool isReference) => new ReferenceScope(this, isReference);

		public class ReferenceScope : IDisposable {
			private readonly OutputContext ctx;
			private readonly bool wasReference;

			public ReferenceScope(OutputContext ctx, bool isReference) {
				this.ctx = ctx;
				this.wasReference = ctx.IsReference;

				ctx.IsReference = isReference;
			}

			public void Dispose() => ctx.IsReference = wasReference;
		}

		public IDisposable EnterExtenderScope(ExtenderRegistry extenders)
		{
			return new ExtenderScope(this, extenders);
		}
	}

	public interface IExtensionContext
	{
		ExtenderRegistry Extensions { get; set; }
	}

	public class ExtenderScope : IDisposable
	{
		private readonly IExtensionContext ctx;
		private readonly ExtenderRegistry originalExtensions;

		public ExtenderScope(IExtensionContext ctx) : this(ctx, new ExtenderRegistry(ctx.Extensions))
		{
		}

		public ExtenderScope(IExtensionContext ctx, ExtenderRegistry newScope)
		{
			this.ctx = ctx;
			this.originalExtensions = this.ctx.Extensions;
			// If we don't actually have a new scope, then this is a no-op.
			this.ctx.Extensions = newScope ?? this.originalExtensions;
		}

		public void Dispose()
		{
			this.ctx.Extensions = this.originalExtensions;
		}
	}

	public class ExtenderRegistry {
		private readonly ExtenderRegistry parent;

		private readonly List<(Extender Extender, Selector Selector, bool isReference)> extensions 
			= new List<(Extender, Selector, bool)>();

		public ExtenderRegistry() { }

		public ExtenderRegistry(ExtenderRegistry parent) {
			this.parent = parent;
		}

		public void Add(Extender target, Selector extension, bool isReference) {
			extensions.Add((target, extension, isReference));
		}

		public IEnumerable<Selector> GetExtensions(Selector candidate, bool includeReferences) {
			foreach (var extension in extensions) {
				if (extension.isReference && !includeReferences) {
					continue;
				}

				if (!extension.Extender.PartialMatch && candidate.Equals(extension.Extender.Target)) {
					yield return extension.Selector;
				} else if (extension.Extender.PartialMatch && candidate.Contains(extension.Extender.Target)) {
					yield return candidate.Replace(extension.Extender.Target, extension.Selector);
				}
			}

			if (parent == null) {
				yield break;
			}

			foreach (var extension in parent.GetExtensions(candidate, includeReferences)) {
				yield return extension;
			}
		}
	}
}
