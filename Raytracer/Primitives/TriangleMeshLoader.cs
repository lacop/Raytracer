using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Raytracer.Math;

namespace Raytracer.Primitives
{
    class TriangleMeshLoader
    {
        private TriangleMeshLoader()
        {
            
        }

        public static List<Triangle> LoadOBJ (string path, Material material)
        {
            List<Triangle> triangles = new List<Triangle>();
            List<Vector> vertices = new List<Vector>();    

            foreach (string line in File.ReadAllLines(path))
            {
                if (line.StartsWith("#")) // Skip comments
                    continue;

                string[] parts = line.Split(new[] {' '});
                switch (parts[0])
                {
                    case "v":
                        vertices.Add(new Vector(float.Parse(parts[1], CultureInfo.InvariantCulture),
                                                float.Parse(parts[2], CultureInfo.InvariantCulture),
                                                float.Parse(parts[3], CultureInfo.InvariantCulture)));
                        break;
                    
                    case "f":
                        if (parts.Length != 4)
                            throw new NotImplementedException("Only triangle faces are supported for now");
                        triangles.Add(new Triangle(vertices[int.Parse(parts[1])-1],
                                                   vertices[int.Parse(parts[2])-1],
                                                   vertices[int.Parse(parts[3])-1],
                                                   material));
                        break;
                }
                
            }

            return triangles;
        }

    }
}
