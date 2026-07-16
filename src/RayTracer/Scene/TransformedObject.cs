using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class TransformedObject : IHittable
    {
        public IHittable Object { get; set; }
        public Matrix4x4 TransformMatrix { get; set; }
        public Matrix4x4 InverseMatrix { get; set; }

        public TransformedObject(IHittable obj, Matrix4x4 transform)
        {
            Object = obj;
            TransformMatrix = transform;
            InverseMatrix = transform.Inverse();
        }

        public bool Hit(Ray ray, out HitRecord record)
        {
            record = null;

            // ===== 1. Transformar raio: mundo → espaço do objeto =====
            // Origem é um ponto (w=1) → TransformPoint
            // Direção é um vetor (w=0) → TransformDirection
            Vector3 localOrigin = InverseMatrix.TransformPoint(ray.Origin);
            Vector3 localDirection = InverseMatrix.TransformDirection(ray.Direction);

            Ray localRay = new Ray(localOrigin, localDirection);

            // ===== 2. Testar interseção no espaço do objeto =====
            if (!Object.Hit(localRay, out HitRecord localRecord))
                return false;

            // ===== 3. Transformar resultado: espaço do objeto → mundo =====
            // Ponto é transformado com M (TransformPoint)
            Vector3 worldPoint = TransformMatrix.TransformPoint(localRecord.Point);

            // Normal é transformada com (M⁻¹)ᵀ (TransformNormal na inversa)
            Vector3 worldNormal = InverseMatrix.TransformNormal(localRecord.Normal);

            record = new HitRecord
            {
                T = localRecord.T,
                Point = worldPoint,
                Normal = worldNormal,
                Material = localRecord.Material
            };

            return true;
        }
    }
}
