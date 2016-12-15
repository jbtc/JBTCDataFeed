
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class QueryExpressionEvaluator
    {
        /// <summary>
        /// Does the evaluation work.
        /// </summary>
        public void doEvaluationWork()
        {
            
            Ciloci.Flee.ExpressionContext context = new Ciloci.Flee.ExpressionContext();
            context.Imports.AddType(typeof(CustomFunctions));
            context.Variables.Add("a", 100);
            context.Variables.Add("b", 200);

            Ciloci.Flee.IDynamicExpression e = context.CompileDynamic("product(a,b) + sum(a,b)");
            int result = (int)e.Evaluate();
        }

    }
    public static class CustomFunctions
    {
        /// <summary>
        /// Products of a and b
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static int Product(int a, int b)
        {
            return a * b;
        }

        /// <summary>
        /// Sum of a and b
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static int Sum(int a, int b)
        {
            return a + b;
        }

        /// <summary>
        /// Gets the int tag data.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public static int getIntTagData(string tagName)
        {
            return 1;
        }

        /// <summary>
        /// Gets the string tag data.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <returns></returns>
        public static string getStringTagData(string tagName)
        {
            return "ok";
        }

        /// <summary>
        /// Gets the float tag data.
        /// </summary>
        /// <param name="tagname">The tagname.</param>
        /// <returns></returns>
        public static decimal getFloatTagData(string tagname)
        {
            return Convert.ToDecimal("1.2");
        }
    }
}
