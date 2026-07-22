using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class Triangle : IHittable
    {
        public Vector3 V0 { get; set; }
        public Vector3 V1 { get; set; }
        public Vector3 V2 { get; set; }

        public Vector3 N0 { get; set; }
        public Vector3 N1 { get; set; }
        public Vector3 N2 { get; set; }

        public Vector3 UV0 { get; set; }  // coordenadas de textura do vértice 0 (X=u, Y=v)
        public Vector3 UV1 { get; set; }  // coordenadas de textura do vértice 1
        public Vector3 UV2 { get; set; }  // coordenadas de textura do vértice 2
        public bool HasUVs { get; set; }

        public Material Material { get; set; }
        public bool SmoothShading { get; set; }

        /// <summary>Flat shading, sem UVs.</summary>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2, Material material)
        {
            V0 = v0; V1 = v1; V2 = v2;
            Material = material;
            SmoothShading = false;
            HasUVs = false;
            UV0 = UV1 = UV2 = new Vector3(0, 0, 0);

            // Calcula a normal do plano do triângulo: (V1-V0) × (V2-V0)
            Vector3 faceNormal = Vector3.CrossProduct(v1 - v0, v2 - v0).Normalize();
            N0 = N1 = N2 = faceNormal;
        }

        /// <summary>Smooth shading, sem UVs.</summary>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2,
                        Vector3 n0, Vector3 n1, Vector3 n2,
                        Material material)
        {
            V0 = v0; V1 = v1; V2 = v2;
            N0 = n0; N1 = n1; N2 = n2;
            Material = material;
            SmoothShading = true;
            HasUVs = false;
            UV0 = UV1 = UV2 = new Vector3(0, 0, 0);
        }

        /// <summary>Smooth shading COM UVs (para texturas).</summary>
        public Triangle(Vector3 v0, Vector3 v1, Vector3 v2,
                        Vector3 n0, Vector3 n1, Vector3 n2,
                        Vector3 uv0, Vector3 uv1, Vector3 uv2,
                        Material material)
        {
            V0 = v0; V1 = v1; V2 = v2;
            N0 = n0; N1 = n1; N2 = n2;
            UV0 = uv0; UV1 = uv1; UV2 = uv2;
            Material = material;
            SmoothShading = true;
            HasUVs = true;
        }

        public bool Hit(Ray ray, out HitRecord record)
        {
            record = null;
            const double EPSILON = 0.0000001;

            // Passo 1: Calcular as arestas do triângulo
            Vector3 edge1 = V1 - V0;  // E1
            Vector3 edge2 = V2 - V0;  // E2

            // Passo 2: Calcular o determinante
            // h = D × E2  (cross product da direção do raio com edge2)
            Vector3 h = Vector3.CrossProduct(ray.Direction, edge2);
            double det = Vector3.DotProduct(edge1, h);

            // Se o determinante é próximo de zero, o raio é paralelo ao triângulo
            if (det > -EPSILON && det < EPSILON)
                return false;

            double invDet = 1.0 / det;

            // Passo 3: Calcular u (primeira coordenada baricêntrica)
            Vector3 s = ray.Origin - V0;  // T = O - V0
            double u = invDet * Vector3.DotProduct(s, h);

            // Se u < 0 ou u > 1, o ponto está fora do triângulo
            if (u < 0.0 || u > 1.0)
                return false;

            // Passo 4: Calcular v (segunda coordenada baricêntrica)
            Vector3 q = Vector3.CrossProduct(s, edge1);
            double v = invDet * Vector3.DotProduct(ray.Direction, q);

            // Se v < 0 ou u+v > 1, o ponto está fora do triângulo
            if (v < 0.0 || u + v > 1.0)
                return false;

            // Passo 5: Calcular t (distância ao longo do raio)
            double t = invDet * Vector3.DotProduct(edge2, q);

            // t > EPSILON garante que a interseção está na frente do raio
            if (t <= EPSILON)
                return false;

            // ===== Interseção válida! =====

            Vector3 point = ray.At(t);
            double w = 1.0 - u - v;

            // Passo 6: Calcular a normal
            Vector3 normal;
            if (SmoothShading)
            {
                // Interpolação baricêntrica das normais dos vértices
                normal = (w * N0 + u * N1 + v * N2).Normalize();
            }
            else
            {
                // Flat shading — usa a normal do plano
                normal = N0; // todas são iguais no flat shading
            }

            // Passo 7: Calcular UVs interpoladas
            double hitU = 0, hitV = 0;
            if (HasUVs)
            {
                hitU = w * UV0.X + u * UV1.X + v * UV2.X;
                hitV = w * UV0.Y + u * UV1.Y + v * UV2.Y;
            }

            record = new HitRecord
            {
                T = t,
                Point = point,
                Normal = normal,
                Material = this.Material,
                U = hitU,
                V = hitV
            };

            return true;
        }
    }
}
