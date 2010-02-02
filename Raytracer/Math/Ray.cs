using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Raytracer.Math
{
    class Ray
    {
        public Vector Origin;
        
        private Vector _direction;
        public Vector Direction
        {
            get { return _direction; }
            set { _direction = value.Normalize(); }
        }

        public Ray(Vector origin, Vector direction)
        {
            Origin = origin;
            Direction = direction;
        }
    }
}
