using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019.day11
{
    enum OpCode
    {
        Add = 1,
        Multiply = 2,
        Read = 3,
        Write = 4,
        JumpIfTrue = 5,
        JumpIfFalse = 6,
        LessThan = 7,
        Equals = 8,
        AdjustsBase = 9,
        Stop = 99,
    }

    public class IntcodeComputer
    {
        public IntcodeComputer Next = null;
        public int Phase;
        public Queue<BigInteger> Input;
        public readonly string Id;
        public List<BigInteger> Output;

        private int mInstructionPointer = 0;
        private int mRelativeBase = 0;
        private BigInteger[] mMemory = new BigInteger[1000000];

        public IntcodeComputer(string id, int startValue = 1)
        {
            Input = new Queue<BigInteger>();
            Input.Enqueue(startValue);
            Output = new List<BigInteger>();
            this.Id = id;
        }

        public void Init(BigInteger[] init)
        {
            for (int i = 0; i < init.Length; i++)
            {
                mMemory[i] = init[i];
            }
        }

        public void Execute()
        {
            BigInteger opCode = GetOpCode(mMemory[mInstructionPointer]);
            int[] modes = GetModes(mMemory[mInstructionPointer]);
            //Console.WriteLine($"{Id}:INSTRUCTION {opCode} {String.Join(",", modes)}");

            int count = 0;
            while (opCode != 99)
            {
                count++;
                switch ((OpCode) (int) opCode)
                {
                    case OpCode.Add:
                        {
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            int memoryIndex = GetMemoryIndex(modes, 3);
                            mMemory[memoryIndex] = a + b;
                        }
                        mInstructionPointer += 4;
                        break;
                    case OpCode.Multiply:
                        {
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            int memoryIndex = GetMemoryIndex(modes, 3);
                            mMemory[memoryIndex] = a * b;
                        }
                        mInstructionPointer += 4;
                        break;
                    case OpCode.Read:
                        {
                            BigInteger val = Input.Dequeue();

                            //Console.WriteLine($"{Id}: READ val={val}");
                            int memoryIndex = GetMemoryIndex(modes);
                            mMemory[memoryIndex] = val;

                            mInstructionPointer += 2;
                        }
                        break;
                    case OpCode.Write:
                        {
                            BigInteger val = GetValue(ref modes, 1);
                            
                            //Console.WriteLine($"{Id}: OUTPUT {val} ");
                            Output.Add(val);
                            
                            mInstructionPointer += 2;
                        }
                        break;
                    case OpCode.JumpIfTrue: // jump -if true
                            // if the first parameter is non - zero, 
                            // it sets the instruction pointer to the value from the second parameter.Otherwise, it does nothing.
                        {
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            if (a > 0)
                            {
                                mInstructionPointer = (int) b;
                            }
                            else
                            {
                                mInstructionPointer += 3;
                            }
                        }
                        break;
                    case OpCode.JumpIfFalse: // jump -if-false
                            // if the first parameter is zero, it sets the instruction pointer to the value from the second parameter.
                            // Otherwise, it does nothing.
                        {
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            if (a == 0)
                            {
                                mInstructionPointer = (int) b;
                            }
                            else
                            {
                                mInstructionPointer += 3;
                            }
                        }
                        break;
                    case OpCode.LessThan: // is less than
                            // if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter.
                            // Otherwise, it stores 0.
                        {
                            Assert.IsTrue(modes.Count() <= 3);
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            BigInteger val = a < b ? 1 : 0;
                            int memoryIndex = GetMemoryIndex(modes, 3);
                            mMemory[memoryIndex] = val;
                            mInstructionPointer += 4;
                        }
                        break;
                    case OpCode.Equals: //is equals
                            // if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter.
                            // Otherwise, it stores 0.
                        {
                            Assert.IsTrue(modes.Count() <= 3);
                            GetValues(ref modes, out BigInteger a, out BigInteger b);
                            BigInteger val = a == b ? 1 : 0;
                            int memoryIndex = GetMemoryIndex(modes, 3);
                            mMemory[memoryIndex] = val;
                            mInstructionPointer += 4;
                        }
                        break;
                    case OpCode.AdjustsBase:
                        //Opcode 9 adjusts the relative base by the value of its only parameter.
                        //The relative base increases(or decreases, if the value is negative) by the value of the parameter.
                        //relativeBase
                        {
                            BigInteger x = GetValue(ref modes, 1);
                            Assert.IsTrue(x > int.MinValue);
                            Assert.IsTrue(x < int.MaxValue);
                            mRelativeBase += (int) x;
                            mInstructionPointer += 2;
                        }
                        break;
                    case OpCode.Stop: // stop
                        mInstructionPointer += 1;
                        //Console.WriteLine($"{Id}: STOP");
                        break;
                    default:
                        throw new Exception($"{Id}: Something went wrong opCode={opCode} count={count}");
                }
                opCode = GetOpCode(mMemory[mInstructionPointer]);
                modes = GetModes(mMemory[mInstructionPointer]);
                //Console.WriteLine($"{Id}: INSTRUCTION {mMemory[mInstructionPointer]} => {opCode} {String.Join(",", modes)}");
            }
        }

        private int GetMemoryIndex(int[] modes, int index = 1)
        {
            int memoryIndex = (int)mMemory[mInstructionPointer + index];
            int mode = 0;
            if (modes.Count() >= index)
            {
                mode = modes[index - 1];
            }
            
            if (mode == 1) Assert.Fail();

            if (mode == 2)
            {
                memoryIndex += mRelativeBase;
            }

            return memoryIndex;
        }

        private void GetValues(ref int[] modes, out BigInteger a, out BigInteger b)
        {
            a = GetValue(ref modes, 1);
            b = GetValue(ref modes, 2);
        }

        private BigInteger GetValue(ref int[] modes, int index)
        {
            Assert.IsTrue(index > 0);
            BigInteger result = 0; //position
            int mode = 0;
            if (modes.Count() >= index)
            {
                mode = modes[index - 1];
            }
            
            if (mode == 0)
            {
                result = mMemory[(int)mMemory[mInstructionPointer + index]];
            }
            else if (mode == 1)
            {
                //immediate
                result = mMemory[mInstructionPointer + index]; 
            }
            else if (mode == 2) 
            {
                //relative
                result = mMemory[mRelativeBase + (int)mMemory[mInstructionPointer + index]]; ;
            }
            else
            {
                Assert.Fail("mode invalid");
            }
            return result;
        }

        private int[] GetModes(BigInteger v)
        {
            // 0: position mode
            // 1: immediate mode
            v /= 100;
            List<int> modes = new List<int>();
            while (v > 0)
            {
                BigInteger x = v % 10;
                Assert.IsTrue(x >=0 && x <= 2, "x=" + x);
                modes.Add((int) v % 10);
                v /= 10;
            }
            return modes.ToArray();
        }

        private static BigInteger GetOpCode(BigInteger x)
        {
            BigInteger opCode = ((int) x) % 100;
            return opCode;
        }
    }

    [TestClass]
    public class Day09
    {
        [TestMethod]
        public void Part1Example()
        {

            {
                var computer = new IntcodeComputer("1");
                BigInteger[] init = Parse("109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99");
                computer.Init(init);
                computer.Execute();
                CollectionAssert.AreEqual(init, computer.Output, String.Join(",", computer.Output));
            }
            {
                var computer = new IntcodeComputer("2");
                BigInteger[] init = Parse("1102,34915192,34915192,7,4,7,99,0");
                computer.Init(init);
                computer.Execute();
                Assert.IsTrue(computer.Output.First() > BigInteger.Parse("999999999999999"));
            }
            {
                var computer = new IntcodeComputer("3");
                BigInteger[] init = Parse("104,1125899906842624,99");
                computer.Init(init);
                computer.Execute();
                Assert.AreEqual(computer.Output.First(), BigInteger.Parse("1125899906842624"));
            }
        }

        [TestMethod]
        public void Part1()
        {
            // 203 incorrect
            var computer = new IntcodeComputer("Computer");
            BigInteger[] init = Parse(input);
            computer.Init(init);
            computer.Execute();
            Console.WriteLine("OUTPUTS:\n" + String.Join("\n   ", computer.Output));
        }

        private static BigInteger[] Parse(string input1)
        {
            return input1.Split(",").Select(x => BigInteger.Parse(x.Trim())).ToArray();
        }

        [TestMethod]
        public void Part2()
        {
            var computer = new IntcodeComputer("Computer", 2);
            BigInteger[] init = Parse(input);
            computer.Init(init);
            computer.Execute();
            Console.WriteLine("OUTPUTS:\n" + String.Join("\n   ", computer.Output));
        }

        private static string input = @"1102,34463338,34463338,63,1007,63,34463338,63,1005,63,53,1102,3,1,1000,109,988,209,12,9,1000,209,6,209,3,203,0,1008,1000,1,63,1005,63,65,1008,1000,2,63,1005,63,904,1008,1000,0,63,1005,63,58,4,25,104,0,99,4,0,104,0,99,4,17,104,0,99,0,0,1102,1,22,1012,1101,309,0,1024,1102,1,29,1015,1101,0,30,1014,1101,0,221,1028,1102,24,1,1007,1102,32,1,1006,1102,1,31,1001,1101,0,20,1010,1101,34,0,1003,1102,899,1,1026,1101,304,0,1025,1101,0,1,1021,1101,892,0,1027,1101,0,0,1020,1101,0,484,1023,1101,25,0,1018,1101,0,21,1008,1102,491,1,1022,1102,212,1,1029,1102,1,23,1000,1101,0,26,1009,1102,36,1,1005,1101,27,0,1013,1101,35,0,1019,1101,38,0,1017,1101,0,39,1004,1102,37,1,1002,1102,33,1,1011,1102,28,1,1016,109,1,1208,5,35,63,1005,63,201,1001,64,1,64,1106,0,203,4,187,1002,64,2,64,109,36,2106,0,-9,4,209,1001,64,1,64,1105,1,221,1002,64,2,64,109,-30,2101,0,-4,63,1008,63,34,63,1005,63,247,4,227,1001,64,1,64,1105,1,247,1002,64,2,64,109,1,21108,40,40,8,1005,1016,265,4,253,1106,0,269,1001,64,1,64,1002,64,2,64,109,10,21101,41,0,-7,1008,1011,41,63,1005,63,295,4,275,1001,64,1,64,1105,1,295,1002,64,2,64,109,3,2105,1,3,4,301,1106,0,313,1001,64,1,64,1002,64,2,64,109,-18,2108,38,1,63,1005,63,329,1105,1,335,4,319,1001,64,1,64,1002,64,2,64,109,-11,2108,37,10,63,1005,63,357,4,341,1001,64,1,64,1106,0,357,1002,64,2,64,109,25,21107,42,41,-6,1005,1011,377,1001,64,1,64,1106,0,379,4,363,1002,64,2,64,109,-11,1207,3,25,63,1005,63,395,1105,1,401,4,385,1001,64,1,64,1002,64,2,64,109,-4,1202,0,1,63,1008,63,37,63,1005,63,423,4,407,1105,1,427,1001,64,1,64,1002,64,2,64,109,8,21102,43,1,6,1008,1016,43,63,1005,63,453,4,433,1001,64,1,64,1106,0,453,1002,64,2,64,109,-11,1208,6,36,63,1005,63,471,4,459,1105,1,475,1001,64,1,64,1002,64,2,64,109,21,2105,1,3,1001,64,1,64,1105,1,493,4,481,1002,64,2,64,109,-15,2107,22,3,63,1005,63,513,1001,64,1,64,1106,0,515,4,499,1002,64,2,64,109,-7,2107,35,7,63,1005,63,537,4,521,1001,64,1,64,1105,1,537,1002,64,2,64,109,23,1205,0,551,4,543,1105,1,555,1001,64,1,64,1002,64,2,64,109,-4,21101,44,0,-3,1008,1014,45,63,1005,63,579,1001,64,1,64,1105,1,581,4,561,1002,64,2,64,109,-15,2102,1,3,63,1008,63,33,63,1005,63,601,1106,0,607,4,587,1001,64,1,64,1002,64,2,64,109,23,1205,-5,623,1001,64,1,64,1106,0,625,4,613,1002,64,2,64,109,-7,21102,45,1,-8,1008,1010,43,63,1005,63,645,1105,1,651,4,631,1001,64,1,64,1002,64,2,64,109,-11,2102,1,1,63,1008,63,21,63,1005,63,677,4,657,1001,64,1,64,1106,0,677,1002,64,2,64,109,3,21107,46,47,4,1005,1014,695,4,683,1106,0,699,1001,64,1,64,1002,64,2,64,109,7,21108,47,48,-4,1005,1013,715,1106,0,721,4,705,1001,64,1,64,1002,64,2,64,109,-14,1201,0,0,63,1008,63,32,63,1005,63,741,1106,0,747,4,727,1001,64,1,64,1002,64,2,64,109,4,1201,2,0,63,1008,63,26,63,1005,63,769,4,753,1105,1,773,1001,64,1,64,1002,64,2,64,109,5,1207,-4,22,63,1005,63,795,4,779,1001,64,1,64,1106,0,795,1002,64,2,64,109,2,2101,0,-9,63,1008,63,34,63,1005,63,819,1001,64,1,64,1106,0,821,4,801,1002,64,2,64,109,-11,1202,1,1,63,1008,63,38,63,1005,63,841,1105,1,847,4,827,1001,64,1,64,1002,64,2,64,109,21,1206,-4,865,4,853,1001,64,1,64,1105,1,865,1002,64,2,64,109,3,1206,-6,877,1105,1,883,4,871,1001,64,1,64,1002,64,2,64,109,6,2106,0,-6,1001,64,1,64,1105,1,901,4,889,4,64,99,21101,0,27,1,21101,915,0,0,1106,0,922,21201,1,23692,1,204,1,99,109,3,1207,-2,3,63,1005,63,964,21201,-2,-1,1,21102,942,1,0,1106,0,922,21202,1,1,-1,21201,-2,-3,1,21101,0,957,0,1106,0,922,22201,1,-1,-2,1106,0,968,22102,1,-2,-2,109,-3,2106,0,0";
    }
}
