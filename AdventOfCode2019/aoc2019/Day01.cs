using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    [TestClass]
    public class Day01
    {
        [TestMethod]
        public void ExamplePart1()
        {
            /* Fuel required to launch a given module is based on its mass. Specifically, to find the fuel required for a module, take its mass, divide by three, round down, and subtract 2. */

            /* 
            For a mass of 12, divide by 3 and round down to get 4, then subtract 2 to get 2.
            For a mass of 14, dividing by 3 and rounding down still yields 4, so the fuel required is also 2.
            For a mass of 1969, the fuel required is 654.
            For a mass of 100756, the fuel required is 33583.
            */
            int fuel = fuelFromMass(12);
            Assert.AreEqual(2, fuelFromMass(12));
            Assert.AreEqual(2, fuelFromMass(14));
            Assert.AreEqual(654, fuelFromMass(1969));
            Assert.AreEqual(33583, fuelFromMass(100756));
        }

        private int fuelFromMass(int mass)
        {
            int result = mass / 3 - 2;
            return result >= 0 ? result : 0;
        }

        [TestMethod]
        public void Part1()
        {
            int sum = 0;
            using (StringReader reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int mass = int.Parse(line.Trim());
                    sum += fuelFromMass(mass);
                }
            }
            Console.WriteLine(sum);
        }

        [TestMethod]
        public void Part2()
        {
            int sum = 0;
            using (StringReader reader = new StringReader(input))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    int mass = int.Parse(line.Trim());
                    int fuel = fuelFromMass(mass);
                    while (fuel > 0)
                    {
                        sum += fuel;
                        fuel = fuelFromMass(fuel);
                    }
                }
            }
            Console.WriteLine(sum);
        }




        private static string input = @"94735
80130
127915
145427
89149
91232
100629
97340
86278
87034
147351
123045
91885
85973
64130
113244
58968
76296
127931
98145
120731
98289
110340
118285
60112
57177
58791
59012
66950
139387
145378
86204
147082
84956
134161
148664
74278
96746
144525
81214
70966
107050
134179
138587
80236
139871
104439
64643
145453
94791
51690
94189
148476
79956
81760
149796
109544
57533
142999
126419
115434
57092
64244
109663
94701
109265
145851
95183
84433
53818
106234
127380
149774
59601
138851
54488
100877
136952
61538
67705
60299
130769
113176
106723
133280
111065
63688
139307
122703
60162
89567
63994
66608
126376
136052
112255
98525
134023
141479
98200
";
    }
}
