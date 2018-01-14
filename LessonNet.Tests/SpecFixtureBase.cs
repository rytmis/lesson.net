using System;
using System.Collections.Generic;
using System.Linq;
using LessonNet.Parser;
using LessonNet.Parser.CodeGeneration;
using LessonNet.Parser.ParseTree;
using Xunit;
using Xunit.Sdk;

namespace LessonNet.Tests
{

	public abstract class SpecFixtureBase
	{
		protected bool StrictMath { get; set; }
		protected virtual Dictionary<string, string> SetupImports() {
			// TODO: Actually set up imports in InMemoryFileResolver
			return null;
		}

		protected void AssertLess(string input, string expected) {
			var evaluatedCss = Evaluate(input);

			Assert.Equal(expected.Trim(), evaluatedCss.Trim());
		}

		protected void AssertExpressionUnchanged(string input) {
			AssertExpression(input, input);
		}

		protected void AssertExpression(string expected, string input) {
			string evaluated = Evaluate($".rule {{ property: {input} }} ");

			string actualResult = evaluated.Replace(".rule", "").Replace("property:", "").Trim('{', '}', '\r', '\n', '\t', ' ', ';');

			Assert.Equal(expected, actualResult);
		}

		protected void AssertRuleUnchanged(string input) {
			AssertRule(input, input);
		}

		protected void AssertRule(string expected, string input) {
			string evaluated = Evaluate($".rule {{ {input} }} ");

			string actualResult = evaluated.Replace(".rule", "").Trim('{', '}', '\r', '\n', '\t', ' ', ';');

			Assert.Equal(expected, actualResult);
		}

		protected void AssertExpression(string expected, string input, IDictionary<string, string> localVariables) {
			var actualResult = EvaluateExpression(input, localVariables);

			Assert.Equal(expected, actualResult);
		}

		protected string EvaluateExpression(string input, IDictionary<string, string> localVariables = null) {
			string variableDeclarations = "";
			if (localVariables != null) {
				variableDeclarations = string.Join("", localVariables.Select(kvp => $"@{kvp.Key}: {kvp.Value};"));
			}

			string evaluated = Evaluate($".rule {{ {variableDeclarations} property: {input} }} ");

			return evaluated
				.Replace(".rule", "")
				.Replace("property:", "")
				.Trim('{', '}', ' ', '\r', '\n', ';');
		}

		protected void AssertExpressionException<TException>(string input) where TException : Exception {
			AssertException<TException>($".rule {{ {input} }} ");
		}

		protected void AssertLessUnchanged(string input)
		{
			EvaluationContext context = new EvaluationContext(new LessTreeParser(), new InMemoryFileResolver(input));

			var currentStylesheet = context.ParseCurrentStylesheet(isReference: false);
			var evaluated = currentStylesheet.EvaluateSingle<Stylesheet>(context);

			var output = new OutputContext(' ', 2);
			output.Append(evaluated);
			
			Assert.Equal(input.Trim(), output.GetCss().Trim());
		}

		protected void AssertException<TException>(string input) where TException : Exception {
			Assert.Throws<TException>(() => Evaluate(input));
		}

		protected void AssertError(string expectedMessage, string expectedHighlight, int expectedLine, int expectedColumn, string input) {
			try {
				string result = Evaluate(input);

				throw new XunitException($"Expected error, but got: \n\n{result}");
			} catch (ParserException ex) {
				Assert.Equal(expectedLine, ex.Line);
				Assert.Equal(expectedColumn, ex.Column);
			}
		}

		protected string Evaluate(string input) {
			var context = CreateContext(input);

			var stylesheet = context.ParseCurrentStylesheet(isReference: false);
			var evaluated = stylesheet.EvaluateSingle<Stylesheet>(context);

			var output = context.GetOutputContext(' ', 2);
			output.Append(evaluated);

			return output.GetCss();
		}

		protected virtual EvaluationContext CreateContext(string input) {
			return new EvaluationContext(new LessTreeParser(), new InMemoryFileResolver(input) {
				Imports = SetupImports()
			}, StrictMath);
		}
	}
}