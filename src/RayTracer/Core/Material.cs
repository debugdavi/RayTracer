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

        // Reflexão e refração
        public double Reflectivity { get; set; }      // 0.0 = fosco, 1.0 = espelho perfeito
        public double Transparency { get; set; }       // 0.0 = opaco, 1.0 = totalmente transparente
        public double RefractiveIndex { get; set; }    // 1.0 = ar, 1.33 = água, 1.5 = vidro, 2.42 = diamante

        // Construtor com cor sólida (sem textura)
        public Material(Vector3 color, double ka, double kd, double ks, double shininess)
        {
            Color = color;
            Ka = ka;
            Kd = kd;
            Ks = ks;
            Shininess = shininess;
            DiffuseTexture = null;
            Reflectivity = 0.0;
            Transparency = 0.0;
            RefractiveIndex = 1.0;
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
            Reflectivity = 0.0;
            Transparency = 0.0;
            RefractiveIndex = 1.0;
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
