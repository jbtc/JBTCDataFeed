
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using Ciloci.Flee;
using System.Globalization;
using System.Threading;
using System.Reflection;
namespace Ciloci.Flee.Tests
{

	[TestFixture()]
	public class IndividualTests : ExpressionTests
	{

		[Test(Description = "Test that we properly handle newline escapes in strings")]
		public void TestStringNewlineEscape()
		{
			IGenericExpression<string> e = this.CreateGenericExpression<string>("\"a\\r\\nb\"");
			string s = e.Evaluate();
            string expected = string.Format("a{0}b", System.Environment.NewLine);
			Assert.AreEqual(expected, s);
		}

		[Test(Description = "Test that we can parse from multiple threads")]
		public void TestMultiTreadedParse()
		{
			Thread t1 = new Thread(ThreadRunParse);
			t1.Name = "Thread1";

			Thread t2 = new Thread(ThreadRunParse);
			t2.Name = "Thread2";

			ExpressionContext context = new ExpressionContext();

			t1.Start(context);
			t2.Start(context);

			t1.Join();
			t2.Join();
		}

		private void ThreadRunParse(object o)
		{
			ExpressionContext context = o as ExpressionContext;
			// Test parse
			for (int i = 0; i <= 40 - 1; i++) {
				IDynamicExpression e = this.CreateDynamicExpression("1+1*200", context);
			}
		}

		[Test(Description = "Test that we can parse from multiple threads")]
		public void TestMultiTreadedEvaluate()
		{
			System.Threading.Thread t1 = new System.Threading.Thread(ThreadRunEvaluate);
			t1.Name = "Thread1";

			System.Threading.Thread t2 = new System.Threading.Thread(ThreadRunEvaluate);
			t2.Name = "Thread2";

			IDynamicExpression e = this.CreateDynamicExpression("1+1*200");

			t1.Start(e);
			t2.Start(e);

			t1.Join();
			t2.Join();
		}

		private void ThreadRunEvaluate(object o)
		{
			// Test evaluate
			IDynamicExpression e2 = o as IDynamicExpression;

			for (int i = 0; i <= 40 - 1; i++) {
				int result = (int)e2.Evaluate();
				Assert.AreEqual(1 + 1 * 200, result);
			}
		}

		[Test(Description = "Test evaluation of generic expressions")]
		public void TestGenericEvaluate()
		{
			ExpressionContext context = null;
			context = new ExpressionContext();

			context.Options.ResultType = typeof(object);

			IGenericExpression<int> e1 = this.CreateGenericExpression<int>("1000", context);
			Assert.AreEqual(1000, e1.Evaluate());

			IGenericExpression<double> e2 = this.CreateGenericExpression<double>("1000.25", context);
			Assert.AreEqual(1000.25, e2.Evaluate());

			IGenericExpression<double> e3 = this.CreateGenericExpression<double>("1000", context);
			Assert.AreEqual(1000.0, e3.Evaluate());

			IGenericExpression<ValueType> e4 = this.CreateGenericExpression<ValueType>("1000", context);
			ValueType vt = e4.Evaluate();
			Assert.AreEqual(1000, vt);

			IGenericExpression<object> e5 = this.CreateGenericExpression<object>("1000 + 2.5", context);
			object o = e5.Evaluate();
			Assert.AreEqual(1000 + 2.5, o);
		}

		[Test(Description = "Test expression imports")]
		public void TestImports()
		{
			IGenericExpression<double> e = null;
			ExpressionContext context = null;

			context = new ExpressionContext();
			// Import math type directly
			context.Imports.AddType(typeof(Math));

			// Should be able to see PI without qualification
			e = this.CreateGenericExpression<double>("pi", context);
			Assert.AreEqual(Math.PI, e.Evaluate());

			context = new ExpressionContext(MyValidExpressionsOwner);
			// Import math type with prefix
			context.Imports.AddType(typeof(Math), "math");

			// Should be able to see pi by qualifying with Math	
			e = this.CreateGenericExpression<double>("math.pi", context);
			Assert.AreEqual(Math.PI, e.Evaluate());

			// Import nothing
			context = new ExpressionContext();
			// Should not be able to see PI
			this.AssertCompileException("pi");
			this.AssertCompileException("math.pi");

			// Test importing of builtin types
			context = new ExpressionContext();
			context.Imports.ImportBuiltinTypes();

			this.CreateGenericExpression<double>("double.maxvalue", context);
			this.CreateGenericExpression<string>("string.concat(\"a\", \"b\")", context);
			this.CreateGenericExpression<long>("long.maxvalue * 2", context);
		}

