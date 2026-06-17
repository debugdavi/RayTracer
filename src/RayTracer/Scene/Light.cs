using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public abstract class Light
    {
        public Vector3 Intensity { get; set; }

        protected Light(Vector3 intensity)
        {
            Intensity = intensity;
        }

        public abstract Vector3 GetDirection(Vector3 point);

        public abstract double GetDistance(Vector3 point);

        public abstract Vector3 GetEffectiveIntensity(Vector3 point);
    }
}
