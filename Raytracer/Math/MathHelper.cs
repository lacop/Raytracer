using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    static class MathHelper
    {
        public static float Min (float a, float b, float c)
        {
            return System.Math.Min(a, System.Math.Min(b, c));
        }

        public static float Max(float a, float b, float c)
        {
            return System.Math.Max(a, System.Math.Max(b, c));
        }
    }
}
