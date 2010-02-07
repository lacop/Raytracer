using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;
using Raytracer.Primitives;

namespace Raytracer.Acceleration
{
    class NullAccelerator : AccelerationStructure
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private List<Primitive> primitives;

        public override void Build(List<Primitive> primitives)
        {
            log.InfoFormat("Building nullaccelerator structure for {0} primitives ...", primitives.Count);
            this.primitives = primitives;
        }

        public override bool FindIntersection(Ray r, out float closest, out Primitive closestPrimitive)
        {
            float t = 0;

            closest = float.PositiveInfinity;
            closestPrimitive = null;

            foreach (Primitive primitive in primitives)
            {
                if (primitive.Intersects(r, out t) && t > 0)
                {
                    if (t < closest)
                    {
                        closest = t;
                        closestPrimitive = primitive;
                    }
                }
            }

            return closestPrimitive != null;
        }
    }   
}
