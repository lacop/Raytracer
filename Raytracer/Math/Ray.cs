using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    class Ray
    {
        public Vector Origin;
        public Vector Direction;

        public Ray(Vector origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }
}
