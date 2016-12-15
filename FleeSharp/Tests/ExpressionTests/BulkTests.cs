
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using Ciloci.Flee;
namespace Ciloci.Flee.Tests
{

	[TestFixture()]
	public class BulkTests : ExpressionTests
	{

		[Test(Description = "Expressions that should be valid")]
		public void TestValidExpressions()
		{
			MyCurrentContext = MyGenericContext;

			MyCurrentContext.Variables.ResolveFunction += TestValidExpressions_OnResolveFunction;
			MyCurrentContext.Variables.InvokeFunction += TestValidExpressions_OnInvokeFunction;

			this.ProcessScriptTests("ValidExpressions.txt", DoTestValidExpressions);

			MyCurrentContext.Variables.ResolveFunction -= TestValidExpressions_OnResolveFunction;
			MyCurrentContext.Variables.InvokeFunction -= TestValidExpressions_OnInvokeFunction;
		}

		private void TestValidExpressions_OnResolveFunction(object sender, ResolveFunctionEventArgs e)
		{
			e.ReturnType = typeof(int);
		}

		private void TestValidExpressions_OnInvokeFunction(object sender, InvokeFunctionEventArgs e)
		{
			e.Result = 100;
		}

		[Test(Description = "Expressions that should not be valid")]
		public void TestInvalidExpressions()
		{
			this.ProcessScriptTests("InvalidExpressions.txt", DoTestInvalidExpressions);
		}

		[Test(Description = "Casts that should be valid")]
		public void TestValidCasts()
		{
			MyCurrentContext = MyValidCastsContext;
			this.ProcessScriptTests("ValidCasts.txt", DoTestValidExpressions);
		}

		[Test(Description = "Test our handling of checked expressions")]
		public void TestCheckedExpressions()
		{
			this.ProcessScriptTests("CheckedTests.txt", DoTestCheckedExpressions);
		}

		private void DoTestValidExpressions(string[] arr)
		{
			string typeName = string.Concat("System.", arr[0]);
			Type expressionType = Type.GetType(typeName, true, true);

			ExpressionContext context = MyCurrentContext;
			context.Options.ResultType = expressionType;

			IDynamicExpression e = this.CreateDynamicExpression(arr[1], context);
			this.DoTest(e, arr[2], expressionType, ExpressionTests.TestCulture);
		}

		private void DoTestInvalidExpressions(string[] arr)
		{
			Type expressionType = Type.GetType(arr[0], true, true);
			CompileExceptionReason reason = (CompileExceptionReason)System.Enum.Parse(typeof(CompileExceptionReason), arr[2], true);

			ExpressionContext context = MyGenericContext;
			ExpressionOptions options = context.Options;
			options.ResultType = expressionType;
			context.Imports.AddType(typeof(Math));
			options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;

			this.AssertCompileException(arr[1], context);
		}

		private void DoTestCheckedExpressions(string[] arr)
		{
			string expression = arr[0];
			bool @checked = bool.Parse(arr[1]);
			bool shouldOverflow = bool.Parse(arr[2]);

			ExpressionContext context = new ExpressionContext(MyValidExpressionsOwner);
			ExpressionOptions options = context.Options;
			context.Imports.AddType(typeof(Math));
			context.Imports.ImportBuiltinTypes();
			options.Checked = @checked;

			try {
				IDynamicExpression e = this.CreateDynamicExpression(expression, context);
				e.Evaluate();
				Assert.IsFalse(shouldOverflow);
			} catch (OverflowException) {
				Assert.IsTrue(shouldOverflow);
			}
		}
	}
}
