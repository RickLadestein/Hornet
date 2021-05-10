using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Assimp;

namespace HornetEngine.Math
{
    public struct Vector2f : IEquatable<Vector2f>
    {
        public float x;
        public float y;

        public Vector2f(float value)
        {
            this.x = value;
            this.y = value;
        }

        public Vector2f(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Length()
        {
            return MathF.Sqrt(LengthSqrt());
        }

        public float LengthSqrt()
        {
            return MathF.Pow(x, 2) +
                MathF.Pow(y, 2);
        }

        public static Vector2f operator +(Vector2f a, Vector2f b)
        {
            float _x = a.x + b.x;
            float _y = a.y + b.y;
            return new Vector2f(_x, _y);
        }

        public static Vector2f operator -(Vector2f a, Vector2f b)
        {
            float _x = a.x - b.x;
            float _y = a.y - b.y;
            return new Vector2f(_x, _y);
        }

        public static Vector2f operator *(Vector2f a, Vector2f b)
        {
            float _x = a.x * b.x;
            float _y = a.y * b.y;
            return new Vector2f(_x, _y);
        }

        public static Vector2f operator /(Vector2f a, Vector2f b)
        {
            float _x = a.x / b.x;
            float _y = a.y / b.y;
            return new Vector2f(_x, _y);
        }

        public bool Equals([AllowNull] Vector2f other)
        {
            bool xeq = this.x == other.x ? true : false;
            bool yeq = this.y == other.y ? true : false;
            return xeq && yeq;

        }
    }
}
