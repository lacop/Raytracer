using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    abstract class Primitive
    {
        public Material Material;

        public abstract bool Intersects(Ray r, out float t);
        public abstract Vector Normal(Vector p);
    }
}
