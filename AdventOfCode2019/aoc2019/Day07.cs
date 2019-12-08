using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    public class Amplifier
    {
        public Amplifier Next = null;
        public int Phase;
        public Queue<int> Input;
        public AutoResetEvent Signal;
        public readonly string Id;
        public int Output;
        
        private bool isPhase = true;
        private int instructionPointer = 0;
        private int[] memory = new int[1000000];

        public Amplifier(int phase, AutoResetEvent signal, string id)
        {
            Phase = phase;
            Signal = signal;
            Input = new Queue<int>();
            this.Id = id;
        }

        public void init(int[] init)
        {
            for (int i = 0; i < init.Length; i++)
            {
                memory[i] = init[i];
            }
        }

        public async Task<bool> compute()
        {
            //TODO Reset instruction pointer??
            //instructionPointer = 0;
            int opCode = GetOpCode(memory[instructionPointer]);
            bool[] modes = GetModes(memory[instructionPointer]);
            Console.WriteLine($"{Id}:INSTRUCTION {opCode} {String.Join(",", modes)}");
            
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
                            Console.WriteLine($"{Id}: READ isPhase={isPhase} Phase={Phase} Input={Input}");
                            int val;
                            if (isPhase)
                            {
                                val = Phase;
                                isPhase = false;
                            }
                            else
                            {
                                Console.WriteLine($"{Id}: Wait for signal... ");
                                Signal.WaitOne();
                                val = Input.Dequeue();
                                Console.WriteLine($"{Id}: Signal received");
                            }
                            Console.WriteLine($"{Id}: val={val}");
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
                            Console.WriteLine($"{Id}: OUTPUT {val} ");
                            Output = val;
                            //if (Next != null)
                            //{
                                Next.Input.Enqueue(val);
                                Next.Signal.Set();
                            //}
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
                        Console.WriteLine($"{Id}: STOP");
                        break;
                    default:
                        throw new Exception($"{Id}: Something went wrong opCode={opCode} count={count}");
                }
                opCode = GetOpCode(memory[instructionPointer]);
                modes = GetModes(memory[instructionPointer]);
                Console.WriteLine($"{Id}: INSTRUCTION {memory[instructionPointer]} => {opCode} {String.Join(",", modes)}");
            }
            return true;
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
                var signal = new AutoResetEvent(false);
                Amplifier last = null;
                string names = "ABCDE";
                for (int i = 0; i < 5; i++)
                {
                    Amplifier amp = new Amplifier(phases[i], signal, names[i].ToString());
                    if (i == 0)
                    {
                        amp.Input.Enqueue(0);
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
                    amp.init(init);
                    amp.compute();
                }
                Console.WriteLine($"Result={amplifiers.Last().Output}");
                max = Math.Max(max, amplifiers.Last().Output);
                return false;
            });
            Console.WriteLine($"Final result: {max}");
            Assert.AreEqual(366376, max);
        }

        [TestMethod]
        public void Part2()
        {
            // feedback loop
            // input integers from 5 to 9
            int max = int.MinValue;
            string names = "ABCDE";
            int[] phaseSettings = new int[] { 5, 6, 7, 8, 9 };
            Common.ForAllPermutation<int>(phaseSettings, (int[] phases) =>
            {
                List<Amplifier> amplifiers = new List<Amplifier>();
                Amplifier last = null;
                for (int i = 0; i < 5; i++)
                {
                    var signal = new AutoResetEvent(false);
                    Amplifier amp = new Amplifier(phases[i], signal, names[i].ToString());
                    if (i == 0)
                    {
                        amp.Input.Enqueue(0);
                        amp.Signal.Set();
                    }
                    if (last != null)
                    {
                        last.Next = amp;
                    }
                    amplifiers.Add(amp);

                    last = amp;
                }
                amplifiers.Last().Next = amplifiers.First();

                // find the largest output signal that can be sent to the thrusters by trying every possible combination of phase settings on the amplifiers
                foreach (var amp in amplifiers)
                {
                    amp.init(input.Split(",").Select(x => int.Parse(x.Trim())).ToArray());
                }
                var tasks = new List<Task>();
                for (int i = 0; i < amplifiers.Count; i++)
                {
                    Amplifier amp = amplifiers[i];
                    var t = Task.Run(async delegate
                    {
                        await amp.compute();
                    });
                    tasks.Add(t);
                }
                Task.WaitAll(tasks.ToArray());

                Console.WriteLine($"Result={amplifiers.Last().Output}");
                max = Math.Max(max, amplifiers.Last().Output);
                return false;
            });
            Console.WriteLine($"Final result: {max}");
            Assert.AreEqual(21596786, max);
        }

        private static string input = @"3,8,1001,8,10,8,105,1,0,0,21,38,47,64,85,106,187,268,349,430,99999,3,9,1002,9,4,9,1001,9,4,9,1002,9,4,9,4,9,99,3,9,1002,9,4,9,4,9,99,3,9,1001,9,3,9,102,5,9,9,1001,9,5,9,4,9,99,3,9,101,3,9,9,102,5,9,9,1001,9,4,9,102,4,9,9,4,9,99,3,9,1002,9,3,9,101,2,9,9,102,4,9,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,101,2,9,9,4,9,3,9,101,1,9,9,4,9,99,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,99";
    }
}
