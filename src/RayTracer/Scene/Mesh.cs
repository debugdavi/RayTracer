using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class Mesh : IHittable
    {
        public List<Triangle> Triangles { get; set; }
        public Material Material { get; set; }

        // BVH interna: acelera a busca de O(n) para O(log n) nos triângulos
        private BVHNode? _bvh;

        public Mesh(Material material)
        {
            Triangles = new List<Triangle>();
            Material = material;
            _bvh = null;
        }

        public void AddTriangle(Triangle triangle)
        {
            Triangles.Add(triangle);
        }

        /// <summary>
        /// Constrói a BVH interna a partir dos triângulos.
        /// Deve ser chamado APÓS todos os triângulos terem sido adicionados.
        /// </summary>
        public void BuildBVH()
        {
            if (Triangles.Count > 0)
            {
                List<IHittable> objs = new List<IHittable>(Triangles);
                _bvh = new BVHNode(objs);
            }
        }

        public bool Hit(Ray ray, out HitRecord record)
        {
            // Se a BVH foi construída, usa ela (O(log n))
            if (_bvh != null)
                return _bvh.Hit(ray, out record);

            // Fallback: busca linear (O(n)) — caso BuildBVH não tenha sido chamado
            record = null;
            double closestT = double.MaxValue;
            HitRecord closestHit = null;

            foreach (var triangle in Triangles)
            {
                if (triangle.Hit(ray, out HitRecord tempRecord))
                {
                    if (tempRecord.T > 0.001 && tempRecord.T < closestT)
                    {
                        closestT = tempRecord.T;
                        closestHit = tempRecord;
                    }
                }
            }

            if (closestHit != null)
            {
                record = closestHit;
                return true;
            }

            return false;
        }

        public AABB GetBoundingBox()
        {
            if (_bvh != null)
                return _bvh.GetBoundingBox();

            // Fallback: calcular da lista de triângulos
            if (Triangles.Count == 0)
                return new AABB(new Vector3(0, 0, 0), new Vector3(0, 0, 0));

            AABB box = Triangles[0].GetBoundingBox();
            for (int i = 1; i < Triangles.Count; i++)
                box = AABB.Surrounding(box, Triangles[i].GetBoundingBox());
            return box;
        }
    }
}
