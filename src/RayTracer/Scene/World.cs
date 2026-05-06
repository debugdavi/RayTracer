using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class World
    {
        private List<Sphere> _spheres = new List<Sphere>();

        public void Add(Sphere sphere) => _spheres.Add(sphere);

        public Vector3 Trace(Ray ray)
        {
            double closestT = double.MaxValue;
            Sphere hitObject = null;

            foreach (var sphere in _spheres)
            {
                if (sphere.Hit(ray, out double t))
                {
                    if (t > 0.001 && t < closestT)
                    {
                        closestT = t;
                        hitObject = sphere;
                    }
                }
            }

            if (hitObject != null)
            {
                Vector3 point = ray.At(closestT);
                Vector3 normal = (point - hitObject.Center).Normalize();
                return 0.5 * (normal + new Vector3(1, 1, 1));
            }

            // Fundo (Céu)
            double tBg = 0.5 * (ray.Direction.Normalize().Y + 1.0);
            return (1.0 - tBg) * new Vector3(1, 1, 1) + tBg * new Vector3(0.5, 0.7, 1.0);
        }
    }
}
