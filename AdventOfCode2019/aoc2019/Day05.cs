using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    [TestClass]
    public class Day05
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void ExamplePart1()
        {
            Assert.AreEqual(1, compute("string"));
        }

        private int compute(string v)
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public void Part1()
        {
            foreach (string line in Common.GetLines(input))
            {
                TestContext.WriteLine(line);
            }
        }

        [TestMethod]
        public void Part2()
        {
        }

        private static string input = @"94735
80130
127915
";
    }
}
