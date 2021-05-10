using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace HornetEngine.Math
{
    public struct Vector3f : IEquatable<Vector3f>
    {
        public float x;
        public float y;
        public float z;

        public Vector3f(float value)
        {
            this.x = value;
            this.y = value;
            this.z = value;
        }

        public Vector3f(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
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

        public static Vector3f operator +(Vector3f a, Vector3f b)
        {
            float _x = a.x + b.x;
            float _y = a.y + b.y;
            float _z = a.z + b.z;
            return new Vector3f(_x, _y, _z);
        }

        public static Vector3f operator -(Vector3f a, Vector3f b)
        {
            float _x = a.x - b.x;
            float _y = a.y - b.y;
            float _z = a.y - b.y;
            return new Vector3f(_x, _y, _z);
        }

        public static Vector3f operator *(Vector3f a, Vector3f b)
        {
            float _x = a.x * b.x;
            float _y = a.y * b.y;
            float _z = a.z * b.z;
            return new Vector3f(_x, _y, _z);
        }

        public static Vector3f operator /(Vector3f a, Vector3f b)
        {
            float _x = a.x / b.x;
            float _y = a.y / b.y;
            float _z = a.z / b.z;
            return new Vector3f(_x, _y, _z);
        }

        public bool Equals([AllowNull] Vector3f other)
        {
            bool xeq = this.x == other.x ? true : false;
            bool yeq = this.y == other.y ? true : false;
            bool zeq = this.z == other.z ? true : false;
            return xeq && yeq && zeq;
        }
    }
}
