using System;
using Ciloci.Flee;

namespace ConsoleTester
{

	static class Program
	{

		public static void Main()
		{
			ExpressionContext context = new ExpressionContext();
			context.Options.EmitToAssembly = true;

			//context.ParserOptions.RequireDigitsBeforeDecimalPoint = True
			//context.ParserOptions.DecimalSeparator = ","c
			//context.ParserOptions.RecreateParser()
			//context.Options.ResultType = GetType(Decimal)

            context.Variables["a"] = 2;
            context.Variables["b"] = 4;

			IDynamicExpression e1 = context.CompileDynamic("If (19 in (13,28,33,48,71,73,101,102,103,104,23,23,234,34,345,345,45,34,34,4555,445,20),1,0)");
            var res1 = e1.Evaluate();

            var e = context.CompileGeneric<bool>("b > a");
			object result = e.Evaluate();

            Console.ReadLine();
		}

	}
}
