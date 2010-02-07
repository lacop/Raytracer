using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    class Vector
    {
        public float X, Y, Z;

        public Vector(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float Length ()
        {
            return (float) System.Math.Sqrt(X*X + Y*Y + Z*Z);
        }

        public Vector Normalize ()
        {
            float invlen = 1.0f/Length();
            return new Vector(X*invlen, Y*invlen, Z*invlen);
        }

        public Vector Inverse ()
        {
            return new Vector(1.0f/X, 1.0f/Y, 1.0f/Z);
        }
        
        //TODO: static versions
        public float Dot (Vector v)
        {
            return X*v.X + Y*v.Y + Z*v.Z;
        }

        public static Vector Cross (Vector a, Vector b)
        {
            return new Vector(a.Y*b.Z - a.Z*b.Y, a.Z*b.X - a.X*b.Z, a.X*b.Y - a.Y*b.X);
        }

        public static Vector operator+ (Vector a, Vector b)
        {
            return new Vector(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }
        public static Vector operator- (Vector a, Vector b)
        {
            return new Vector(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static Vector operator+ (Vector v, float a)
        {
            return new Vector(v.X + a, v.Y + a, v.Z + a);
        }
        public static Vector operator- (Vector v, float a)
        {
            return new Vector(v.X - a, v.Y - a, v.Z - a);
        }

        public static Vector operator* (Vector a, float f)
        {
            return new Vector(a.X*f, a.Y*f, a.Z*f);
        }

        public override string ToString()
        {
            return string.Format("({0}, {1}, {2})", X, Y, Z);
        }
    }
}
