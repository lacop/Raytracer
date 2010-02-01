using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    class Plane : Primitive
    {
        //TODO: protect with properties and autonormalize
        public Vector Norm;
        public float Dist;

        public Plane(Vector norm, float dist, Material material)
        {
            Norm = norm.Normalize();
            Dist = dist;
            Material = material;
        }

        public override bool Intersects(Ray r, out float t)
        {
            float d = Norm.Dot(r.Direction.Normalize());
            if (d == 0)
            {
                t = float.PositiveInfinity;
                return false;
            }

            float dist = -(Norm.Dot(r.Origin) + Dist) / d;
            if (dist <= 0)
            {
                t = float.PositiveInfinity;
                return false;
            }

            t = dist;
            return true;

        }

        public override Vector Normal(Vector p)
        {
            return Norm;
        }
    }
}
