using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    class Light
    {
        public Vector Position;

        public Light(Vector position)
        {
            Position = position;
        }
    }
}
