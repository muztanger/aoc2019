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
    
    [TestClass]
    public class Day10
    {
        [TestMethod]
        public void Part1Example1()
        {

            string example =
@".#..#
.....
#####
....#
...##";
            (Pos, int) best = FindBest(example);
            Console.WriteLine($"{best.Item1}: {best.Item2}");
            Assert.AreEqual(8, best.Item2);
        }

        [TestMethod]
        public void Part1Example2()
        {
            string example =
@"#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.";
            (Pos, int) best = FindBest(example);
            Console.WriteLine($"{best.Item1}: {best.Item2}");
            Assert.AreEqual(35, best.Item2);
        }

        [TestMethod]
        public void Part1Example3()
        {
            string example =
@"......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####";
            (Pos, int) best = FindBest(example);
            Console.WriteLine($"{best.Item1}: {best.Item2}");
            Assert.AreEqual(33, best.Item2);

        }

        [TestMethod]
        public void Part1Example4()
        {
            string example =
@".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#..";
            (Pos, int) best = FindBest(example);
            Console.WriteLine($"{best.Item1}: {best.Item2}");
            Assert.AreEqual(41, best.Item2);

        }

        private static (Pos, int) FindBest(string example)
        {
            Dictionary<Pos, int> counts = CountPerPosition(example);
            (Pos, int) best = (new Pos(0, 0), 0);
            foreach (var kv in counts)
            {
                if (best.Item2 < kv.Value)
                {
                    best = (kv.Key, kv.Value);
                }
                Console.WriteLine($"{kv.Key}: {kv.Value}");
            }

            return best;
        }

        private static Dictionary<Pos, int> CountPerPosition(string example)
        {
            List<int[]> rows = new List<int[]>();
            foreach (var line in Common.GetLines(example))
            {
                rows.Add(line.Select(x => x.Equals('#') ? 1 : 0).ToArray());
            }

            int N = rows.Count;
            Assert.AreEqual(rows[0].Length, N);

            Pos upperLeft = new Pos(0, 0);
            Pos lowerRight = new Pos(N, N);
            var positions = new List<Pos>();
            for (int row = 0; row < N; row++)
            {
                int[] line = rows[row];
                for (int col = 0; col < N; col++)
                {
                    if (line[col] > 0)
                    {
                        positions.Add(new Pos(col, row));
                        //if (row > lowerRight.y) lowerRight = new Pos(lowerRight.x, row);
                    }
                }
            }
            var counts = new Dictionary<Pos, int>();
            for (int i = 0; i < positions.Count; i++)
            {
                var vectors = new List<(Pos, Pos, Line)>();
                Pos p1 = positions[i];
                Console.WriteLine($"Search for {p1}");
                counts[p1] = 0;
                for (int j = 0; j < positions.Count; j++)
                {
                    if (j == i) continue;
                    var p2 = positions[j];
                    bool skip = false;
                    foreach (var vector in vectors) //TODO Lines are too general, need to check whether the point is on the other side and not colliding
                    {
                        if (vector.Item3.OnLine(p2) && !vector.Item1.Between(vector.Item2, p2))
                        {
                            Console.WriteLine($"   {p2} found on line");
                            skip = true;
                            break;
                        }
                    }
                    if (skip) continue;

                    counts[p1]++;
                    var nextLine = new Line(p1, p2);
                    vectors.Add((p1, p2, nextLine));
                }
            }

            return counts;
        }

        private bool WithinRange(Pos check, Pos upperLeft, Pos lowerRight)
        {
            if (check.x < upperLeft.x || check.x > lowerRight.x) return false;
            if (check.y < upperLeft.y || check.y > lowerRight.y) return false;
            return true;
        }

        [TestMethod]
        public void Part1()
        {
            string example =
@".###..#......###..#...#
#.#..#.##..###..#...#.#
#.#.#.##.#..##.#.###.##
.#..#...####.#.##..##..
#.###.#.####.##.#######
..#######..##..##.#.###
.##.#...##.##.####..###
....####.####.#########
#.########.#...##.####.
.#.#..#.#.#.#.##.###.##
#..#.#..##...#..#.####.
.###.#.#...###....###..
###..#.###..###.#.###.#
...###.##.#.##.#...#..#
#......#.#.##..#...#.#.
###.##.#..##...#..#.#.#
###..###..##.##..##.###
###.###.####....######.
.###.#####.#.#.#.#####.
##.#.###.###.##.##..##.
##.#..#..#..#.####.#.#.
.#.#.#.##.##########..#
#####.##......#.#.####.";
            (Pos, int) best = FindBest(example);
            Console.WriteLine($"{best.Item1}: {best.Item2}");
            Assert.AreEqual(230, best.Item2);
        }

        [TestMethod]
        public void Part2()
        {
        }

        private static string input = @"";
    }

    internal class Line
    {
        private readonly Pos mP1;
        private readonly Pos mP2;
        private bool mIsVertical;
        private int mDx;
        private int mDy;
        double mSlope = 0;

        public Line(Pos p1, Pos p2)
        {
            mP1 = p1;
            mP2 = p2;
            mIsVertical = p1.x == p2.x;
            mDx = p2.x - p1.x;
            mDy = p2.y - p1.y;
            if (!mIsVertical)
            {
                mSlope = mDy / (double)mDx;
            }
        }

        public bool OnLine(Pos pos)
        {
            return mDx * (pos.y - mP1.y) - mDy * (pos.x - mP1.x) == 0;
        }
    }
}