		[Test(Description = "Test importing of a method")]
		public void TestImportMethod()
		{
			ExpressionContext context = new ExpressionContext();
			context.Imports.AddMethod("cos", typeof(Math), string.Empty);

			IDynamicExpression e = this.CreateDynamicExpression("cos(100)", context);
			Assert.AreEqual(Math.Cos(100), (double)e.Evaluate());

			System.Reflection.MethodInfo mi = typeof(int).GetMethod("parse", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.IgnoreCase, null, System.Reflection.CallingConventions.Any, new Type[] { typeof(string) }, null);
			context.Imports.AddMethod(mi, "");

			e = this.CreateDynamicExpression("parse(\"123\")", context);
			Assert.AreEqual(123, (int)e.Evaluate());
		}

		[Test(Description = "Test that we can import multiple types into a namespace")]
		public void TestImportsNamespaces()
		{
			ExpressionContext context = new ExpressionContext();
			context.Imports.AddType(typeof(Math), "ns1");
			context.Imports.AddType(typeof(string), "ns1");

			IDynamicExpression e = this.CreateDynamicExpression("ns1.cos(100)", context);
			Assert.AreEqual(Math.Cos(100), (double)e.Evaluate());

			e = this.CreateDynamicExpression("ns1.concat(\"a\", \"b\")", context);
			Assert.AreEqual(string.Concat("a", "b"), (string)e.Evaluate());
		}

		[Test(Description = "Test our string equality")]
		public void TestStringEquality()
		{
			ExpressionContext context = new ExpressionContext();
			ExpressionOptions options = context.Options;

			IGenericExpression<bool> e = null;

			// Should be equal
			e = this.CreateGenericExpression<bool>("\"abc\" = \"abc\"", context);
			Assert.IsTrue(e.Evaluate());

			// Should not be equal
			e = this.CreateGenericExpression<bool>("\"ABC\" = \"abc\"", context);
			Assert.IsFalse(e.Evaluate());

			// Should be not equal
			e = this.CreateGenericExpression<bool>("\"ABC\" <> \"abc\"", context);
			Assert.IsTrue(e.Evaluate());

			// Change string compare type
			options.StringComparison = StringComparison.OrdinalIgnoreCase;

			// Should be equal
			e = this.CreateGenericExpression<bool>("\"ABC\" = \"abc\"", context);
			Assert.IsTrue(e.Evaluate());

			// Should also be equal
			e = this.CreateGenericExpression<bool>("\"ABC\" <> \"abc\"", context);
			Assert.IsFalse(e.Evaluate());

			// Should also be not equal
			e = this.CreateGenericExpression<bool>("\"A\" <> \"z\"", context);
			Assert.IsTrue(e.Evaluate());
		}

		[Test(Description = "Test expression variables")]
		public void TestVariables()
		{
			this.TestValueTypeVariables();
			this.TestReferenceTypeVariables();
		}

		private void TestValueTypeVariables()
		{
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("a", 100);
			variables.Add("b", -100);
			variables.Add("c", DateTime.Now);

			IGenericExpression<int> e1 = this.CreateGenericExpression<int>("a+b", context);
			int result = e1.Evaluate();
			Assert.AreEqual(100 + -100, result);

			variables["B"] = 1000;
			result = e1.Evaluate();
			Assert.AreEqual(100 + 1000, result);

			IGenericExpression<string> e2 = this.CreateGenericExpression<string>("c.tolongdatestring() + c.Year.tostring()", context);
			Assert.AreEqual(DateTime.Now.ToLongDateString() + DateTime.Now.Year, e2.Evaluate());

			// Test null value
			variables["a"] = null;
			e1 = this.CreateGenericExpression<int>("a", context);
			Assert.AreEqual(0, e1.Evaluate());
		}

