using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace aoc2019
{
    class Moon
    {
        public Pos3 pos;
        public Pos3 vel;
        public Pos3 gravity;

        public Moon()
        {
            gravity = new Pos3(0, 0, 0);
        }

        public Moon(Moon other)
        {
            this.pos = other.pos;
            this.vel = other.vel;
            this.gravity = other.gravity;
        }

        internal void Gravity(Moon other)
        {
            {
                var a = Compare(this.pos.x, other.pos.x);
                gravity.x += a;
                vel.x += a;
            }
            {
                var a = Compare(this.pos.y, other.pos.y);
                gravity.y += a;
                vel.y += a;
            }
            {
                var a = Compare(this.pos.z, other.pos.z);
                gravity.z += a;
                vel.z += a;
            }
        }

        internal int Compare(int a, int b)
        {
            return a > b ? -1 : a < b ? 1 : 0;
        }

        public override string ToString()
        {
            return $"pos={pos} vel={vel}";
        }

        internal Pos3 Step()
        {
            pos += vel;
            Pos3 result = gravity;
            gravity = new Pos3(0, 0, 0);
            return result;
        }

        internal int PotentialEnergy()
        {
            return new int[3] { pos.x, pos.y, pos.z }.Aggregate(0, (sum, w) => sum + Math.Abs(w));
        }

        internal int KineticEnergy()
        {
            return new int[3] { vel.x, vel.y, vel.z }.Aggregate(0, (sum, w) => sum + Math.Abs(w));
        }

        internal int Energy()
        {
            return PotentialEnergy() * KineticEnergy();
        }
    }

    [TestClass]
    public class Day12
    {
        [TestMethod]
        public void Part1Example1()
        {
            var poss = new List<Pos3>()
            {
                new Pos3(-1,   0,  2),
                new Pos3( 2, -10, -7),
                new Pos3( 4,  -8,  8),
                new Pos3( 3,   5, -1),
            };
            int energy = Energy(poss, 10);
            Assert.AreEqual(179, energy);
        }

        [TestMethod]
        public void Part1Example2()
        {
            var poss = new List<Pos3>()
            {
                new Pos3(-8, -10,  0),
                new Pos3( 5,   5, 10),
                new Pos3( 2,  -7,  3),
                new Pos3( 9,  -8, -3),
            };
            int energy = Energy(poss, 100);
            Assert.AreEqual(1940, energy);
        }

        [TestMethod]
        public void Part1()
        {
            var poss = new List<Pos3>()
            {
                new Pos3(14,  15, -2),
                new Pos3(17,  -3,  4),
                new Pos3( 6,  12, -13),
                new Pos3(-2,  10, -8),
            };
            int energy = Energy(poss, 1000);
            Assert.AreEqual(10189, energy);
        }

        [TestMethod]
        public void Part2Example1()
        {
            var poss = new List<Pos3>()
            {
                new Pos3(-1,   0,  2),
                new Pos3( 2, -10, -7),
                new Pos3( 4,  -8,  8),
                new Pos3( 3,   5, -1),
            };
            Assert.AreEqual(2772, ConvergenceCount(poss));
        }

        [TestMethod]
        public void Part2Example2()
        {
            var poss = new List<Pos3>()
            {
                new Pos3(-8, -10,  0),
                new Pos3( 5,   5, 10),
                new Pos3( 2,  -7,  3),
                new Pos3( 9,  -8, -3),
            };
            Assert.AreEqual(4686774924, ConvergenceCount(poss));
        }

        private static int Energy(List<Pos3> poss, int iterations)
        {
            List<Moon> moons = Init(poss);

            for (int i = 0; i < iterations; i++)
            {
                //Console.WriteLine($"Step {i + 1}");
                Next(moons);
                //Print(moons);
            }

            return SystemEnergy(moons);
        }
        private static long ConvergenceCount(List<Pos3> poss)
        {
            List<Moon> moons = Init(poss);
            
            long count = 0;
            bool moving = true;
            //var origo = new Pos3(0, 0, 0);
            while (moving)
            {
                Next(moons);
                moving = false;

                foreach (var moon in moons)
                {
                    if (moon.vel.x != 0 || moon.vel.y != 0 || moon.vel.z !=0 )
                    {
                        moving = true;
                        break;
                    }
                }
                count++;
            }

            return count * 2;
        }


        private static int SystemEnergy(List<Moon> moons)
        {
            int energy = 0;
            foreach (var moon in moons)
            {
                energy += moon.Energy();
            }

            return energy;
        }

        private static void Next(List<Moon> moons)
        {
            Gravity(moons);
            Step(moons);
        }

        private static List<Moon> Init(List<Pos3> poss)
        {
            var vels = new List<Pos3>()
            {
                new Pos3(0, 0, 0),
                new Pos3(0, 0, 0),
                new Pos3(0, 0, 0),
                new Pos3(0, 0, 0),
            };

            var moons = new List<Moon>();
            for (int i = 0; i < poss.Count; i++)
            {
                moons.Add(new Moon() { pos = poss[i], vel = vels[i] });
            }

            return moons;
        }

        private static void Print(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                Console.WriteLine(moon);
            }
        }

        private static void Step(List<Moon> moons)
        {
            foreach (var moon in moons)
            {
                Pos3 gravity = moon.Step();
                Console.WriteLine($"{moon} {gravity}");
            }
            Console.WriteLine("---");
        }

        private static void Gravity(List<Moon> moons)
        {
            for (int i = 0; i < moons.Count - 1; i++)
            {
                var m1 = moons[i];
                for (int j = i + 1; j < moons.Count; j++)
                {
                    var m2 = moons[j];
                    m1.Gravity(m2);
                    m2.Gravity(m1);
                }
            }
        }

        private static string input = @"<x=14, y=15, z=-2>
<x=17, y=-3, z=4>
<x=6, y=12, z=-13>
<x=-2, y=10, z=-8>
";
    }
}
