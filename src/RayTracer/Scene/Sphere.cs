using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class Sphere : IHittable
    {
        public Vector3 Center { get; set; }
        public double Radius { get; set; }
        public Material Material { get; set; }
        public Sphere(Vector3 center, double radius, Material material)
        {
            Center = center;
            Radius = radius;
            Material = material;
        }
        public bool Hit(Ray ray, out HitRecord record)
        {
            record = null;

            Vector3 oc = ray.Origin - Center;
            double a = Vector3.DotProduct(ray.Direction, ray.Direction);
            double b = 2.0 * Vector3.DotProduct(oc, ray.Direction);
            double c = Vector3.DotProduct(oc, oc) - Radius * Radius;

            double discriminant = b * b - 4 * a * c;
            
            if (discriminant < 0)
                return false;

            double sqrtD = Math.Sqrt(discriminant);
            double t1 = (-b - sqrtD) / (2.0 * a);
            double t2 = (-b + sqrtD) / (2.0 * a);

            double t;
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
                return false;
            }

            Vector3 point = ray.At(t);
            Vector3 normal = (point - Center).Normalize();

            record = new HitRecord
            {
                T = t,
                Point = point,
                Normal = normal,
                Material = this.Material,
                // Coordenadas UV esféricas (latitude/longitude da normal)
                U = 0.5 + Math.Atan2(normal.Z, normal.X) / (2.0 * Math.PI),
                V = 0.5 + Math.Asin(Math.Clamp(normal.Y, -1.0, 1.0)) / Math.PI
            };

            return true;
        }
    }
}
