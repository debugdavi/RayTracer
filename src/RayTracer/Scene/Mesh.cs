using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class Mesh : IHittable
    {
        public List<Triangle> Triangles { get; set; }
        public Material Material { get; set; }

        public Mesh(Material material)
        {
            Triangles = new List<Triangle>();
            Material = material;
        }

        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }

        public bool Hit(Ray ray, out HitRecord record)
        {
            record = null;
            double closestT = double.MaxValue;
            HitRecord closestHit = null;

            foreach (var triangle in Triangles)
            {
                if (triangle.Hit(ray, out HitRecord tempRecord))
                {
                    if (tempRecord.T > 0.001 && tempRecord.T < closestT)
                    {
                        closestT = tempRecord.T;
                        closestHit = tempRecord;
                    }
                }
            }

            if (closestHit != null)
            {
                record = closestHit;
                return true;
            }

            return false;
        }
    }
}
