using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class Material
    {
        public Vector3 Color { get; set; }
        public double Ka { get; set; }
        public double Kd { get; set; }
        public double Ks { get; set; }
        public double Shininess { get; set; }

        public Material(Vector3 color, double ka, double kd, double ks, double shininess)
        {
            Color = color;
            Ka = ka;
            Kd = kd;
            Ks = ks;
            Shininess = shininess;
        }
    }
}
