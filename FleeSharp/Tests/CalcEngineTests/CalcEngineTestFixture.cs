
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
	public class CalcEngineTestFixture : ExpressionTests
	{

		[Test()]
		public void TestBasic()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 1);
			ce.Add("b", "a + y", context);

			ce.Add("c", "b * 2", context);

			ce.Recalculate("a");
			int result = ce.GetResult<int>("c");
			Assert.AreEqual(result, ((100 * 2) + 1) * 2);

			variables["x"] = 345;
			ce.Recalculate("a");
			result = ce.GetResult<int>("c");
			Assert.AreEqual(((345 * 2) + 1) * 2, result);
		}

		private IDynamicExpression CreateExpression(string expression, ExpressionContext context)
		{
			return context.CompileDynamic(expression);
		}

		[Test()]
		public void TestMutipleIdenticalReferences()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			ce.Add("b", "a + a + a", context);

			ce.Recalculate("a");
			int result = ce.GetResult<int>("b");
			Assert.AreEqual((100 * 2) * 3, result);
		}

		[Test()]
		public void TestComplex()
		{
			CalculationEngine ce = new CalculationEngine();

			ExpressionContext context = new ExpressionContext();

			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 24);
			ce.Add("b", "y * 2", context);

			ce.Add("c", "a + b", context);

			ce.Add("d", "80", context);

			ce.Add("e", "a + b + c + d", context);

			ce.Recalculate("d");
			ce.Recalculate(new string[] {
				"a",
				"b"
			});

			int result = ce.GetResult<int>("e");
			Assert.AreEqual((100 * 2) + (24 * 2) + ((100 * 2) + (24 * 2)) + 80, result);
		}

		[Test()]
		public void TestRecalculateNonSource()
		{
			// Test recalculate with a non-source
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 1);
			ce.Add("b", "a + y", context);

			ce.Add("c", "b * 2", context);

			ce.Recalculate("a", "b");
			int result = ce.GetResult<int>("c");
			Assert.AreEqual(((100) * 2 + 1) * 2, result);
		}

		[Test()]
		public void TestPartialRecalculate()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 1);
			ce.Add("b", "a + y", context);

			ce.Add("c", "b * 2", context);

			ce.Recalculate("a");

			variables["y"] = 222;
			ce.Recalculate("b");

			int result = ce.GetResult<int>("c");
			Assert.AreEqual(((100 * 2) + 222) * 2, result);
		}

		[Test(), ExpectedException(typeof(CircularReferenceException))]
		public void TestCircularReference1()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 1);
			ce.Add("b", "a + y + b", context);

			ce.Recalculate("a");
		}

		[Test(), ExpectedException(typeof(CircularReferenceException))]
		public void TestCircularReference2()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", 100);
			ce.Add("a", "x * 2", context);

			variables.Add("y", 1);
			ce.Add("b", "a + y + b", context);

			ce.Recalculate("b");
		}

		[Test()]
		public void TestWithReferenceTypes()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("x", "string");
			ce.Add("a", "x + \" \"", context);

			variables.Add("y", "word");
			ce.Add("b", "y", context);

			ce.Add("c", "a + b", context);

			ce.Recalculate("b", "a");

			string result = ce.GetResult<string>("c");
			Assert.AreEqual("string" + " " + "word", result);
		}

		[Test()]
		public void TestRemove1()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			ce.Add("a", "100", context);
			ce.Add("b", "200", context);
			ce.Add("c", "a + b", context);
			ce.Add("d", "300", context);
			ce.Add("e", "c + d", context);

			ce.Remove("a");
			// Only b and d should remain
			Assert.AreEqual(2, ce.Count);

			ce.Remove("b");
			Assert.AreEqual(1, ce.Count);

			ce.Remove("d");
			Assert.AreEqual(0, ce.Count);

			// b and d should have no dependents or precedents
			Assert.IsFalse(ce.HasDependents("b"));
			Assert.IsFalse(ce.HasDependents("d"));
			Assert.IsFalse(ce.HasPrecedents("b"));
			Assert.IsFalse(ce.HasPrecedents("d"));
		}

		[Test()]
		public void TestRemove2()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			ce.Add("a", "100", context);
			ce.Add("b", "200", context);
			ce.Add("c", "a + b", context);
			ce.Add("d", "300", context);
			ce.Add("e", "c + d + a", context);

			ce.Remove("a");
			// Only b and d should remain
			Assert.AreEqual(2, ce.Count);
			ce.Remove("b");
			Assert.AreEqual(1, ce.Count);
			ce.Remove("d");
			Assert.AreEqual(0, ce.Count);
		}

		[Test()]
		public void TestRemove3()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			ce.Add("a", "100", context);
			ce.Add("b", "200", context);
			ce.Add("c", "a + b", context);
			ce.Add("d", "300 + c", context);
			ce.Add("e", "c + d", context);

			ce.Remove("d");
			Assert.AreEqual(3, ce.Count);

			ce.Recalculate("c");
			ce.Remove("c");
			Assert.AreEqual(2, ce.Count);

			ce.Remove("a");
			Assert.AreEqual(1, ce.Count);

			ce.Remove("b");
			Assert.AreEqual(0, ce.Count);
		}

		[Test()]
		public void TestRemove4()
		{
			CalculationEngine ce = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			ce.Add("a", "100", context);
			ce.Add("b", "200", context);
			ce.Add("c", "a + b", context);
			ce.Add("d", "300 + c", context);
			ce.Add("e", "c + d", context);

			ce.Remove("a");
			Assert.AreEqual(1, ce.Count);

			ce.Remove("b");
			Assert.AreEqual(0, ce.Count);
		}

		[Test()]
		public void TestBatchLoad()
		{
			// Test that we can add expressions in any order
			CalculationEngine engine = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();

			int interest = 2;
			context.Variables.Add("interest", interest);

			BatchLoader loader = engine.CreateBatchLoader();

			loader.Add("c", "a + b", context);
			loader.Add("a", "100 + interest", context);
			loader.Add("b", "a + 1 + a", context);
			// Test an expression with a reference in a string
			loader.Add("d", "\"str \\\" str\" + a + \"b\"", context);

			engine.BatchLoad(loader);

			int result = engine.GetResult<int>("b");
			Assert.AreEqual((100 + interest) + 1 + (100 + interest), result);

			interest = 300;
			context.Variables["interest"] = interest;
			engine.Recalculate("a");

			result = engine.GetResult<int>("b");
			Assert.AreEqual((100 + interest) + 1 + (100 + interest), result);

			result = engine.GetResult<int>("c");
			Assert.AreEqual((100 + interest) + 1 + (100 + interest) + (100 + interest), result);

			Assert.AreEqual("str \" str400b", engine.GetResult<string>("d"));
		}

		[Test()]
		public void TestCalcEngineAtom()
		{
			// Test that calc engine atom reference work properly
			CalculationEngine engine = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();

			engine.Add("a", "\"abc\"", context);
			engine.Add("b", "a.length", context);
			engine.Add("c", "a.startswith(\"a\")", context);

			int result = engine.GetResult<int>("b");
			Assert.AreEqual("abc".Length, result);

			Assert.AreEqual(true, engine.GetResult<bool>("c"));
		}

		[Test()]
		public void TestDependencyManagement()
		{
			// Test that we are keeping accurate stats on our dependencies

			CalculationEngine engine = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();

			engine.Add("a", "100", context);
			engine.Add("b", "100", context);

			// Nothing should point to a and b
			Assert.IsFalse(engine.HasPrecedents("a"));
			Assert.IsFalse(engine.HasPrecedents("b"));
			Assert.IsFalse(engine.HasDependents("a"));
			Assert.IsFalse(engine.HasDependents("b"));

			engine.Add("c", "a + b", context);
			engine.Add("d", "a + c", context);

			// a and b still have nothing pointing to them
			Assert.IsFalse(engine.HasPrecedents("a"));
			Assert.IsFalse(engine.HasPrecedents("b"));
			// but they have dependents
			Assert.IsTrue(engine.HasDependents("a"));
			Assert.IsTrue(engine.HasDependents("b"));

			// c and d have precedents
			Assert.IsTrue(engine.HasPrecedents("d"));
			Assert.IsTrue(engine.HasPrecedents("c"));
			// and only c should have dependents
			Assert.IsTrue(engine.HasDependents("c"));
			Assert.IsFalse(engine.HasDependents("d"));

			// test our counts
			Assert.AreEqual(2, engine.GetDependents("a").Length);
			Assert.AreEqual(1, engine.GetDependents("b").Length);
			Assert.AreEqual(1, engine.GetDependents("c").Length);
			Assert.AreEqual(0, engine.GetDependents("d").Length);

			Assert.AreEqual(0, engine.GetPrecedents("a").Length);
			Assert.AreEqual(0, engine.GetPrecedents("b").Length);
			Assert.AreEqual(2, engine.GetPrecedents("c").Length);
			Assert.AreEqual(2, engine.GetPrecedents("d").Length);

			engine.Remove("d");

			Assert.IsFalse(engine.HasDependents("c"));
			Assert.IsFalse(engine.HasDependents("d"));
			Assert.IsFalse(engine.HasPrecedents("d"));
			Assert.IsTrue(engine.HasPrecedents("c"));

			Assert.AreEqual(1, engine.GetDependents("a").Length);
			Assert.AreEqual(1, engine.GetDependents("b").Length);
			Assert.AreEqual(0, engine.GetDependents("c").Length);

			engine.Remove("c");

			Assert.IsFalse(engine.HasPrecedents("c"));
			Assert.IsFalse(engine.HasDependents("c"));
			Assert.IsFalse(engine.HasDependents("a"));
			Assert.IsFalse(engine.HasDependents("b"));

			Assert.AreEqual(0, engine.GetDependents("a").Length);
			Assert.AreEqual(0, engine.GetDependents("b").Length);

			engine.Remove("a");
			engine.Remove("b");

			Assert.IsFalse(engine.HasDependents("a"));
			Assert.IsFalse(engine.HasPrecedents("a"));
			Assert.IsFalse(engine.HasDependents("b"));
			Assert.IsFalse(engine.HasPrecedents("b"));

			Assert.AreEqual(0, engine.GetDependents("a").Length);
			Assert.AreEqual(0, engine.GetDependents("b").Length);
			Assert.AreEqual(0, engine.GetPrecedents("a").Length);
			Assert.AreEqual(0, engine.GetPrecedents("b").Length);
		}

		[Test()]
		public void TestInfoMethodsWithMissing()
		{
			// Test that our informational methods can be called with a non-existant expression
			CalculationEngine engine = new CalculationEngine();

			Assert.IsFalse(engine.HasDependents("zz"));
			Assert.IsFalse(engine.HasPrecedents("zz"));
			Assert.AreEqual(0, engine.GetDependents("zz").Length);
			Assert.AreEqual(0, engine.GetPrecedents("zz").Length);
		}

		[Test()]
		public void TestClear()
		{
			CalculationEngine engine = new CalculationEngine();
			ExpressionContext context = new ExpressionContext();

			engine.Add("a", "100", context);
			engine.Add("b", "a + 2", context);

			engine.Clear();

			Assert.IsFalse(engine.HasDependents("a"));
			Assert.IsFalse(engine.HasPrecedents("b"));
			Assert.AreEqual(0, engine.Count);
		}
	}
}
