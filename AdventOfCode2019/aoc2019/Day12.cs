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

        internal void Gravity(Moon other)
        {
            vel.x += this.pos.x > other.pos.x ? -1 : 0;
            vel.x += this.pos.x < other.pos.x ? 1 : 0;

            vel.y += this.pos.y > other.pos.y ? -1 : 0;
            vel.y += this.pos.y < other.pos.y ? 1 : 0;

            vel.z += this.pos.z > other.pos.z ? -1 : 0;
            vel.z += this.pos.z < other.pos.z ? 1 : 0;
        }

        public override string ToString()
        {
            return $"pos={pos} vel={vel}";
        }

        internal void Step()
        {
            pos += vel;
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
            Assert.AreEqual(1940, energy);
        }

        private static int Energy(List<Pos3> poss, int iterations)
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

            for (int i = 0; i < iterations; i++)
            {
                //Console.WriteLine($"Step {i + 1}");
                Gravity(moons);
                Step(moons);
                //Print(moons);
            }

            int energy = 0;
            foreach (var moon in moons)
            {
                energy += moon.Energy();
            }

            return energy;
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
                moon.Step();
            }
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
