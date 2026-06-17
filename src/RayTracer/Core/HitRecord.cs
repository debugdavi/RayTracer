using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class HitRecord
    {
        public double T { get; set; }
        public Vector3 Point { get; set; }
        public Vector3 Normal { get; set; }
        public Material Material { get; set; }
    }
}
