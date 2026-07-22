using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class Material
    {
        public Vector3 Color { get; set; }
        public double Ka { get; set; }
        public double Kd { get; set; }
        public double Ks { get; set; }
        public double Shininess { get; set; }
        public Texture? DiffuseTexture { get; set; }

        // Construtor com cor sólida (sem textura)
        public Material(Vector3 color, double ka, double kd, double ks, double shininess)
        {
            Color = color;
            Ka = ka;
            Kd = kd;
            Ks = ks;
            Shininess = shininess;
            DiffuseTexture = null;
        }

        // Construtor com textura
        public Material(Texture texture, double ka, double kd, double ks, double shininess)
        {
            Color = new Vector3(1, 1, 1); // fallback branco
            Ka = ka;
            Kd = kd;
            Ks = ks;
            Shininess = shininess;
            DiffuseTexture = texture;
        }

        /// <summary>
        /// Retorna a cor no ponto (u, v):
        /// - Se tem textura → busca na imagem
        /// - Se não → usa a cor sólida
        /// </summary>
        public Vector3 GetColor(double u, double v)
        {
            if (DiffuseTexture != null)
                return DiffuseTexture.Sample(u, v);
            return Color;
        }
    }
}
