using System;
using System.Diagnostics;
using NUnit.Framework;
using Ciloci.Flee.CalcEngine;

namespace Ciloci.Flee.Tests
{

	// Tests and methods to evaluate speed

	[TestFixture()]
	public class Benchmarks : ExpressionTests
	{

		[Test(Description = "Test that setting variables is fast")]
		public void TestFastVariables()
		{
			// Test should take 200ms or less
			const int EXPECTED_TIME = 200;
			const int ITERATIONS = 100000;

			ExpressionContext context = new ExpressionContext();
			VariableCollection vars = context.Variables;
			vars.DefineVariable("a", typeof(Int32));
			vars.DefineVariable("b", typeof(Int32));
			IDynamicExpression e = this.CreateDynamicExpression("a + b * (a ^ 2)", context);

			Stopwatch sw = new Stopwatch();
			sw.Start();

			for (int i = 0; i <= ITERATIONS - 1; i++) {
				object result = e.Evaluate();
				vars["a"] = 200;
				vars["b"] = 300;
			}

			sw.Stop();

			this.PrintSpeedMessage("Fast variables", ITERATIONS, sw);
			NUnit.Framework.Assert.Less((decimal)sw.ElapsedMilliseconds, EXPECTED_TIME, "Test time above expected value");
		}

        [Test(Description = "Test the speed of the simple calc engine")]
		public void TestSimpleCalcEngine()
		{
			const int ITERATIONS = 2000;

			SimpleCalcEngine engine = new SimpleCalcEngine();
			engine.Context.Imports.AddType(typeof(Math));

			Stopwatch sw = new Stopwatch();

			// Test speed of populating the engine
			string prev = "1";
			string cur = null;

			sw.Start();

			// Create a chain of expressions each referring to the previous one
			for (int i = 0; i <= ITERATIONS - 1; i++) {
				cur = string.Format("i_{0}", Guid.NewGuid().ToString("N"));
				string expression = string.Format("{0} + 1 + cos(3.14)", prev);

				engine.AddGeneric<double>(cur, expression);

				prev = cur;
			}

			sw.Stop();

			this.PrintSpeedMessage("Simple calc engine (population)", ITERATIONS, sw);

			// Evaluate the last expression (which will evaluate all the previous ones up the chain)
            var e = engine[cur] as IGenericExpression<double>;

			sw.Reset();
			sw.Start();

			double result = e.Evaluate();
			sw.Stop();

			this.PrintSpeedMessage("Simple calc engine (evaluation)", ITERATIONS, sw);
		}

		[Test(Description = "Test how fast we parse/compile an expression")]
		public void TestParseCompileSpeed()
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

			Stopwatch sw = new Stopwatch();

			// Do one compile to eliminate the cold start effect
			IDynamicExpression e = this.CreateDynamicExpression(expressionText, context);

			sw.Start();
			e = this.CreateDynamicExpression(expressionText, context);
			e = this.CreateDynamicExpression(this.GetIndividualTest("LongBranch2"));
			e = this.CreateDynamicExpression("if(1 > 0, 1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0+1.0, 20.0)");
			sw.Stop();

			const int EXPECTED_TIME = 20;

            float timePerExpression = (float)(sw.ElapsedMilliseconds / 3.0);

			Assert.Less(timePerExpression, EXPECTED_TIME);

			this.WriteMessage("Parse/Compile speed = {0:n0}ms", timePerExpression);
		}

		private void PrintSpeedMessage(string title, int iterations, Stopwatch sw)
		{
			this.WriteMessage("{0}: {1:n0} iterations in {2:n2}ms = {3:n2} iterations/sec", title, iterations, sw.ElapsedMilliseconds, iterations / (sw.ElapsedMilliseconds / 1000.0));
		}
	}
}
