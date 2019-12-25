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
            string example =  @".#..#..###
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
                //Console.WriteLine($"{kv.Key}: {kv.Value}");
            }

            return best;
        }

        private static Dictionary<Pos, int> CountPerPosition(string example)
        {
            List<Pos> positions = GetPositions(example);

            var counts = new Dictionary<Pos, int>();
            for (int i = 0; i < positions.Count; i++)
            {
                var vectors = new List<(Pos, Pos, Line)>();
                Pos p1 = positions[i];
                //Console.WriteLine($"Search for {p1}");
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
                            //Console.WriteLine($"   {p2} found on line");
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

        private static List<Pos> GetPositions(string example)
        {
            List<int[]> rows = new List<int[]>();
            foreach (var line in Common.GetLines(example))
            {
                rows.Add(line.Trim().Select(x => x.Equals('.') ? 0 : 1).ToArray());
            }

            int Rows = rows.Count;
            int Cols = rows[0].Length;
            //Assert.AreEqual(rows[0].Length, Rows);

            Pos upperLeft = new Pos(0, 0);
            Pos lowerRight = new Pos(Cols, Rows);
            var positions = new List<Pos>();
            for (int row = 0; row < Rows; row++)
            {
                int[] line = rows[row];
                for (int col = 0; col < Cols; col++)
                {
                    if (line[col] > 0)
                    {
                        positions.Add(new Pos(col, row));
                        //if (row > lowerRight.y) lowerRight = new Pos(lowerRight.x, row);
                    }
                }
            }

            return positions;
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
        public void Part2Example1()
        {
            string example =
@".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##";
            (Pos, int) best = FindBest(example);

            Pos bestPos = best.Item1;
            Console.WriteLine($"{bestPos}: {best.Item2}");
            Assert.AreEqual(new Pos(11,13), bestPos);

            var positionsList = GetPositions(example);
            var positions = new Dictionary<double, List<Pos>>();
            Console.WriteLine($"Best: {bestPos}");
            foreach (var pos in positionsList)
            {
                double angle = 0;
                Pos delta = pos - bestPos;
                //Console.WriteLine($" delta={delta} {pos}");
                if (delta == new Pos(0, 0) || delta.Dist(new Pos(0,0)) < Double.Epsilon) continue; // skip best

                if (delta.x == 0)
                {
                    angle = delta.y < 0 ? 0 : Math.PI;
                }
                else if (delta.y == 0)
                {
                    angle = delta.x > 0 ? Math.PI / 2.0 : 3.0 * Math.PI / 2.0;
                }
                else if (delta.x > 0 && delta.y < 0) // #1
                {
                    angle = Math.Atan(-delta.x / (double) delta.y);
                }
                else if (delta.x > 0 && delta.y > 0) // #2
                {
                    angle = Math.PI / 2 + Math.Atan(delta.y / (double) delta.x);
                }
                else if (delta.x < 0 && delta.y > 0) // #3
                {
                    angle = Math.PI + Math.Atan(-delta.x / (double) delta.y);
                }
                else
                {
                    Assert.IsTrue(delta.x < 0);
                    Assert.IsTrue(delta.y < 0);
                    angle = 3 * Math.PI / 2.0 + Math.Atan(-delta.y / (double)delta.x);
                }
                //Console.WriteLine($"{pos} {delta} angle={angle / (2.0 * Math.PI) * 360}");
                if (!positions.ContainsKey(angle))
                {
                    positions[angle] = new List<Pos>() { pos };
                }
                else
                {
                    positions[angle].Add(pos);
                }
            }
            var keys = positions.Keys.ToArray();
            Array.Sort(keys);

            // sort
            foreach (var key in keys)
            {
                var list = positions[key];
                Console.Write($"{key}: ");
                list.Sort(delegate (Pos p1, Pos p2) { return bestPos.Dist(p1) < bestPos.Dist(p2) ? -1 : bestPos.Dist(p1) == bestPos.Dist(p2) ? 0: 1; });
                foreach (var elem in list)
                {
                    Console.Write($"{elem}:{bestPos.Dist(elem)} ");
                }
                Console.WriteLine();
                positions[key] = list;
            }

            int count = 0;
            Pos lastRemoved = null;

            var printThis = new List<int>() { 1, 2, 3, 10, 20, 50, 100, 199, 200, 201, 299 };
            int N = 299;
            while (count < N)
            {
                foreach (var key in keys)
                {
                    if (positions[key].Any())
                    {
                        var list = positions[key];
                        lastRemoved = list[0];
                        count++;
                        if (true || printThis.Contains(count))
                        {
                            Console.WriteLine($"{count} Removing {lastRemoved}");
                        }
                        list.RemoveAt(0);
                        positions[key] = list;
                        if (count >= N) break;
                    }
                }
            }
            Console.WriteLine($"Last removed: {lastRemoved}");
        }


        [TestMethod]
        public void Part2()
        {
        }

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
