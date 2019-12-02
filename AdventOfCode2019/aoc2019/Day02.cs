using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    [TestClass]
    public class Day02
    {
        [TestMethod]
        public void ExamplePart1()
        {
            // Opcode 1 adds
            // Opcode 2 multiplies
            // Opcode 99 means that the program is finished and should immediately halt
            var example1 = "1,0,0,0,99";
            var arr = example1.Split(",").Select(x => int.Parse(x.Trim())).ToArray();
            CollectionAssert.AreEqual(new int[]{ 2, 0, 0, 0, 99 }, execute(new int[] { 1, 0, 0, 0, 99 }));
            CollectionAssert.AreEqual(new int[]{ 2, 3, 0, 6, 99 }, execute(new int[] { 2, 3, 0, 3, 99 }));
            CollectionAssert.AreEqual(new int[]{ 2, 4, 4, 5, 99, 9801 }, execute(new int[] { 2, 4, 4, 5, 99, 0 }));
            CollectionAssert.AreEqual(new int[]{ 30, 1, 1, 4, 2, 5, 6, 0, 99 }, execute(new int[] { 1, 1, 1, 4, 99, 5, 6, 0, 99 }));
        }

        private static int[] execute(int[] arr)
        {
            int index = 0;
            int opCode = arr[index];
            while (opCode != 99)
            {
                switch (opCode)
                {
                    case 1:
                        arr[arr[index + 3]] = arr[arr[index + 1]] + arr[arr[index + 2]];
                        break;
                    case 2:
                        arr[arr[index + 3]] = arr[arr[index + 1]] * arr[arr[index + 2]];
                        break;
                    case 99:
                        break;
                    default:
                        throw new Exception($"Something went wrong opCode={opCode}");
                }
                index += 4;
                opCode = arr[index];
            }
            return arr;
        }


        [TestMethod]
        public void Part1()
        {
            var arr = input.Split(",").Select(x => int.Parse(x.Trim())).ToArray();
            arr[1] = 12;
            arr[2] = 2;
            execute(arr);
            Console.WriteLine(arr[0]);
        }

        [TestMethod]
        public void Part2()
        {
            
        }




        private static string input = @"1,0,0,3,1,1,2,3,1,3,4,3,1,5,0,3,2,1,6,19,1,19,5,23,2,13,23,27,1,10,27,31,2,6,31,35,1,9,35,39,2,10,39,43,1,43,9,47,1,47,9,51,2,10,51,55,1,55,9,59,1,59,5,63,1,63,6,67,2,6,67,71,2,10,71,75,1,75,5,79,1,9,79,83,2,83,10,87,1,87,6,91,1,13,91,95,2,10,95,99,1,99,6,103,2,13,103,107,1,107,2,111,1,111,9,0,99,2,14,0,0";
    }
}
