using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;
using Raytracer.Primitives;

namespace Raytracer.Acceleration
{
    abstract class AccelerationStructure
    {
        public abstract void Build(List<Primitive> primitives);

        public abstract bool FindIntersection(Ray r, out float t, out Primitive primitive);
    }
}
