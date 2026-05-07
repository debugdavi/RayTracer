using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class Sphere
    {
        public Vector3 Center { get; set; }
        public double Radius { get; set; }
        public Sphere(Vector3 center, double radius)
        {
            Center = center;
            Radius = radius;
        }
        public bool Hit(Ray ray, out double t)
        {
            Vector3 oc = ray.Origin - Center;

            double a = Vector3.DotProduct(ray.Direction, ray.Direction);
            double b = 2.0 * Vector3.DotProduct(oc, ray.Direction);
            double c = Vector3.DotProduct(oc, oc) - Radius * Radius;

            double discriminant = b * b - 4 * a * c;

            double sqrtD = Math.Sqrt(discriminant);
            double t1 = (-b - sqrtD) / (2.0 * a);
            double t2 = (-b + sqrtD) / (2.0 * a);

            if (t1 > 0.001 && t2 > 0.001)
            {
                t = Math.Min(t1, t2);
            }
            else if (t1 > 0.001)
            {
                t = t1;
            }
            else if (t2 > 0.001)
            {
                t = t2; 
            }
            else
            {
                t = 0;
                return false;
            }

            return true;
        }
    }
}
