﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    class Plane : Primitive
    {
        //TODO: protect with properties and autonormalize
        private Vector _norm;
        public Vector Norm
        {
            get { return _norm; }
            set { _norm = value.Normalize(); }
        }

        public float Dist;

        public Plane(Vector norm, float dist, Material material)
        {
            _norm = norm.Normalize();
            Dist = dist;
            Material = material;
        }

        public override bool Intersects(Ray r, out float t)
        {
            float d = _norm.Dot(r.Direction);
            if (d == 0)
            {
                t = float.PositiveInfinity;
                return false;
            }

            float dist = -(_norm.Dot(r.Origin) + Dist) / d;
            if (dist <= 0)
            {
                t = float.PositiveInfinity;
                return false;
            }

            t = dist;
            return true;

        }

        public override Vector Normal(Vector p)
        {
            return _norm;
        }

        public override AABB GetBoundingBox()
        {
            //TODO: not working for axis-parallel planes
            return new AABB(new Vector(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity),
                            new Vector(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity));
        }

        public override bool Intersects (AABB box)
        {
            return box.Intersects(GetBoundingBox());
        }

        public override float GetMinExtreme (int axis)
        {
            throw new NotImplementedException();
        }

        public override float GetMaxExtreme (int axis)
        {
            throw new NotImplementedException();
        }
    }
}
