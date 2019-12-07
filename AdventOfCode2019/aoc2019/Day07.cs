using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    public class Amplifier
    {
        public Amplifier Next = null;
        public int Phase;
        public int Input;
        public ManualResetEvent Signal;
        public int Output;

        public Amplifier(int phase, ManualResetEvent signal)
        {
            Phase = phase;
            Signal = signal;
        }

        public void compute(int[] init)
        {
            int instructionPointer = 0;
            var memory = new int[1000000];
            for (int i = 0; i < init.Length; i++)
            {
                memory[i] = init[i];
            }

            int opCode = GetOpCode(memory[0]);
            bool[] modes = GetModes(memory[0]);
            Console.WriteLine($"INSTRUCTION: {opCode} {String.Join(",", modes)}");
            bool isPhase = true;

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
                        {
                            Console.WriteLine($"READ: isPhase={isPhase} Phase={Phase} Input={Input}");
                            int val = Input;
                            if (isPhase)
                            {
                                val = Phase;
                                isPhase = false;
                            }
                            Console.WriteLine($"      val={val}");
                            memory[memory[instructionPointer + 1]] = val; // from text
                            instructionPointer += 2;
                        }
                        break;
                    case 4: // write
                        {
                            int val;
                            if (modes.Count() > 0 && modes[1]) // check
                            {
                                val = memory[instructionPointer + 1];
                            }
                            else
                            {
                                val = memory[memory[instructionPointer + 1]];
                            }
                            Console.WriteLine("OUTPUT: " + val);
                            Output = val;
                            if (Next != null)
                            {
                                Next.Input = val;
                                Next.Signal.Set();
                            }
                            instructionPointer += 2;
                        }
                        break;
                    case 5: // jump -if true
                            // if the first parameter is non - zero, 
                            // it sets the instruction pointer to the value from the second parameter.Otherwise, it does nothing.
                        {
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            if (a > 0)
                            {
                                instructionPointer = b;
                            }
                            else
                            {
                                instructionPointer += 3;
                            }
                        }
                        break;
                    case 6: // jump -if-false
                            // if the first parameter is zero, it sets the instruction pointer to the value from the second parameter.
                            // Otherwise, it does nothing.
                        {
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            if (a == 0)
                            {
                                instructionPointer = b;
                            }
                            else
                            {
                                instructionPointer += 3;
                            }
                        }
                        break;
                    case 7: // is less than
                            // if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter.
                            // Otherwise, it stores 0.
                        {
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            int val = a < b ? 1 : 0;
                            memory[memory[instructionPointer + 3]] = val;
                            instructionPointer += 4;
                        }
                        break;
                    case 8: //is equals
                            // if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter.
                            // Otherwise, it stores 0.
                        {
                            Assert.IsTrue(modes.Count() < 3);
                            GetValues(instructionPointer, ref memory, ref modes, out int a, out int b);
                            int val = a == b ? 1 : 0;
                            memory[memory[instructionPointer + 3]] = val;
                            instructionPointer += 4;
                        }
                        break;
                    case 99: // stop
                        instructionPointer += 1;
                        break;
                    default:
                        throw new Exception($"Something went wrong opCode={opCode} count={count}");
                }
                opCode = GetOpCode(memory[instructionPointer]);
                modes = GetModes(memory[instructionPointer]);
                Console.WriteLine($"INSTRUCTION: {memory[instructionPointer]} => {opCode} {String.Join(",", modes)}");
            }
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
    }


    [TestClass]
    public class Day07
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Part1()
        {
            int max = int.MinValue;
            int[] phaseSettings = new int[] { 0, 1, 2, 3, 4 };
            Common.ForAllPermutation<int>(phaseSettings, (int[] phases) =>
            {
                List<Amplifier> amplifiers = new List<Amplifier>();
                ManualResetEvent signal = new ManualResetEvent(false);
                Amplifier last = null;
                for (int i = 0; i < 5; i++)
                {
                    Amplifier amp = new Amplifier(phases[i], signal);
                    if (i == 0)
                    {
                        amp.Input = 0;
                        amp.Signal.Set();
                    }
                    if (last != null)
                    {
                        last.Next = amp;
                    }
                    amplifiers.Add(amp);

                    last = amp;
                }

                // find the largest output signal that can be sent to the thrusters by trying every possible combination of phase settings on the amplifiers
                foreach (var amp in amplifiers)
                {
                    var init = input.Split(",").Select(x => int.Parse(x.Trim())).ToArray();
                    amp.compute(init);
                }
                Console.WriteLine($"Result={amplifiers.Last().Output}");
                max = Math.Max(max, amplifiers.Last().Output);
                return false;
            });
            Console.WriteLine($"Final result: {max}");
        }

        

        

        [TestMethod]
        public void Part2()
        {


            

        }





            
        private static string input = @"3,8,1001,8,10,8,105,1,0,0,21,38,47,64,85,106,187,268,349,430,99999,3,9,1002,9,4,9,1001,9,4,9,1002,9,4,9,4,9,99,3,9,1002,9,4,9,4,9,99,3,9,1001,9,3,9,102,5,9,9,1001,9,5,9,4,9,99,3,9,101,3,9,9,102,5,9,9,1001,9,4,9,102,4,9,9,4,9,99,3,9,1002,9,3,9,101,2,9,9,102,4,9,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,99";
}
}
