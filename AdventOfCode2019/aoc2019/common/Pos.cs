using System;
using System.Diagnostics.CodeAnalysis;

namespace aoc2019
{
    public class Pos : IEquatable<Pos>
    {
        public int x;
        public int y;
        public Pos(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Pos(Pos other)
        {
            this.x = other.x;
            this.y = other.y;
        }

        public static Pos operator *(Pos p1, int n)
        {
            return new Pos(p1.x * n, p1.y * n);
        }

        public static Pos operator +(Pos p1, Pos p2)
        {
            return new Pos(p1.x + p2.x, p1.y + p2.y);
        }

        public override string ToString()
        {
            return $"({x}, {y})";
        }

        internal int manhattan(Pos inter)
        {
            return Math.Abs(x - inter.x) + Math.Abs(y - inter.y);
        }

        bool IEquatable<Pos>.Equals(Pos other)
        {
            return this.x == other.x && this.y == other.y;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Pos);
        }

        public bool Equals([AllowNull] Pos other)
        {
            return other != null &&
                   x == other.x &&
                   y == other.y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() * 7919 + y.GetHashCode();
        }
    }
}
