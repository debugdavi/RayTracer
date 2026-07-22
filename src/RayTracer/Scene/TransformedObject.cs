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
                Material = localRecord.Material,
                U = localRecord.U,
                V = localRecord.V
            };

            return true;
        }

        public AABB GetBoundingBox()
        {
            // Transformar os 8 cantos da AABB do objeto interno
            AABB inner = Object.GetBoundingBox();
            Vector3 min = inner.Min;
            Vector3 max = inner.Max;

            // Gerar todos os 8 cantos
            Vector3[] corners = new Vector3[8]
            {
                new Vector3(min.X, min.Y, min.Z),
                new Vector3(max.X, min.Y, min.Z),
                new Vector3(min.X, max.Y, min.Z),
                new Vector3(max.X, max.Y, min.Z),
                new Vector3(min.X, min.Y, max.Z),
                new Vector3(max.X, min.Y, max.Z),
                new Vector3(min.X, max.Y, max.Z),
                new Vector3(max.X, max.Y, max.Z),
            };

            // Transformar cada canto e encontrar o novo min/max
            Vector3 newMin = TransformMatrix.TransformPoint(corners[0]);
            Vector3 newMax = newMin;

            for (int i = 1; i < 8; i++)
            {
                Vector3 p = TransformMatrix.TransformPoint(corners[i]);
                newMin = new Vector3(
                    Math.Min(newMin.X, p.X),
                    Math.Min(newMin.Y, p.Y),
                    Math.Min(newMin.Z, p.Z)
                );
                newMax = new Vector3(
                    Math.Max(newMax.X, p.X),
                    Math.Max(newMax.Y, p.Y),
                    Math.Max(newMax.Z, p.Z)
                );
            }

            return new AABB(newMin, newMax);
        }
    }
}
