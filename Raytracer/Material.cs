using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer
{
    class Material
    {
        public Vector Color;
        
        public float Diffuse;
        public float Reflection;

        public Material(Vector color, float diffuse, float reflection)
        {
            Color = color;
            Diffuse = diffuse;
            Reflection = reflection;
        }
    }
}
