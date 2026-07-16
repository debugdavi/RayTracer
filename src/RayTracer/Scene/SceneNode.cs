using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class SceneNode
    {
        public Matrix4x4 LocalTransform { get; set; }
        public IHittable Object { get; set; }           // geometria (pode ser null se é um nó organizador)
        public List<SceneNode> Children { get; set; }

        public SceneNode(Matrix4x4 localTransform = null, IHittable obj = null)
        {
            LocalTransform = localTransform ?? Matrix4x4.Identity();
            Object = obj;
            Children = new List<SceneNode>();
        }

        public void AddChild(SceneNode child)
        {
            Children.Add(child);
        }

        public List<IHittable> Flatten(Matrix4x4 parentTransform = null)
        {
            // Transformação acumulada = pai × local
            Matrix4x4 worldTransform = (parentTransform != null)
                ? parentTransform * LocalTransform
                : LocalTransform;

            List<IHittable> result = new List<IHittable>();

            // Se este nó tem geometria, cria um TransformedObject
            if (Object != null)
            {
                result.Add(new TransformedObject(Object, worldTransform));
            }

            // Recursivamente processa os filhos
            foreach (var child in Children)
            {
                result.AddRange(child.Flatten(worldTransform));
            }

            return result;
        }
    }
}
