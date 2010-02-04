﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

using Raytracer.Math;
using Raytracer.Primitives;

namespace Raytracer
{
    class Program
    {
        private static Primitive[] primitives;
        private static Light[] lights;

        private static long stats_primary = 0;
        private static long stats_total = 0;
        private static long stats_maxdepth = 0;
        private static long stats_miss = 0;

        // Render setup
        const int w = 800;
        const int h = 600;

        const float x1 = -4;
        const float x2 = 4;
        const float y1 = 3;
        const float y2 = -3;

        const int ss = 1; // Supersampling level

        static float dx, dy;
        static float ss_step, ss_lim;

        static void Main(string[] args)
        {
            /*
            Sphere s = new Sphere(new Vector(0, 0, 10), 5);
            Ray r = new Ray(new Vector(0, 0, 0), new Vector(1, 0, 2));

            Console.WriteLine(s.Intersects(r));
            Console.ReadKey();
            */

            // Scene setup
            primitives = new Primitive[]
                         {
                             /*
                             new Plane(new Vector(0, 0, 1), 3, new Material(new Vector(0.8f, 0.8f, 0.8f), 0.8f, 0.0f)),
                             new Plane(new Vector(0, 1, 0), -2, new Material(new Vector(0.8f, 0.8f, 0.8f), 0.8f, 0.0f)),
                             new Plane(new Vector(0, -1, 0), -2, new Material(new Vector(0.8f, 0.8f, 0.8f), 0.8f, 0.0f)),
                             new Plane(new Vector(1, 0, 0), -3f, new Material(new Vector(0, 0, 0.8f), 0.8f, 0.0f)),
                             new Plane(new Vector(-1, 0, 0), -3f, new Material(new Vector(0.8f, 0, 0), 0.8f, 0.0f)),
                             */

                             //new Sphere(new Vector(0, 0, 200), 100, new Material(new Vector(0, 0.0f, 0), 0.0f, 1.0f)),
                             //new Sphere(new Vector(100, 100, -300), 100, new Material(new Vector(1.0f, 1.0f, 0), 1.0f, 0.0f)),
                             
                             //new Sphere(new Vector(-50, 0, 100), 50, new Material(new Vector(1, 0, 0), 0.4f, 0.0f)),
                             //new Sphere(new Vector(-200, 50, 150), 50, new Material(new Vector(1, 1, 0), 0.5f, 0.0f)),
                             //new Sphere(new Vector(-100, 25, 180), 60, new Material(new Vector(0, 0, 0), 0.0f, 1.0f)),
                             //new Sphere(new Vector(100, 150, 75), 20, new Material(new Vector(0, 0, 1), 0.8f, 0.0f)),
                             
                             // reflection
                             //new Sphere(new Vector(0, -150, 200), 100, new Material(new Vector(0, 1.0f, 0), 1.0f, 0.0f)),
                             //new Sphere(new Vector(200, -75, 400), 200, new Material(new Vector(1.0f, 0.0f, 0), 0.5f, 1.0f)),
                             
                             //bikker v3 scene
                             new Plane(new Vector(0, 1, 0), 4.4f, new Material(new Vector(0.4f, 0.3f, 0.3f), 1.0f, 0.0f)), //ground
                             new Plane(new Vector(0.4f, 0, -1), 12, new Material(new Vector(0.5f, 0.3f, 0.5f), 0.6f, 0.3f)), //back
                             new Sphere(new Vector(2, 0.8f, 3), 2.5f, new Material(new Vector(0.7f, 0.7f, 1.0f), 0.6f, 0.2f)), //big sphere
                             new Sphere(new Vector(-5.5f, -0.5f, 7), 2, new Material(new Vector(0.7f, 0.7f, 1.0f), 0.7f, 0.6f)), //small sphere
                             new Sphere(new Vector(-1.5f, -2.5f, 1.5f   ), 1.5f, new Material(new Vector(1.0f, 0.4f, 0.4f), 0.7f, 0.0f)), //extra sphere
                             
                             //new Sphere(new Vector(-5.5f, -0.5f, 7), 2f, new Material(new Vector(0.7f, 0.7f, 1.0f), 0.8f, 0.0f)),
                             //new Sphere(new Vector(2, 0.8f, 3), 2.5f, new Material(new Vector(0.7f, 0.7f, 1.0f), 0.8f, 0.0f)),
                             //new Sphere(new Vector(0, 5, 5), 1.5f, new Material(new Vector(1, 1, 1), 1.0f, 0.0f)),

                             //bikker v2 scene
                             //new Plane(new Vector(0, 1, 0), 4.4f, new Material(new Vector(0.4f, 0.3f, 0.3f), 1.0f, 0.0f)),
                             //new Sphere(new Vector(1, -0.8f, 3), 2.5f, new Material(new Vector(0.7f, 0.7f, 0.7f), 1.0f, 0.0f)),
                             //new Sphere(new Vector(-5.5f, -0.5f, 7), 2, new Material(new Vector(0.7f, 0.7f, 1.0f), 1.0f, 0.0f)),
                             
                         };
            
            lights = new[]
                     {
                         //new Light(new Vector(0, 0, 0)), 
                         /*new Light(new Vector(0, -500, 200)),
                         new Light(new Vector(100, -100, 300)),
                         new Light(new Vector(200, 100, 400)),*/
                         //new Light(new Vector(0, 0, 0)),
                         //new Light(new Vector(300, -100, 100)),
                         //new Light(new Vector(500, 300, -20)),
                         //new Light(new Vector(-200, -200, 50)),

                         //bikker v3 scene
                         new Light(new Vector(0, 5, 5)), 
                         new Light(new Vector(-3, 5, 1)), 

                         //bikker v2 scene
                         //new Light(new Vector(0, 5, 5)), 
                         //new Light(new Vector(2, 5, 1)), 

                     };

            dx = (x2 - x1)/w;
            dy = (y2 - y1)/h;

            Console.Write("Rendering : ");
            int cl = Console.CursorLeft;
            int ct = Console.CursorTop;
            int cw = Console.WindowWidth - cl - 4;

            ss_step = 1.0f/ss;
            ss_lim = (ss - 1)/2.0f * ss_step;

            Color[] pixels = new Color[w*h];

            Stopwatch sw = new Stopwatch();
            sw.Start();
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Vector color = TracePixel(x, y);

                    pixels[y*w+x] = Color.FromArgb((int) (255*color.X), (int) (255*color.Y), (int) (255*color.Z));
                }
                
