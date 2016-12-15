
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using NUnit.Framework;
using Ciloci.Flee;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Xml.XPath;
namespace Ciloci.Flee.Tests
{

	public abstract class ExpressionTests
	{

		private const string COMMENT_CHAR = "\'";
        private const char SEPARATOR_CHAR = ';';

		protected delegate void LineProcessor(string[] lineParts);

		protected ExpressionOwner MyValidExpressionsOwner = new ExpressionOwner();
		protected ExpressionContext MyGenericContext;
		protected ExpressionContext MyValidCastsContext;

		protected ExpressionContext MyCurrentContext;

		protected static readonly CultureInfo TestCulture = CultureInfo.GetCultureInfo("en-CA");

		#region "Initialization"
		public ExpressionTests()
		{
			MyValidExpressionsOwner = new ExpressionOwner();

			MyGenericContext = this.CreateGenericContext(MyValidExpressionsOwner);

			ExpressionContext context = new ExpressionContext(MyValidExpressionsOwner);
			context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
			context.Imports.ImportBuiltinTypes();
			context.Imports.AddType(typeof(Convert), "Convert");
			context.Imports.AddType(typeof(Guid));
			context.Imports.AddType(typeof(Version));
			context.Imports.AddType(typeof(DayOfWeek));
			context.Imports.AddType(typeof(DayOfWeek), "DayOfWeek");
			context.Imports.AddType(typeof(ValueType));
			context.Imports.AddType(typeof(IComparable));
			context.Imports.AddType(typeof(ICloneable));
			context.Imports.AddType(typeof(Array));
			context.Imports.AddType(typeof(System.Delegate));
			context.Imports.AddType(typeof(AppDomainInitializer));
			context.Imports.AddType(typeof(System.Text.Encoding));
			context.Imports.AddType(typeof(System.Text.ASCIIEncoding));
			context.Imports.AddType(typeof(ArgumentException));

			MyValidCastsContext = context;

			// For testing virtual properties
			TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(int))), typeof(int));
			TypeDescriptor.AddProvider(new UselessTypeDescriptionProvider(TypeDescriptor.GetProvider(typeof(string))), typeof(string));

			this.Initialize();
		}


		protected virtual void Initialize()
		{
            MyGenericContext.Options.ParseCulture = TestCulture;
		}

		protected ExpressionContext CreateGenericContext(object owner)
		{
			ExpressionContext context = null;

			if (owner == null) {
				context = new ExpressionContext();
			} else {
				context = new ExpressionContext(owner);
			}

			context.Options.OwnerMemberAccess = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic;
			context.Imports.ImportBuiltinTypes();
			context.Imports.AddType(typeof(Math), "Math");
			context.Imports.AddType(typeof(Uri), "Uri");
			context.Imports.AddType(typeof(Mouse), "Mouse");
			context.Imports.AddType(typeof(Monitor), "Monitor");
			context.Imports.AddType(typeof(DateTime), "DateTime");
			context.Imports.AddType(typeof(Convert), "Convert");
			context.Imports.AddType(typeof(Type), "Type");
			context.Imports.AddType(typeof(DayOfWeek), "DayOfWeek");
			context.Imports.AddType(typeof(ConsoleModifiers), "ConsoleModifiers");

			NamespaceImport ns1 = new NamespaceImport("ns1");
			NamespaceImport ns2 = new NamespaceImport("ns2");
			ns2.Add(new TypeImport(typeof(Math)));

			ns1.Add(ns2);

			context.Imports.RootImport.Add(ns1);

			context.Variables.Add("varInt32", 100);
			context.Variables.Add("varDecimal", new decimal(100));
			context.Variables.Add("varString", "string");

			return context;
		}
		#endregion

		#region "Test framework"

		protected IGenericExpression<T> CreateGenericExpression<T>(string expression)
		{
			return this.CreateGenericExpression<T>(expression, new ExpressionContext());
		}

		protected IGenericExpression<T> CreateGenericExpression<T>(string expression, ExpressionContext context)
		{
			IGenericExpression<T> e = context.CompileGeneric<T>(expression);
			return e;
		}

		protected IDynamicExpression CreateDynamicExpression(string expression)
		{
			return this.CreateDynamicExpression(expression, new ExpressionContext());
		}

		protected IDynamicExpression CreateDynamicExpression(string expression, ExpressionContext context)
		{
			return context.CompileDynamic(expression);
		}

		protected void AssertCompileException(string expression)
		{
			try {
				this.CreateDynamicExpression(expression);
				Assert.Fail();

			} catch (ExpressionCompileException) {
			}
		}

		protected void AssertCompileException(string expression, ExpressionContext context, CompileExceptionReason expectedReason = CompileExceptionReason.Unknown)
		{
			try {
				this.CreateDynamicExpression(expression, context);
				Assert.Fail("Compile exception expected");
			} catch (ExpressionCompileException ex) {
				if (expectedReason !=  CompileExceptionReason.Unknown) {
					Assert.AreEqual(expectedReason, ex.Reason, string.Format("Expected reason '{0}' but got '{1}'", expectedReason, ex.Reason));
				}
			}
		}

		protected void DoTest(IDynamicExpression e, string result, Type resultType, CultureInfo testCulture)
		{
			if (object.ReferenceEquals(resultType, typeof(object))) {
				Type expectedType = Type.GetType(result, false, true);

				if (expectedType == null) {
					// Try to get the type from the Tests assembly
					result = string.Format("{0}.{1}", this.GetType().Namespace, result);
					expectedType = this.GetType().Assembly.GetType(result, true, true);
				}

				object expressionResult = e.Evaluate();

				if (object.ReferenceEquals(expectedType, typeof(void))) {
					Assert.IsNull(expressionResult);
				} else {
					Assert.IsInstanceOfType(expectedType, expressionResult);
				}

			} else {
				TypeConverter tc = TypeDescriptor.GetConverter(resultType);

				object expectedResult = tc.ConvertFromString(null, testCulture, result);
				object actualResult = e.Evaluate();

				expectedResult = RoundIfReal(expectedResult);
				actualResult = RoundIfReal(actualResult);

				Assert.AreEqual(expectedResult, actualResult);
			}
		}

		protected object RoundIfReal(object value)
		{
			if (object.ReferenceEquals(value.GetType(), typeof(double))) {
				double d = (double)value;
				d = Math.Round(d, 4);
				return d;
			} else if (object.ReferenceEquals(value.GetType(), typeof(float))) {
				float s = (float)value;
                s = (float)Math.Round(s, 4);
				return s;
			} else {
				return value;
			}
		}

		protected void ProcessScriptTests(string scriptFileName, LineProcessor processor)
		{
			this.WriteMessage("Testing: {0}", scriptFileName);

			System.IO.Stream instream = this.GetScriptFile(scriptFileName);
			System.IO.StreamReader sr = new System.IO.StreamReader(instream);

			try {
				this.ProcessLines(sr, processor);
			} finally {
				sr.Close();
			}
		}

		private void ProcessLines(System.IO.TextReader sr, LineProcessor processor)
		{
			while (sr.Peek() != -1) {
				string line = sr.ReadLine();
				this.ProcessLine(line, processor);
			}
		}

		private void ProcessLine(string line, LineProcessor processor)
		{
			if (line.StartsWith(COMMENT_CHAR) == true) {
				return;
			}

			try {
				var arr = line.Split(SEPARATOR_CHAR);
				processor(arr);
			} catch (Exception) {
				this.WriteMessage("Failed line: {0}", line);
				throw;
			}
		}

		protected System.IO.Stream GetScriptFile(string fileName)
		{
			Assembly a = Assembly.GetExecutingAssembly();
			return a.GetManifestResourceStream(this.GetType(), fileName);
		}

		protected string GetIndividualTest(string testName)
		{
			Assembly a = Assembly.GetExecutingAssembly();

			Stream s = a.GetManifestResourceStream(this.GetType(), "IndividualTests.xml");

			XPathDocument doc = new XPathDocument(s);

			XPathNavigator nav = doc.CreateNavigator();

			nav = nav.SelectSingleNode(string.Format("Tests/Test[@Name='{0}']", testName));

			string str = (string)nav.TypedValue;

			s.Close();

			return str;
		}

		protected void WriteMessage(string msg, params object[] args)
		{
			msg = string.Format(msg, args);
			Console.WriteLine(msg);
		}

		#endregion

		#region "Utility"

		protected static object Parse(string s)
		{
			bool b = false;

			if (bool.TryParse(s, out b) == true) {
				return b;
			}

			int i = 0;

			if (int.TryParse(s, NumberStyles.Integer, TestCulture, out i) == true) {
				return i;
			}

			double d = 0;

            if (double.TryParse(s, NumberStyles.Float, TestCulture, out d) == true)
            {
				return d;
			}

			DateTime dt = default(DateTime);

			if (DateTime.TryParse(s, TestCulture, DateTimeStyles.None, out dt) == true) {
				return dt;
			}

			return s;
		}

		protected static IDictionary<string, object> ParseQueryString(string s)
		{
			string[] arr = s.Split('&');
			Dictionary<string, object> dict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

			foreach (string part in arr) {
				string[] arr2 = part.Split('=');
				dict.Add(arr2[0], Parse(arr2[1]));
			}

			return dict;
		}
		#endregion

	}
}
