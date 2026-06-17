using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class SpotLight : Light  
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public double CutoffAngle { get; set; }
        public double FallOffExponent { get; set; }
        public SpotLight(Vector3 position, Vector3 direction, Vector3 intensity,
               double cutoffAngleDegrees, double falloffExponent = 1.0) : base(intensity)
        {
            Position = position;
            Direction = direction.Normalize();
            CutoffAngle = cutoffAngleDegrees * Math.PI / 180.0;
            FallOffExponent = falloffExponent;
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
            Vector3 L = (point - Position).Normalize(); 
            double cosAngle = Vector3.DotProduct(L, Direction);
            double cosCutoff = Math.Cos(CutoffAngle);

            if (cosAngle < cosCutoff)
                return new Vector3(0, 0, 0); 

            double spotFactor = Math.Pow(cosAngle, FallOffExponent);
            return spotFactor * Intensity;
        }
    }
}