		private void TestReferenceTypeVariables()
		{
			ExpressionContext context = new ExpressionContext();
			VariableCollection variables = context.Variables;

			variables.Add("a", "string");
			variables.Add("b", 100);

			IGenericExpression<string> e = this.CreateGenericExpression<string>("a + b + a.tostring()", context);
			string result = e.Evaluate();
			Assert.AreEqual("string" + 100 + "string", result);

			variables["a"] = "test";
			variables["b"] = 1;
			result = e.Evaluate();
			Assert.AreEqual("test" + 1 + "test", result);

			// Test null value
			variables["nullVar"] = string.Empty;
			variables["nullvar"] = null;

			IGenericExpression<bool> e2 = this.CreateGenericExpression<bool>("nullvar = null", context);
			Assert.IsTrue(e2.Evaluate());
		}

		[Test(Description = "Test that we properly enforce member access on the expression owner")]
		public void TestMemberAccess()
		{
			AccessTestExpressionOwner owner = new AccessTestExpressionOwner();
			ExpressionContext context = new ExpressionContext(owner);
			ExpressionOptions options = context.Options;
			IDynamicExpression e = null;

			// Private field, access should be denied
			this.AssertCompileException("privateField1", context);

			// Private field but with override allowing access
			e = this.CreateDynamicExpression("privateField2", context);

			options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

			// Private field but with override denying access
			this.AssertCompileException("privateField3", context);

			options.OwnerMemberAccess = System.Reflection.BindingFlags.Default;

			// Public field, access should be denied
			this.AssertCompileException("PublicField1", context);

			options.OwnerMemberAccess = System.Reflection.BindingFlags.Public;
			// Public field, access should be allowed
			e = this.CreateDynamicExpression("publicField1", context);
		}

		[Test(Description = "Test parsing for different culture")]
		public void TestCultureSensitiveParse()
		{
			ExpressionContext context = new ExpressionContext();
			context.Options.ParseCulture = CultureInfo.GetCultureInfo("fr-FR");
			context.Imports.AddType(typeof(string), "String");
			context.Imports.AddType(typeof(Math), "Math");

			IGenericExpression<double> e = this.CreateGenericExpression<double>("1,25 + 0,75", context);
			Assert.AreEqual(1.25 + 0.75, e.Evaluate());

			e = this.CreateGenericExpression<double>("math.pow(1,25 + 0,75; 2)", context);
			Assert.AreEqual(Math.Pow(1.25 + 0.75, 2), e.Evaluate());

			IGenericExpression<string> e2 = this.CreateGenericExpression<string>("string.concat(1;2;3;4)", context);
			Assert.AreEqual("1234", e2.Evaluate());
		}

		[Test(Description = "Test tweaking of parser options")]
		public void TestParserOptions()
		{
			ExpressionContext context = new ExpressionContext();
			context.Imports.AddType(typeof(string), "String");
			context.Imports.AddType(typeof(Math), "Math");

			context.ParserOptions.DecimalSeparator = ",";
			context.ParserOptions.RecreateParser();
			IGenericExpression<double> e = this.CreateGenericExpression<double>("1,25 + 0,75", context);
			Assert.AreEqual(1.25 + 0.75, e.Evaluate());

			context.ParserOptions.FunctionArgumentSeparator = ";";
			context.ParserOptions.RecreateParser();
			e = this.CreateGenericExpression<double>("math.pow(1,25 + 0,75; 2)", context);
			Assert.AreEqual(Math.Pow(1.25 + 0.75, 2), e.Evaluate());

			e = this.CreateGenericExpression<double>("math.max(,25;,75)", context);
			Assert.AreEqual(Math.Max(0.25, 0.75), e.Evaluate());

			context.ParserOptions.FunctionArgumentSeparator = ",";
			context.ParserOptions.DecimalSeparator = ",";
			context.ParserOptions.RequireDigitsBeforeDecimalPoint = true;
			context.ParserOptions.RecreateParser();
			e = this.CreateGenericExpression<double>("math.max(1,25,0,75)", context);
			Assert.AreEqual(Math.Max(1.25, 0.75), e.Evaluate());

			context.ParserOptions.FunctionArgumentSeparator = ";";
			context.ParserOptions.RecreateParser();
			IGenericExpression<string> e2 = this.CreateGenericExpression<string>("string.concat(1;2;3;4)", context);
			Assert.AreEqual("1234", e2.Evaluate());

			// Ambiguous grammar
			context.ParserOptions.FunctionArgumentSeparator = ",";
			context.ParserOptions.DecimalSeparator = ",";
			context.ParserOptions.RequireDigitsBeforeDecimalPoint = false;
			context.ParserOptions.RecreateParser();
			this.AssertCompileException("math.max(1,24,4,56)", context, CompileExceptionReason.SyntaxError);
		}

