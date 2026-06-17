using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class DirectionalLight : Light
    {
        public Vector3 Direction { get; set; }
        public DirectionalLight(Vector3 direction, Vector3 intensity) : base(intensity)
        {
            Direction = direction.Normalize();
        }
        public override Vector3 GetDirection(Vector3 point)
        {
            return (-Direction).Normalize();
        }
        public override double GetDistance(Vector3 point)
        {
            return double.MaxValue;
        }
        public override Vector3 GetEffectiveIntensity(Vector3 point)
        {
            return Intensity;
        }
    }
}
