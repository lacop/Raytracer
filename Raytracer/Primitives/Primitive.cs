using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    abstract class Primitive
    {
        //TODO: base constructor for debug name and material
        //TODO: debug name
        public Material Material;

        public abstract bool Intersects(Ray r, out float t);
        public abstract Vector Normal(Vector p);

        public abstract AABB GetBoundingBox();
        public abstract bool Intersects (AABB box);
        
        public abstract float GetMinExtreme (int axis);
        public abstract float GetMaxExtreme (int axis);

        public static AABB GetBoundingBox(List<Primitive> primitives)
        {
            Vector min = new Vector(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
            Vector max = new Vector(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);

            foreach (Primitive primitive in primitives)
            {
                AABB pbox = primitive.GetBoundingBox();

                min.X = System.Math.Min(min.X, pbox.Min.X);
                min.Y = System.Math.Min(min.Y, pbox.Min.Y);
                min.Z = System.Math.Min(min.Z, pbox.Min.Z);

                max.X = System.Math.Max(max.X, pbox.Max.X);
                max.Y = System.Math.Max(max.Y, pbox.Max.Y);
                max.Z = System.Math.Max(max.Z, pbox.Max.Z);
            }

            return new AABB(min, max);
        }
        
    }
}
