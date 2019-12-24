using System;
using System.Diagnostics.CodeAnalysis;

namespace aoc2019
{
    public class Pos3 : IEquatable<Pos3>
    {
        public int x;
        public int y;
        public int z;
        public Pos3(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Pos3(Pos3 other)
        {
            this.x = other.x;
            this.y = other.y;
            this.z = other.z;
        }

        public static Pos3 operator *(Pos3 p1, int n)
        {
            return new Pos3(p1.x * n, p1.y * n, p1.z * n);
        }

        public static Pos3 operator +(Pos3 p1, Pos3 p2)
        {
            return new Pos3(p1.x + p2.x, p1.y + p2.y, p1.z + p2.z);
        }
        public static Pos3 operator -(Pos3 p) => new Pos3(-p.x, -p.y, -p.z);
        
        public static Pos3 operator -(Pos3 p1, Pos3 p2) => p1 + (-p2);

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        //internal int manhattan(Pos inter)
        //{
        //    return Math.Abs(x - inter.x) + Math.Abs(y - inter.y) + Math.Abs(z - inter.z);
        //}

        bool IEquatable<Pos3>.Equals(Pos3 other)
        {
            return this.x == other.x && this.y == other.y && this.z == other.z;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Pos3);
        }

        public bool Equals([AllowNull] Pos3 other)
        {
            return other != null &&
                   x == other.x &&
                   y == other.y &&
                   z == other.z;
        }

        public override int GetHashCode()
        {
            return Tuple.Create(x, y, z).GetHashCode();
        }

        internal double Dist(Pos3 p1)
        {
            var delta = p1 - this;
            return Math.Sqrt(delta.x * (double) delta.x
                + delta.y * (double) delta.y
                + delta.z * (double) delta.z);
        }
    }
}