                Console.SetCursorPosition(cl, ct);
                Console.Write(new string('#', (cw*y)/h));
                Console.SetCursorPosition(cl+cw, ct);
                Console.Write(100*(y+1)/h + "%");
            }
            sw.Stop();

            // TODO: Lockbitmap direct write
            Bitmap b = new Bitmap(w, h);
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    b.SetPixel(x, y, pixels[y*w+x]);
                }
            }
            b.Save("../../../out.png", ImageFormat.Png);

            //*
            Console.WriteLine();
            Console.WriteLine("Render time: {0}ms", sw.ElapsedMilliseconds);
            Console.WriteLine();
            Console.WriteLine("Total rays : {0}", stats_total);
            Console.WriteLine("Primary    : {0}\t\t{1} %", stats_primary, 100*stats_primary/stats_total);
            Console.WriteLine("Missed     : {0}\t\t{1} %", stats_miss, 100*stats_miss/stats_total);
            Console.WriteLine("Max depth  : {0}", stats_maxdepth);
            
            Console.WriteLine();
            Console.WriteLine("Press any key to exit ...");
            Console.ReadKey();/**/



        }

        public static Vector TracePixel(int x, int y)
        {
            Vector color = new Vector(0, 0, 0);
            float i; int ii;
            for (i = -ss_lim, ii = 0; ii < ss; i += ss_step, ii++)
            {
                float j; int jj;
                for (j = -ss_lim, jj = 0; jj < ss; j += ss_step, jj++)
                {

                    // Orthogonal projection
                    //Vector pt = new Vector(-w/2 + x, -h/2 + y, -50);
                    //Ray r = new Ray(pt, new Vector(0,0, 1));

                    // Perspective
                    //Vector pt = new Vector(-w/2 + x + i, -h/2 + y + j, 0);
                    //Vector pt = new Vector(-w / 2 + x, -h / 2 + y, 0);
                            
                    /*
                            Vector pt = new Vector(x1+x*dx, y1+y*dy, -5);/*/
                    Vector pt = new Vector(x1+x*dx+dx*i, y1+y*dy+dy*j, -5); //supersample /**/
                            
                    Vector o = new Vector(0, 0, -10);
                    Ray r = new Ray(o, (pt - o).Normalize());

                    color += Trace(r, 0);
                }
            }
            color *= 1.0f/(ss*ss);
            return color;
        }

        public static Vector Trace (Ray r, int depth)
        {
            stats_total++;
            if (depth == 0) 
                stats_primary++;
            if (depth > stats_maxdepth)
                stats_maxdepth = depth;

            if (depth > 10)
                return new Vector(0, 0, 0); // TODO: renderstate, maxdepth
            
            float closest = float.PositiveInfinity;
            Primitive prim = null;

            foreach (var p in primitives)
            {
                float t;
                if (p.Intersects(r, out t) && t > 0.0f)
                {
                    if (t < closest)
                    {
                        closest = t;
                        prim = p;
                    }
                }
            }

            if (prim == null)
            {
                stats_miss++;
                return new Vector(0, 0, 0); //TODO: background / skybox
            }

            Vector ip = r.Origin + r.Direction*closest;

            //TODO: custom class for colors with clamping, rgb/hsv/... models, etc
            Vector color = new Vector(0, 0, 0);
            
            // Lightning
            foreach (var light in lights)
            {
                Vector dir = light.Position - ip;
                float dist = dir.Length();
                dir = dir.Normalize();

                Ray lr = new Ray(ip+dir*float.Epsilon, dir);
                
                float shade = 1.0f;
                foreach (var p in primitives)
                {
                    float t;
                    if (p != prim && p.Intersects(lr, out t) && t < dist)
                    {
                        shade = 0.0f; //TODO: soft shadows
                        break;
                    }
                }

                if (shade > 0)
                {
                    Vector n = prim.Normal(ip);

                    float d = n.Dot(dir);
                    if (d > 0)
                    {
                        color += prim.Material.Color*prim.Material.Diffuse*d*shade; //flipped-params operators
                    }
                }
            }
            //color = prim.Material.Color*prim.Material.Diffuse; //no lightning mode

            // Reflection
            if (prim.Material.Reflection > 0 && depth <= 10) //TODO: render state, limits, ...
            {
                Vector n = prim.Normal(ip);
                Vector rr = (r.Direction - n*2.0f*n.Dot(r.Direction)).Normalize();
                
                color = color*(1.0f - prim.Material.Reflection) + Trace(new Ray(ip + rr*10e-4f, rr), depth + 1)*prim.Material.Reflection; //TODO: fix epsilon
                //color = Trace(new Ray(ip + rr*float.Epsilon, rr), depth + 1);
                //color = Trace(new Ray(ip + rr*0.01f,rr), depth + 1);
            }

            color.X = color.X > 1 ? 1 : color.X;
            color.Y = color.Y > 1 ? 1 : color.Y;
            color.Z = color.Z > 1 ? 1 : color.Z;

            return color;
        }

    }
}
