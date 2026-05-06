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

            if (discriminant < 0)
            {
                t = 0;
                return false;
            }

            t = (-b - Math.Sqrt(discriminant)) / (2.0 * a);
            return true;
        }
    }
}
