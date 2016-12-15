using System;
using System.Linq;

namespace Ciloci.Flee.Tests
{
    class Program
    {
        public static void Main()
        {
            var bulkTests = new BulkTests();

            bulkTests.TestInvalidExpressions();
            bulkTests.TestValidExpressions();

            var benches = new Benchmarks();
            benches.TestSimpleCalcEngine();

            Console.ReadLine();
        }
    }
}
