using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    internal class Triangle : Primitive
    {
        private Vector A;
        private Vector B;
        private Vector C;
        private Vector normal;

        public Triangle(Vector a, Vector b, Vector c, Material material)
        {
            A = a;
            B = b;
            C = c;
            Material = material;

            normal = Vector.Cross(B - A, C - A).Normalize();

        }

        public override bool Intersects(Ray r, out float t)
        {
            Vector u = B - A;
            Vector v = C - A;

            float a = -normal.Dot(r.Origin - A);
            float b = normal.Dot(r.Direction);

            if (System.Math.Abs(b) < 0.0000001f)
            {
                // TODO: move to begining or use ref
                t = float.PositiveInfinity;
                return false;
            }

            t = a/b;

            if (t < 0)
                return false;

            Vector ip = r.Origin + r.Direction*t;

            float uu = u.Dot(u);
            float uv = u.Dot(v);
            float vv = v.Dot(v);

            Vector w = ip - A;
            float wu = w.Dot(u);
            float wv = w.Dot(v);

            float D = uv*uv - uu*vv;

            float pr = (uv*wv - vv*wu)/D;
            float ps = (uv*wu - uu*wv)/D;

            if (pr < 0 || pr > 1 || ps < 0 || pr+ps > 1)
            {
                t = float.PositiveInfinity;
                return false;
            }

            return true;
        }

        public override Vector Normal(Vector p)
        {
            return normal;
        }

        public override AABB GetBoundingBox()
        {
            return new AABB(new Vector(MathHelper.Min(A.X, B.X, C.X), MathHelper.Min(A.Y, B.Y, C.Y),MathHelper.Min(A.Z, B.Z, C.Z)),
                            new Vector(MathHelper.Max(A.X, B.X, C.X), MathHelper.Max(A.Y, B.Y, C.Y),MathHelper.Max(A.Z, B.Z, C.Z)));
        }
    }
}
    