using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmackBrosClient
{
    class Vector2
    {
        private float x;
        private float y;
        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        public Vector2(float a, float b)
        {
            X = a;
            Y = b;
        }
        public Vector2(Vector2 v)
        {
            this.X = v.X;
            this.Y = v.Y;
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(Math.Pow(X, 2) + Math.Pow(Y, 2));
            }
        }
        public static float Distance(Vector2 A, Vector2 B)
        {
            return (float)Math.Sqrt(Math.Pow(B.Y - A.Y, 2) + Math.Pow(B.X - A.X, 2));
        }
        public static float Dot(Vector2 A, Vector2 B)
        {
            return A.X * B.X + A.Y * B.Y;
        }
        public static Vector2 operator +(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X + B.X, A.Y + B.Y);
        }
        public static Vector2 operator -(Vector2 A, Vector2 B)
        {
            return new Vector2(A.X - B.X, A.Y - B.Y);
        }
        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.X, -v.Y);
        }
        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return (a.X == b.X) && (a.Y == b.Y);
        }
        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }
        public static Vector2 operator *(Vector2 a, float scalar)
        {
            return new Vector2(a.X * scalar, a.Y * scalar);
        }
        public double Dot(Vector2 other)
        {
            return Dot(this, other);
        }
        public static Vector2 operator /(Vector2 v1, double s2)
        {
            return
            (
               new Vector2
               (
                  (float)(v1.X / s2),
                  (float)(v1.Y / s2)
               )
            );
        }
        public static bool IsUnitVector(Vector2 v1)
        {
            return v1.Length == 1;
        }
        public bool IsUnitVector()
        {
            return IsUnitVector(this);
        }
        public Vector2 Normalize()
        {
            var magnitude = this.Length;
            if (magnitude == 0)
            {
                return new Vector2(0, 0);
            }
            var inverse = 1 / magnitude;
            return new Vector2(this.X * inverse, this.Y * inverse);
        }
        public static Vector2 Projection(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v2 * ((float)(v1.Dot(v2) / Math.Pow(v2.Length, 2))));
        }

        public Vector2 Projection(Vector2 direction)
        {
            return Projection(this, direction);
        }
    }
}

