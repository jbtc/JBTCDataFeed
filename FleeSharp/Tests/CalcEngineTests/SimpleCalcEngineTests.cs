
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using Ciloci.Flee.CalcEngine;
namespace Ciloci.Flee.Tests
{

	[TestFixture()]
	public class SimpleCalcEngineTests : ExpressionTests
	{


		private SimpleCalcEngine MyEngine;
		public SimpleCalcEngineTests()
		{
			SimpleCalcEngine engine = new SimpleCalcEngine();
			ExpressionContext context = new ExpressionContext();
			context.Imports.AddType(typeof(Math));
			context.Imports.AddType(typeof(Math), "math");

			engine.Context = context;
			MyEngine = engine;
		}

		[Test()]
		public void TestScripts()
		{
			this.ProcessScriptTests("SimpleCalcEngineTests.txt", SimpleCalcEngineTestsProcessor);
		}

		private void SimpleCalcEngineTestsProcessor(string[] lineParts)
		{
			string[] expressions = lineParts[1].Split('|');

			MyEngine.Clear();

			this.AddExpressions(expressions);

			this.Evaluate(lineParts[0]);
		}

		private void AddExpressions(string[] expressions)
		{
			foreach (string expression in expressions) {
				string[] arr = expression.Split(':');

				string name = arr[0];

				string[] arr2 = arr[1].Split('?');
				string text = arr2[0];

				if (arr2.Length > 1) {
					this.AddVariables(arr2[1]);
				}

				MyEngine.AddDynamic(name, text);
			}
		}

		private void Evaluate(string data)
		{
			IDictionary<string, object> results = ParseQueryString(data);

			foreach (KeyValuePair<string, object> entry in results) {
                IDynamicExpression e = MyEngine[entry.Key] as IDynamicExpression;
				object expectedResult = entry.Value;
				object result = e.Evaluate();
				Assert.AreEqual(expectedResult, result);
			}
		}

		private void AddVariables(string variablesText)
		{
			IDictionary<string, object> variables = ParseQueryString(variablesText);

			foreach (KeyValuePair<string, object> entry in variables) {
				MyEngine.Context.Variables.Add(entry.Key, entry.Value);
			}
		}
	}
}
