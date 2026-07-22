using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core.Interfaces
{
    public interface IHittable
    {
        bool Hit(Ray ray, out HitRecord hitRecord);
        AABB GetBoundingBox();
    }
}
