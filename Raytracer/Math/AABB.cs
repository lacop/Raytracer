﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    class AABB
    {
        public Vector Min;
        public Vector Max;

        // width/height/depth from min to max
        public Vector Size
        {
            get
            {
                return new Vector((Max.X - Min.X), (Max.Y - Min.Y), (Max.Z - Min.Z));
            }
        }

        public Vector Center
        {
            get
            {
                return (Min+Size);
            }
        }

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

        public bool Intersects (AABB box)
        {
            return ((box.Max.X >= Min.X) && (box.Min.X <= Max.X) &&
                    (box.Max.Y >= Min.Y) && (box.Min.Y <= Max.Y) &&
                    (box.Min.Z >= Min.Z) && (box.Min.Z <= Max.Z));
        }

        public AABB[] Split (int axis, float pos)
        {
            AABB a = new AABB(new Vector(Min), new Vector(Max));
            AABB b = new AABB(new Vector(Min), new Vector(Max));

            a.Max[axis] = pos;
            b.Min[axis] = pos;

            return new []{a, b};

        }

        public float this[int i]
        {
            get
            {
                return Max[i] - Min[i];
            }
        }

        public float GetSurfaceArea ()
        {
            return 2*(this[0]*this[1] + this[1]*this[2] + this[2]*this[0]);
        }

        public override string ToString ()
        {
            return string.Format("[{0}-|+{1}]", Min, Max);
        }
    }
}
