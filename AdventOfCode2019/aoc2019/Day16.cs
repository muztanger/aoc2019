using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{

    [TestClass]
    public class Day16
    {

        [TestMethod]
        public void Day16Part1Example1()
        {
            string example = "12345678";

            var arr = new List<int>(example.Select(x => int.Parse(x.ToString())));
            const int phases = 4;
            fft(ref arr, phases);
        }

        [TestMethod]
        public void Day16Part1Example2()
        {
            string example = "80871224585914546619083218645595";

            var arr = new List<int>(example.Select(x => int.Parse(x.ToString())));
            const int phases = 100;
            fft(ref arr, phases);
            var expectedString = "24176176";
            var expected = new List<int>(expectedString.Select(x => int.Parse(x.ToString())));
            for (int i = 0; i < expectedString.Length; i++)
            {
                Assert.AreEqual(expected[i], arr[i]);
            }
        }

        [TestMethod]
        public void Day16Part1Example3()
        {
            string example = "19617804207202209144916044189917";

            var arr = new List<int>(example.Select(x => int.Parse(x.ToString())));
            const int phases = 100;
            fft(ref arr, phases);
            Assert.AreEqual("73745418", String.Join("", arr.Take(8)));
        }

        [TestMethod]
        public void Day16Part1Example4()
        {
            string example = "69317163492948606335995924319873";

            var arr = new List<int>(example.Select(x => int.Parse(x.ToString())));
            const int phases = 100;
            fft(ref arr, phases);
            var expectedString = "52432133";
            var expected = new List<int>(expectedString.Select(x => int.Parse(x.ToString())));
            for (int i = 0; i < expectedString.Length; i++)
            {
                Assert.AreEqual(expected[i], arr[i]);
            }
        }

        private static void fft(ref List<int> arr, int phases)
        {
            List<int> arrCopy = arr;
            var basePattern = new int[] { 0, 1, 0, -1 };
            int baseCount = basePattern.Length;
            for (int phase = 1; phase <= phases; phase++)
            {
                var phaseResult = new List<int>();
                for (int repeat = 1; repeat <= arr.Count; repeat++)
                {
                    //int patternIndex = 0;
                    int result = 0;
                    Parallel.For(0, arr.Count, i =>
                    {
                        //if ((i + 1) % repeat == 0)
                        //{

                        //    patternIndex = (patternIndex + 1) % baseCount;
                        //}

                        int patternIndex = ((i + 1) / repeat) % baseCount;
                        Console.WriteLine($"Phase:{phase} Repeat:{repeat} Index:{i:D2} PatternIndex:{patternIndex}");

                        int multiplyer = basePattern[patternIndex];
                        //Console.Write($"{arr[i]} * {multiplyer} ");
                        Interlocked.Add(ref result, multiplyer * arrCopy[i]);
                        //result += multiplyer * arrCopy[i];
                    });
                    //for (int i = 0; i < arr.Count; i++)
                    //{
                    //    if ((i + 1) % repeat == 0)
                    //    {
                    //        patternIndex = (patternIndex + 1) % baseCount;
                    //    }

                    //    int multiplyer = basePattern[patternIndex];
                    //    //Console.Write($"{arr[i]} * {multiplyer} ");
                    //    result += multiplyer * arr[i];
                    //}
                    result = Math.Abs(result) % 10;
                    //Console.WriteLine($"= {result}");
                    phaseResult.Add(result);
                }
                //Console.WriteLine($"After {phase} phases: {String.Join("", phaseResult)}\n");
                arr = phaseResult;
            }
        }

        [TestMethod]
        public void Day16Part1()
        {
            var arr = new List<int>(input.Select(x => int.Parse(x.ToString())));
            const int phases = 100;
            fft(ref arr, phases);
            Console.WriteLine(String.Join("", arr.Take(8)));
            Assert.AreEqual("30550349", String.Join("", arr.Take(8)));
        }

        [TestMethod]
        public void Day16Part2()
        {
            //Console.WriteLine($"Starting {nameof(Day16Part2)}");
          
            var arr = new List<int>(input.Select(x => int.Parse(x.ToString())));
            var offset = int.Parse(input.Remove(8));
            var result = new List<int>();
            var j = 0;
            const int phases = 100;
            int[] basePattern = new int[] { 0, 1, 0, -1 };
            int baseCount = basePattern.Length;
            for (int i = 0; i < 10000; i++)
            {
                    //Console.WriteLine($"{i}");
                //if (i % 100 == 0)
                //{
                //    Console.WriteLine($"{i}");
                //}

                {
                    for (int phase = 1; phase <= phases; phase++)
                    {
                        var phaseResult = new List<int>();
                        for (int repeat = 1; repeat <= arr.Count; repeat++)
                        {
                            int patternIndex = 0;
                            int count = 1;
                            int resultNum = 0;
                            
                            for (int k = 0; k < arr.Count; k++)
                            {
                                //if (i != 0) Console.Write("+ ");
                                if (count % repeat == 0)
                                {
                                    patternIndex = (patternIndex + 1) % baseCount;
                                }

                                int multiplyer = basePattern[patternIndex];
                                //Console.Write($"{arr[i]} * {multiplyer} ");
                                resultNum += multiplyer * arr[k];
                                count++;

                            }
                            resultNum = Math.Abs(resultNum) % 10;
                            //Console.WriteLine($"= {result}");
                            phaseResult.Add(resultNum);
                        }
                        //Console.WriteLine($"After {phase} phases: {String.Join("", phaseResult)}\n");
                        arr = phaseResult;
                    }
                }
                
                if (j + arr.Count >= offset)
                {
                    result.AddRange(arr);
                }
                else
                {
                    j += arr.Count;
                }
            }
            //result.Select()
            Console.WriteLine(String.Join("", offset));
            // offset - 1 - j, out of range! took 49 minutes in debug
            arr.RemoveRange(0, offset - 1 - j);
            Console.WriteLine(String.Join("", arr.Take(8)));
            Assert.AreEqual("30550349", String.Join("", arr.Take(8)));
        }

        private static string input = @"59719811742386712072322509550573967421647565332667367184388997335292349852954113343804787102604664096288440135472284308373326245877593956199225516071210882728614292871131765110416999817460140955856338830118060988497097324334962543389288979535054141495171461720836525090700092901849537843081841755954360811618153200442803197286399570023355821961989595705705045742262477597293974158696594795118783767300148414702347570064139665680516053143032825288231685962359393267461932384683218413483205671636464298057303588424278653449749781937014234119757220011471950196190313903906218080178644004164122665292870495547666700781057929319060171363468213087408071790";
    }
}
