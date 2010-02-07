using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    class Sphere : Primitive
    {
        public Vector Center;
        public float Radius;

        public Sphere(Vector center, float radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }

        public override bool Intersects(Ray r, out float t)
        {
            Vector dst = r.Origin - Center;

            float b = -dst.Dot(r.Direction);
            float d = b * b - dst.Dot(dst) + Radius * Radius;

            if (d < 0)
            {
                t = float.PositiveInfinity;
                return false;
            }

            //TODO: check both solutions, eg. camera in object ...
            t = b - (float)System.Math.Sqrt(d);
            
            if (t < 0) 
                t = b + (float)System.Math.Sqrt(d);

            return true;
        }

        public override Vector Normal(Vector p)
        {
            Vector n = p - Center;
            return n.Normalize();
        }

        public override AABB GetBoundingBox()
        {
            return new AABB(Center-Radius, Center+Radius);
        }
    }
}