		[Test(Description = "Test getting the variables used in an expression")]
		public void TestGetReferencedVariables()
		{
			ExpressionContext context = new ExpressionContext(MyValidExpressionsOwner);
			context.Imports.AddType(typeof(Math));
			context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.NonPublic;

			context.Variables.Add("s1", "string");

			IDynamicExpression e = this.CreateDynamicExpression("s1.length + stringa.length", context);
			string[] referencedVariables = e.Info.GetReferencedVariables();

			Assert.AreEqual(2, referencedVariables.Length);
			Assert.AreNotEqual(-1, System.Array.IndexOf<string>(referencedVariables, "s1"));
			Assert.AreNotEqual(-1, System.Array.IndexOf<string>(referencedVariables, "stringa"));
		}

		[Test(Description = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
		public void TestLongBranchLogical1()
		{
			string expressionText = this.GetIndividualTest("LongBranch1");
			ExpressionContext context = new ExpressionContext();

			VariableCollection vc = context.Variables;

			vc.Add("M0100_ASSMT_REASON", "0");
			vc.Add("M0220_PRIOR_NOCHG_14D", "1");
			vc.Add("M0220_PRIOR_UNKNOWN", "1");
			vc.Add("M0220_PRIOR_UR_INCON", "1");
			vc.Add("M0220_PRIOR_CATH", "1");
			vc.Add("M0220_PRIOR_INTRACT_PAIN", "1");
			vc.Add("M0220_PRIOR_IMPR_DECSN", "1");
			vc.Add("M0220_PRIOR_DISRUPTIVE", "1");
			vc.Add("M0220_PRIOR_MEM_LOSS", "1");
			vc.Add("M0220_PRIOR_NONE", "1");

			vc.Add("M0220_PRIOR_UR_INCON_bool", true);
			vc.Add("M0220_PRIOR_CATH_bool", true);
			vc.Add("M0220_PRIOR_INTRACT_PAIN_bool", true);
			vc.Add("M0220_PRIOR_IMPR_DECSN_bool", true);
			vc.Add("M0220_PRIOR_DISRUPTIVE_bool", true);
			vc.Add("M0220_PRIOR_MEM_LOSS_bool", true);
			vc.Add("M0220_PRIOR_NONE_bool", true);
			vc.Add("M0220_PRIOR_NOCHG_14D_bool", true);
			vc.Add("M0220_PRIOR_UNKNOWN_bool", true);

			IDynamicExpression e = this.CreateDynamicExpression(expressionText, context);
			// We only care that the expression is valid and can be evaluated
			object result = e.Evaluate();
		}

		[Test(Description = "Test that we can handle long logical expressions and that we properly adjust for long branches")]
		public void TestLongBranchLogical2()
		{
			string expressionText = this.GetIndividualTest("LongBranch2");
			ExpressionContext context = new ExpressionContext();

			IGenericExpression<bool> e = this.CreateGenericExpression<bool>(expressionText, context);
			Assert.IsFalse(e.Evaluate());
		}



		[Test(Description = "Test that we can work with base and derived owner classes")]
		public void TestDerivedOwner()
		{
			System.Reflection.MethodBase mb = System.Reflection.MethodBase.GetCurrentMethod();
			System.Reflection.MethodInfo mi = mb as MethodInfo;

			ExpressionContext context = new ExpressionContext(mi);

			// Call a property on the base class
			IGenericExpression<bool> e = this.CreateGenericExpression<bool>("IsPublic", context);

			Assert.AreEqual(mi.IsPublic, e.Evaluate());

			context = new ExpressionContext(mb);
			// Test that setting the owner to a derived class works
			e = this.CreateGenericExpression<bool>("IsPublic", context);
			Assert.AreEqual(mb.IsPublic, e.Evaluate());

			// Pick a non-public method and set it as the new owner
			mi = typeof(Math).GetMethod("InternalTruncate", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
			e.Owner = mi;

			Assert.AreEqual(mi.IsPublic, e.Evaluate());
		}

		[Test(Description = "Test we can handle an expression owner that is a value type")]
		public void TestValueTypeOwner()
		{
			TestStruct owner = new TestStruct(100);
			ExpressionContext context = this.CreateGenericContext(owner);

			IGenericExpression<int> e = this.CreateGenericExpression<int>("myA", context);
			int result = e.Evaluate();
			Assert.AreEqual(100, result);

			e = this.CreateGenericExpression<int>("mya.compareto(100)", context);
			result = e.Evaluate();
			Assert.AreEqual(0, result);

			e = this.CreateGenericExpression<int>("DoStuff()", context);
			result = e.Evaluate();
			Assert.AreEqual(100, result);

			DateTime dt = DateTime.Now;
			context = this.CreateGenericContext(dt);

			e = this.CreateGenericExpression<int>("Month", context);
			result = e.Evaluate();
			Assert.AreEqual(dt.Month, result);

			IGenericExpression<string> e2 = this.CreateGenericExpression<string>("tolongdatestring()", context);
			Assert.AreEqual(dt.ToLongDateString(), e2.Evaluate());
		}

		[Test(Description = "We should be able to import non-public types if they are in the same module as our owner")]
		public void TestNonPublicImports()
		{
			ExpressionContext context = new ExpressionContext();

			try {
				// Should not be able to import non-public type
				// make sure type is not public
				Assert.IsFalse(typeof(TestImport).IsPublic);
				context.Imports.AddType(typeof(TestImport));
				Assert.Fail();

			} catch (ArgumentException) {
			}

			// ...until we set an owner that is in the same module
			context = new ExpressionContext(new OverloadTestExpressionOwner());
			context.Imports.AddType(typeof(TestImport));

			IDynamicExpression e = this.CreateDynamicExpression("DoStuff()", context);
			Assert.AreEqual(100, (int)e.Evaluate());

			// Try the same test with an invidual method
			context = new ExpressionContext();

			try {
				context.Imports.AddMethod("Dostuff", typeof(TestImport), "");
				Assert.Fail();

			} catch (ArgumentException) {
			}

			context = new ExpressionContext(new OverloadTestExpressionOwner());
			context.Imports.AddMethod("Dostuff", typeof(TestImport), "");
			e = this.CreateDynamicExpression("DoStuff()", context);
			Assert.AreEqual(100, (int)e.Evaluate());
		}

		[Test(Description = "Test import with nested types")]
		public void TestNestedTypeImport()
		{
			ExpressionContext context = new ExpressionContext();

			try {
				// Should not be able to import non-public nested type
				context.Imports.AddType(typeof(NestedA.NestedInternalB));
				Assert.Fail();

			} catch (ArgumentException) {
			}

			// Should be able to import public nested type
			context.Imports.AddType(typeof(NestedA.NestedPublicB));
			IDynamicExpression e = this.CreateDynamicExpression("DoStuff()", context);
			Assert.AreEqual(100, (int)e.Evaluate());

			// Should be able to import nested internal type now
			context = new ExpressionContext(new OverloadTestExpressionOwner());
			context.Imports.AddType(typeof(NestedA.NestedInternalB));
			e = this.CreateDynamicExpression("DoStuff()", context);
			Assert.AreEqual(100, (int)e.Evaluate());
		}

		[Test(Description = "We should not allow access to the non-public members of a variable"), ExpectedException(typeof(ExpressionCompileException))]
		public void TestNonPublicVariableMemberAccess()
		{
			ExpressionContext context = new ExpressionContext();
			context.Variables.Add("a", "abc");

			this.CreateDynamicExpression("a.m_length", context);
		}

		[Test(Description = "We should not compile an expression that accesses a non-public field with the same name as a variable")]
		public void TestFieldWithSameNameAsVariable()
		{
			ExpressionContext context = new ExpressionContext(new Monitor());
			context.Variables.Add("doubleA", new ExpressionOwner());
			this.AssertCompileException("doubleA.doubleA", context);

			// But it should work for a public member
			context = new ExpressionContext();
			Monitor m = new Monitor();
			context.Variables.Add("I", m);

			IDynamicExpression e = this.CreateDynamicExpression("i.i", context);
			Assert.AreEqual(m.I, (int)e.Evaluate());
		}

        //[Test(Description = "Test we can match members with names that differ only by case")]
        //public void TestMemberCaseSensitivity()
        //{
        //    FleeTest.CaseSensitiveOwner owner = new FleeTest.CaseSensitiveOwner();
        //    ExpressionContext context = new ExpressionContext(owner);
        //    context.Options.CaseSensitive = true;
        //    context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

        //    context.Variables.Add("x", 300);
        //    context.Variables.Add("X", 400);

        //    IDynamicExpression e = this.CreateDynamicExpression("a + A + x + X", context);
        //    Assert.AreEqual(100 + 200 + 300 + 400, (int)e.Evaluate());

        //    // Should fail since the function is called Cos
        //    this.AssertCompileException("cos(1)", context, CompileExceptionReason.UndefinedName);
        //}

		[Test(Description = "Test handling of on-demand variables")]
		public void TestOnDemandVariables()
		{
			ExpressionContext context = new ExpressionContext();
			context.Variables.ResolveVariableType += OnResolveVariableType;
			context.Variables.ResolveVariableValue += OnResolveVariableValue;

			IDynamicExpression e = this.CreateDynamicExpression("a + b", context);
			Assert.AreEqual(100 + 100, (int)e.Evaluate());
		}

		private void OnResolveVariableType(object sender, ResolveVariableTypeEventArgs e)
		{
			e.VariableType = typeof(int);
		}

		private void OnResolveVariableValue(object sender, ResolveVariableValueEventArgs e)
		{
			e.VariableValue = 100;
		}

		[Test(Description = "Test on-demand functions")]
		public void TestOnDemandFunctions()
		{
			ExpressionContext context = new ExpressionContext();
			context.Variables.ResolveFunction += OnResolveFunction;
			context.Variables.InvokeFunction += OnInvokeFunction;

			IDynamicExpression e = this.CreateDynamicExpression("func1(100) * func2(0.25)", context);
			Assert.AreEqual(100 * 0.25, (double)e.Evaluate());
		}

		private void OnResolveFunction(object sender, ResolveFunctionEventArgs e)
		{
			switch (e.FunctionName) {
				case "func1":
					e.ReturnType = typeof(int);
					break;
				case "func2":
					e.ReturnType = typeof(double);
					break;
			}
		}

		private void OnInvokeFunction(object sender, InvokeFunctionEventArgs e)
		{
			e.Result = e.Arguments[0];
		}

		[Test(Description = "Test that we properly resolve method overloads")]
		public void TestOverloadResolution()
		{
			OverloadTestExpressionOwner owner = new OverloadTestExpressionOwner();
			ExpressionContext context = new ExpressionContext(owner);

			// Test value types
			this.DoTestOverloadResolution("valuetype1(100)", context, 1);
			this.DoTestOverloadResolution("valuetype1(100.0f)", context, 2);
			this.DoTestOverloadResolution("valuetype1(100.0)", context, 3);
			this.DoTestOverloadResolution("valuetype2(100)", context, 1);

			// Test value type -> reference type
			this.DoTestOverloadResolution("Value_ReferenceType1(100)", context, 1);
			this.DoTestOverloadResolution("Value_ReferenceType2(100)", context, 1);
			//	with interfaces
			this.DoTestOverloadResolution("Value_ReferenceType3(100)", context, 1);

			// Test reference types
			this.DoTestOverloadResolution("ReferenceType1(\"abc\")", context, 2);
			this.DoTestOverloadResolution("ReferenceType1(b)", context, 1);
			this.DoTestOverloadResolution("ReferenceType2(a)", context, 2);
			//	with interfaces
			this.DoTestOverloadResolution("ReferenceType3(\"abc\")", context, 2);

			// Test nulls
			this.DoTestOverloadResolution("ReferenceType1(null)", context, 2);
			this.DoTestOverloadResolution("ReferenceType2(null)", context, 2);

			// Test ambiguous match
			this.DoTestOverloadResolution("valuetype3(100)", context, -1);
			this.DoTestOverloadResolution("Value_ReferenceType4(100)", context, -1);
			this.DoTestOverloadResolution("ReferenceType4(\"abc\")", context, -1);
			this.DoTestOverloadResolution("ReferenceType4(null)", context, -1);

			// Test access control
			this.DoTestOverloadResolution("Access1(\"abc\")", context, 1);
			this.DoTestOverloadResolution("Access2(\"abc\")", context, -1);
			this.DoTestOverloadResolution("Access2(null)", context, -1);

			// Test with multiple arguments
			this.DoTestOverloadResolution("Multiple1(1.0f, 2.0)", context, 1);
			this.DoTestOverloadResolution("Multiple1(100, 2.0)", context, 2);
			this.DoTestOverloadResolution("Multiple1(100, 2.0f)", context, 2);
		}

		private void DoTestOverloadResolution(string expression, ExpressionContext context, int expectedResult)
		{
			try {
				IGenericExpression<int> e = this.CreateGenericExpression<int>(expression, context);
				int result = e.Evaluate();
				Assert.AreEqual(expectedResult, result);
			} catch (Exception) {
				Assert.AreEqual(-1, expectedResult);
			}
		}

		[Test(Description = "Test the NumbersAsDoubles option")]
		public void TestNumbersAsDoubles()
		{
			ExpressionContext context = new ExpressionContext();
			context.Options.IntegersAsDoubles = true;

			IGenericExpression<double> e = this.CreateGenericExpression<double>("1 / 2", context);
			Assert.AreEqual(1 / 2.0, e.Evaluate());

			e = this.CreateGenericExpression<double>("4 * 4 / 10", context);
			Assert.AreEqual(4 * 4 / 10.0, e.Evaluate());

			context.Variables.Add("a", 1);

			e = this.CreateGenericExpression<double>("a / 10", context);
			Assert.AreEqual(1 / 10.0, e.Evaluate());

			// Should get a double back
			IDynamicExpression e2 = this.CreateDynamicExpression("100", context);
			Assert.IsInstanceOf(typeof(double),e2.Evaluate());
		}

		[Test(Description = "Test variables that are expressions")]
		public void TestExpressionVariables()
		{
			ExpressionContext context1 = new ExpressionContext();
			context1.Imports.AddType(typeof(Math));
			context1.Variables.Add("a", Math.PI);
			IDynamicExpression exp1 = this.CreateDynamicExpression("sin(a)", context1);

			ExpressionContext context2 = new ExpressionContext();
			context2.Imports.AddType(typeof(Math));
			context2.Variables.Add("a", Math.PI);
			IGenericExpression<double> exp2 = this.CreateGenericExpression<double>("cos(a/2)", context2);

			ExpressionContext context3 = new ExpressionContext();
			context3.Variables.Add("a", exp1);
			context3.Variables.Add("b", exp2);
			IDynamicExpression exp3 = this.CreateDynamicExpression("a + b", context3);

			double a = Math.Sin(Math.PI);
			double b = Math.Cos(Math.PI / 2);

			Assert.AreEqual(a + b, exp3.Evaluate());

			ExpressionContext context4 = new ExpressionContext();
			context4.Variables.Add("a", exp1);
			context4.Variables.Add("b", exp2);
			IGenericExpression<double> exp4 = this.CreateGenericExpression<double>("(a * b) + (b - a)", context4);

			Assert.AreEqual((a * b) + (b - a), exp4.Evaluate());
		}

		[Test(Description = "Test that no state is held in the original context between compiles")]
		public void TestNoStateHeldInContext()
		{
			ExpressionContext context = new ExpressionContext();

			IGenericExpression<int> e1 = context.CompileGeneric<int>("300");

			// The result type of the cloned context should be set to integer
			Assert.IsTrue(object.ReferenceEquals(e1.Context.Options.ResultType, typeof(Int32)));

			// The original context should not be affected
			Assert.IsNull(context.Options.ResultType);

			// This should compile
			IDynamicExpression e2 = context.CompileDynamic("\"abc\"");
			Assert.IsTrue(object.ReferenceEquals(e2.Context.Options.ResultType, typeof(string)));

			// The original context should not be affected
			Assert.IsNull(context.Options.ResultType);
		}

		[Test(Description = "Test cloning an expression")]
		public void TestExpressionClone()
		{
			ExpressionContext context = new ExpressionContext();
			context.Variables.Add("a", 100);
			context.Variables.Add("b", 200);
			IGenericExpression<int> exp1 = this.CreateGenericExpression<int>("(a * b)", context);

			IGenericExpression<int> exp2 = exp1.Clone() as IGenericExpression<int>;

			Assert.AreNotSame(exp1.Context.Variables, exp2.Context.Variables);

			exp2.Context.Variables["a"] = 10;
			exp2.Context.Variables["b"] = 20;

			Assert.AreEqual(10 * 20, exp2.Evaluate());

			Thread t1 = new Thread(ThreadRunClone);
			Thread t2 = new Thread(ThreadRunClone);
			t1.Start(exp1);
			t2.Start(exp2);

			IDynamicExpression exp3 = this.CreateDynamicExpression("a * b", context);
			IDynamicExpression exp4 = exp3.Clone() as IDynamicExpression;

			Assert.AreEqual(100 * 200, exp4.Evaluate());
		}

		private void ThreadRunClone(object o)
		{
			IGenericExpression<int> exp = o as IGenericExpression<int>;

			int a = (int)exp.Context.Variables["a"];
			int b = (int)exp.Context.Variables["b"];

			for (int i = 0; i <= 10000 - 1; i++) {
				Assert.AreEqual(a * b, exp.Evaluate());
			}
		}

		[Test(Description = "Test the RealLiteralDataType option")]
		public void TestRealLiteralDataTypeOption()
		{
			ExpressionContext context = new ExpressionContext();
			context.Options.RealLiteralDataType = RealLiteralDataType.Single;

			IDynamicExpression e = this.CreateDynamicExpression("100.25", context);

			Assert.IsInstanceOf(typeof(float), e.Evaluate());

			context.Options.RealLiteralDataType = RealLiteralDataType.Decimal;
			e = this.CreateDynamicExpression("100.25", context);

			Assert.IsInstanceOf(typeof(decimal), e.Evaluate());

			context.Options.RealLiteralDataType = RealLiteralDataType.Double;
			e = this.CreateDynamicExpression("100.25", context);

			Assert.IsInstanceOf(typeof(double), e.Evaluate());

			// Override should still work though
			e = this.CreateDynamicExpression("100.25f", context);
			Assert.IsInstanceOf(typeof(float), e.Evaluate());

			e = this.CreateDynamicExpression("100.25d", context);
			Assert.IsInstanceOf(typeof(double), e.Evaluate());

			e = this.CreateDynamicExpression("100.25M", context);
			Assert.IsInstanceOf(typeof(decimal), e.Evaluate());
		}
	}
}
