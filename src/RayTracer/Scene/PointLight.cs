using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class PointLight : Light
    {
        public Vector3 Position { get; set; }
        public PointLight(Vector3 position, Vector3 intensity) : base(intensity)
        {
            Position = position;
        }
        public override Vector3 GetDirection(Vector3 point)
        {
            return (Position - point).Normalize();
        }
        public override double GetDistance(Vector3 point)
        {
            Vector3 diff = Position - point;
            return Math.Sqrt(diff.LengthSquared());
        }
        public override Vector3 GetEffectiveIntensity(Vector3 point)
        {
            return Intensity; 
        }
    }
}
