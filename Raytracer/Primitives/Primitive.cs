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
    }
}
