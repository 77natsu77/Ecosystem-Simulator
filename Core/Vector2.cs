using System;

namespace Ecosystem_Simulator.Core
{
    public struct Vector2
    {
        public float X;
        public float Y;

        public Vector2(float x, float y) { X = x; Y = y; }

        // Addition: Vector + Vector
        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.X + b.X, a.Y + b.Y);

        // Subtraction: Vector - Vector
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.X - b.X, a.Y - b.Y);

        // Multiplication: Vector * Scalar (scaling up)
        public static Vector2 operator *(Vector2 a, float b) => new Vector2(a.X * b, a.Y * b);

        // Division: Vector / Scalar (scaling down)
        public static Vector2 operator /(Vector2 a, float b) => new Vector2(a.X / b, a.Y / b);

        public float Length => (float)Math.Sqrt(X * X + Y * Y);
    }
}