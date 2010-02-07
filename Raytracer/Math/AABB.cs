using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    class AABB
    {
        public Vector Min;
        public Vector Max;

        public AABB(Vector min, Vector max)
        {
            Min = min;
            Max = max;
        }

        public bool Intersects (Ray r)
        {
            return Intersects(r, 0, float.PositiveInfinity);
        }

        public bool Intersects (Ray r, float t0, float t1)
        {
            float txmin, txmax;
            float tymin, tymax;

            Vector rdir = r.Direction.Inverse(); // cache 1/dir for speedup

            if (rdir.X >= 0)
            {
                txmin = (Min.X - r.Origin.X)*rdir.X;
                txmax = (Max.X - r.Origin.X)*rdir.X;
            }
            else
            {
                txmin = (Max.X - r.Origin.X)*rdir.X;
                txmax = (Min.X - r.Origin.X)*rdir.X;
            }

            if (rdir.Y >= 0)
            {
                tymin = (Min.Y - r.Origin.Y) * rdir.Y;
                tymax = (Max.Y - r.Origin.Y) * rdir.Y;
            }
            else
            {
                tymin = (Max.Y - r.Origin.Y) * rdir.Y;
                tymax = (Min.Y - r.Origin.Y) * rdir.Y;
            }

            if ((txmin > tymax) || (tymin > txmax))
                return false;

            if (tymin > txmin)
                txmin = tymin;
            if (tymax < txmax)
                txmax = tymax;

            float tzmin, tzmax;

            if (rdir.Z >= 0)
            {
                tzmin = (Min.Z - r.Origin.Z) * rdir.Z;
                tzmax = (Max.Z - r.Origin.Z) * rdir.Z;
            }
            else
            {
                tzmin = (Max.Z - r.Origin.Z) * rdir.Z;
                tzmax = (Min.Z - r.Origin.Z) * rdir.Z;
            }

            if ((txmin > tzmax) || (tzmin > txmax))
                return false;

            if (tzmin > txmin)
                txmin = tzmin;
            if (tzmax < txmax)
                txmax = tzmax;

            return ((txmin < t1) && (txmax > t0));

        }
    }
}
