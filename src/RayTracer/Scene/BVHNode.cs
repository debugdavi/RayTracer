using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;

namespace RayTracer.Scene
{
    /// <summary>
    /// Nó da Bounding Volume Hierarchy (BVH).
    /// Organiza os objetos numa árvore binária de caixas delimitadoras.
    /// 
    /// Sem BVH: cada raio testa TODOS os objetos → O(n)
    /// Com BVH: cada raio só desce nos ramos cujas caixas o raio acerta → O(log n)
    /// 
    /// Construção:
    ///   1. Escolhe o eixo de maior extensão (X, Y ou Z)
    ///   2. Ordena os objetos pelo centro da AABB nesse eixo
    ///   3. Divide no meio: metade esquerda → filho Left, metade direita → filho Right
    ///   4. Repete recursivamente até ter 1 ou 2 objetos por folha
    /// </summary>
    public class BVHNode : IHittable
    {
        public AABB Box { get; set; }
        public IHittable Left { get; set; }
        public IHittable? Right { get; set; }

        public BVHNode(List<IHittable> objects)
        {
            if (objects.Count == 0)
                throw new ArgumentException("BVH precisa de pelo menos 1 objeto");

            if (objects.Count == 1)
            {
                // Folha: um único objeto
                Left = objects[0];
                Right = null;
                Box = Left.GetBoundingBox();
                return;
            }

            if (objects.Count == 2)
            {
                // Dois objetos: um em cada filho
                Left = objects[0];
                Right = objects[1];
                Box = AABB.Surrounding(Left.GetBoundingBox(), Right.GetBoundingBox());
                return;
            }

            // ===== Escolher o eixo de maior extensão =====
            AABB totalBox = objects[0].GetBoundingBox();
            for (int i = 1; i < objects.Count; i++)
                totalBox = AABB.Surrounding(totalBox, objects[i].GetBoundingBox());

            double extX = totalBox.Max.X - totalBox.Min.X;
            double extY = totalBox.Max.Y - totalBox.Min.Y;
            double extZ = totalBox.Max.Z - totalBox.Min.Z;

            // Eixo com maior extensão → melhor separação espacial
            int axis;
            if (extX >= extY && extX >= extZ) axis = 0;      // X
            else if (extY >= extX && extY >= extZ) axis = 1;  // Y
            else axis = 2;                                     // Z

            // ===== Ordenar pelo centro da AABB no eixo escolhido =====
            objects.Sort((a, b) =>
            {
                Vector3 ca = a.GetBoundingBox().Center();
                Vector3 cb = b.GetBoundingBox().Center();
                double va = axis == 0 ? ca.X : axis == 1 ? ca.Y : ca.Z;
                double vb = axis == 0 ? cb.X : axis == 1 ? cb.Y : cb.Z;
                return va.CompareTo(vb);
            });

            // ===== Dividir no meio =====
            int mid = objects.Count / 2;
            Left = new BVHNode(objects.GetRange(0, mid));
            Right = new BVHNode(objects.GetRange(mid, objects.Count - mid));

            Box = AABB.Surrounding(Left.GetBoundingBox(), Right.GetBoundingBox());
        }

        public bool Hit(Ray ray, out HitRecord record)
        {
            record = null;

            // ===== PODA: se o raio não acerta a caixa, descarta toda a subárvore =====
            if (!Box.Hit(ray, 0.001, double.MaxValue))
                return false;

            // Testar filho esquerdo
            bool hitLeft = Left.Hit(ray, out HitRecord leftRecord);

            // Testar filho direito (se existe)
            bool hitRight = false;
            HitRecord rightRecord = null;
            if (Right != null)
                hitRight = Right.Hit(ray, out rightRecord);

            // Retornar o mais próximo
            if (hitLeft && hitRight)
            {
                record = leftRecord.T < rightRecord.T ? leftRecord : rightRecord;
                return true;
            }
            else if (hitLeft)
            {
                record = leftRecord;
                return true;
            }
            else if (hitRight)
            {
                record = rightRecord;
                return true;
            }

            return false;
        }

        public AABB GetBoundingBox() => Box;
    }
}
