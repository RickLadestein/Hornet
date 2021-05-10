using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HornetEngine.Math
{
    public struct Vector4f : IEquatable<Vector4f>
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4f(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
            this.w = value;
        }

        public Vector4f(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float Length()
        {
            return MathF.Sqrt(LengthSqrt());
        }

        public float LengthSqrt()
        {
            return MathF.Pow(x, 2) +
                MathF.Pow(y, 2) +
                MathF.Pow(z, 2);
        }

        public static Vector4f operator +(Vector4f a, Vector4f b)
        {
            float _x = a.x + b.x;
            float _y = a.y + b.y;
            float _z = a.z + b.z;
            float _w = a.w + b.w;
            return new Vector4f(_x, _y, _z, _w);
        }

        public static Vector4f operator -(Vector4f a, Vector4f b)
        {
            float _x = a.x - b.x;
            float _y = a.y - b.y;
            float _z = a.y - b.y;
            float _w = a.w - b.w;
            return new Vector4f(_x, _y, _z, _w);
        }

        public static Vector4f operator *(Vector4f a, Vector4f b)
        {
            float _x = a.x * b.x;
            float _y = a.y * b.y;
            float _z = a.z * b.z;
            float _w = a.w * b.w;
            return new Vector4f(_x, _y, _z, _w);
        }

        public static Vector4f operator /(Vector4f a, Vector4f b)
        {
            float _x = a.x / b.x;
            float _y = a.y / b.y;
            float _z = a.z / b.z;
            float _w = a.w / b.w;
            return new Vector4f(_x, _y, _z, _w);
        }

        public bool Equals([AllowNull] Vector4f other)
        {
            bool xeq = this.x == other.x ? true : false;
            bool yeq = this.y == other.y ? true : false;
            bool zeq = this.z == other.z ? true : false;
            bool weq = this.w == other.w ? true : false;
            return xeq && yeq && zeq && weq;
        }
    }
}
