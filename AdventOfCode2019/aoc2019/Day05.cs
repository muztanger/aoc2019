using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    [TestClass]
    public class Day05
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Part1()
        {
            var init = input.Split(",").Select(x => int.Parse(x.Trim())).ToArray();

            int instructionPointer = 0;
            var memory = new int[100000];
            for (int i = 0; i < init.Length; i++)
            {
                memory[i] = init[i];
            }

            /*
            Opcode 3 takes a single integer as input and saves it to the address given by its only parameter. For example, the instruction 3,50 would take an input value and store it at address 50.
            Opcode 4 outputs the value of its only parameter. For example, the instruction 4,50 would output the value at address 50.
            */

            int opCode = GetOpCode(memory[0]);
            bool[] modes = GetModes(memory[0]);
            TestContext.WriteLine($"INSTRUCTION: {opCode} {String.Join(",", modes)}");

            int count = 0;
            while (opCode != 99)
            {
                count++;
                switch (opCode)
                {
                    case 1: // add
                        {
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            memory[memory[instructionPointer + 3]] = a + b;
                        }
                        instructionPointer += 4;
                        break;
                    case 2: // multiply
                        {
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            memory[memory[instructionPointer + 3]] = a * b;
                        }
                        instructionPointer += 4;
                        break;
                    case 3: // read
                        memory[memory[instructionPointer + 1]] = 1; // from text
                        instructionPointer += 2;
                        break;
                    case 4: // write
                        if (modes.Count() > 0)
                        {
                            Console.WriteLine("OUTPUT: " + memory[instructionPointer + 1]);
                        }
                        else
                        {
                            Console.WriteLine("OUTPUT: " + memory[memory[instructionPointer + 1]]);
                        }
                        instructionPointer += 2;
                        break;
                    case 99: // stop
                        instructionPointer += 1;
                        break;
                    default:
                        throw new Exception($"Something went wrong opCode={opCode} count={count}");
                }
                opCode = GetOpCode(memory[instructionPointer]);
                modes = GetModes(memory[instructionPointer]);
                TestContext.WriteLine($"INSTRUCTION: {memory[instructionPointer]} => {opCode} {String.Join(",", modes)}");
            }
            
            //223incorrect
        }

        private static void GetValues(int instructionPointer, ref int[] memory, ref bool[] modes, out int a, out int b)
        {
            a = memory[instructionPointer + 1];
            if (modes.Count() < 1 || !modes[0])
            {
                a = memory[memory[instructionPointer + 1]];
            }
            b = memory[instructionPointer + 2];
            if (modes.Count() < 2 || !modes[1])
            {
                b = memory[memory[instructionPointer + 2]];
            }
        }

        private bool[] GetModes(int v)
        {
            // 0: position mode
            // 1: immediate mode
            v /= 100;
            List<bool> modes = new List<bool>();
            while (v > 0)
            {
                int x = v % 10;
                Assert.IsTrue(x == 0 || x == 1, "x=" + x);
                modes.Add(v % 10 == 1);
                v /= 10;
            }
            return modes.ToArray();
        }

        private static int GetOpCode(int x)
        {
            int opCode = x % 10;
            if (opCode == 9) opCode = 99;
            return opCode;
        }







private static string input = @"3,225,1,225,6,6,1100,1,238,225,104,0,1101,72,36,225,1101,87,26,225,2,144,13,224,101,-1872,224,224,4,224,102,8,223,223,1001,224,2,224,1,223,224,223,1102,66,61,225,1102,25,49,224,101,-1225,224,224,4,224,1002,223,8,223,1001,224,5,224,1,223,224,223,1101,35,77,224,101,-112,224,224,4,224,102,8,223,223,1001,224,2,224,1,223,224,223,1002,195,30,224,1001,224,-2550,224,4,224,1002,223,8,223,1001,224,1,224,1,224,223,223,1102,30,44,225,1102,24,21,225,1,170,117,224,101,-46,224,224,4,224,1002,223,8,223,101,5,224,224,1,224,223,223,1102,63,26,225,102,74,114,224,1001,224,-3256,224,4,224,102,8,223,223,1001,224,3,224,1,224,223,223,1101,58,22,225,101,13,17,224,101,-100,224,224,4,224,1002,223,8,223,101,6,224,224,1,224,223,223,1101,85,18,225,1001,44,7,224,101,-68,224,224,4,224,102,8,223,223,1001,224,5,224,1,223,224,223,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,7,677,226,224,102,2,223,223,1005,224,329,101,1,223,223,8,677,226,224,1002,223,2,223,1005,224,344,1001,223,1,223,1107,677,677,224,102,2,223,223,1005,224,359,1001,223,1,223,1107,226,677,224,102,2,223,223,1005,224,374,101,1,223,223,7,226,677,224,102,2,223,223,1005,224,389,101,1,223,223,8,226,677,224,1002,223,2,223,1005,224,404,101,1,223,223,1008,226,677,224,1002,223,2,223,1005,224,419,1001,223,1,223,107,677,677,224,102,2,223,223,1005,224,434,101,1,223,223,1108,677,226,224,1002,223,2,223,1006,224,449,101,1,223,223,1108,677,677,224,102,2,223,223,1006,224,464,101,1,223,223,1007,677,226,224,102,2,223,223,1006,224,479,101,1,223,223,1008,226,226,224,102,2,223,223,1006,224,494,101,1,223,223,108,226,226,224,1002,223,2,223,1006,224,509,101,1,223,223,107,226,226,224,102,2,223,223,1006,224,524,101,1,223,223,1107,677,226,224,102,2,223,223,1005,224,539,1001,223,1,223,108,226,677,224,1002,223,2,223,1005,224,554,101,1,223,223,1007,226,226,224,102,2,223,223,1005,224,569,101,1,223,223,8,226,226,224,102,2,223,223,1006,224,584,101,1,223,223,1008,677,677,224,1002,223,2,223,1005,224,599,1001,223,1,223,107,226,677,224,1002,223,2,223,1005,224,614,1001,223,1,223,1108,226,677,224,102,2,223,223,1006,224,629,101,1,223,223,7,677,677,224,1002,223,2,223,1005,224,644,1001,223,1,223,108,677,677,224,102,2,223,223,1005,224,659,101,1,223,223,1007,677,677,224,102,2,223,223,1006,224,674,101,1,223,223,4,223,99,226";
}
}
