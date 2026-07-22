using System;

namespace RayTracer.Core
{
    /// <summary>
    /// Axis-Aligned Bounding Box (Caixa delimitadora alinhada aos eixos).
    /// Usado pela BVH para descartar rapidamente objetos que o raio não vai acertar.
    /// </summary>
    public class AABB
    {
        public Vector3 Min { get; set; }
        public Vector3 Max { get; set; }

        public AABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Testa se o raio intercepta esta caixa no intervalo [tMin, tMax].
        /// Usa o "Slab Method" — testa entrada/saída em cada eixo (X, Y, Z).
        /// Se os intervalos se sobrepõem nos 3 eixos, o raio passa pela caixa.
        /// </summary>
        public bool Hit(Ray ray, double tMin, double tMax)
        {
            // Eixo X
            double invD = 1.0 / ray.Direction.X;
            double t0 = (Min.X - ray.Origin.X) * invD;
            double t1 = (Max.X - ray.Origin.X) * invD;
            if (invD < 0) { double tmp = t0; t0 = t1; t1 = tmp; }
            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin) return false;

            // Eixo Y
            invD = 1.0 / ray.Direction.Y;
            t0 = (Min.Y - ray.Origin.Y) * invD;
            t1 = (Max.Y - ray.Origin.Y) * invD;
            if (invD < 0) { double tmp = t0; t0 = t1; t1 = tmp; }
            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin) return false;

            // Eixo Z
            invD = 1.0 / ray.Direction.Z;
            t0 = (Min.Z - ray.Origin.Z) * invD;
            t1 = (Max.Z - ray.Origin.Z) * invD;
            if (invD < 0) { double tmp = t0; t0 = t1; t1 = tmp; }
            tMin = t0 > tMin ? t0 : tMin;
            tMax = t1 < tMax ? t1 : tMax;
            if (tMax <= tMin) return false;

            return true;
        }

        /// <summary>
        /// Retorna a menor AABB que contém ambas as caixas.
        /// Usado para construir a hierarquia (pai = caixa que engloba os filhos).
        /// </summary>
        public static AABB Surrounding(AABB a, AABB b)
        {
            Vector3 min = new Vector3(
                Math.Min(a.Min.X, b.Min.X),
                Math.Min(a.Min.Y, b.Min.Y),
                Math.Min(a.Min.Z, b.Min.Z)
            );
            Vector3 max = new Vector3(
                Math.Max(a.Max.X, b.Max.X),
                Math.Max(a.Max.Y, b.Max.Y),
                Math.Max(a.Max.Z, b.Max.Z)
            );
            return new AABB(min, max);
        }

        /// <summary>
        /// Retorna o centro da caixa (usado para ordenar objetos na construção da BVH).
        /// </summary>
        public Vector3 Center()
        {
            return new Vector3(
                (Min.X + Max.X) * 0.5,
                (Min.Y + Max.Y) * 0.5,
                (Min.Z + Max.Z) * 0.5
            );
        }
    }
}
