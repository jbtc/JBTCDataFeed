
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
        /// Does the evaluating work.
        /// </summary>
        public void doEvaluationWorkSample()
        {
            
            Ciloci.Flee.ExpressionContext context = new Ciloci.Flee.ExpressionContext();
            context.Imports.AddType(typeof(CustomFunctions));
            //context.Variables.Add("a", 100);
            context.Variables.Add("b", 23);

            Ciloci.Flee.IDynamicExpression e = context.CompileDynamic("product("+"1"+",b) + sum("+"17"+",b)");
            int result = (int)e.Evaluate();
        }

    }

    /// <summary>
    /// extension function to enhance the expressions with custom functions
    /// </summary>
    public static class CustomFunctions
    {
        #region basic math
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
        #endregion

        #region get tag data by type
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
        #endregion

        #region basic logic and and or 
        /// <summary>
        /// Logical calculation : A and B
        /// </summary>
        /// <param name="a">if set to <c>true</c> [a].</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <returns></returns>
        public static bool logicAND(bool a, bool b)
        {
            return a & b;
        }

        /// <summary>
        /// Logical calculation : A or B
        /// </summary>
        /// <param name="a">if set to <c>true</c> [a].</param>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <returns></returns>
        public static bool logicOR(bool a, bool b)
        {
            return a | b;
        }
        #endregion

        #region equality
        /// <summary>
        /// compare two ints for equality
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool logicEQUALINT(int a, int b)
        {
            return (a==b?true:false);
        }

        /// <summary>
        /// compare two decimal values for equality
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool logicEQUALDecimal(decimal a, decimal b)
        {
            return (a == b ? true : false);
        }

        /// <summary>
        /// compare two strings for equality.
        /// </summary>
        /// <param name="a">a.</param>
        /// <param name="b">The b.</param>
        /// <returns></returns>
        public static bool logicEQUALSTRING(string a, string b)
        {
            return (a == b ? true : false);
        }
        #endregion
    }
}
