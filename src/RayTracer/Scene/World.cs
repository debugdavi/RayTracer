using RayTracer.Core;
using RayTracer.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public class World
    {
        private List<IHittable> _objects = new List<IHittable>();
        private List<Light> _lights = new List<Light>();
        private Vector3 _ambientLight = new Vector3(1, 1, 1); // Intensidade da luz ambiente -> HARDCODED
        public void Add(IHittable obj) => _objects.Add(obj);
        public void AddLight(Light light) => _lights.Add(light);

        public Vector3 Trace(Ray ray)
        {
            double closestT = double.MaxValue;
            HitRecord closestHit = null;

            foreach (var obj in _objects)
            {
                if (obj.Hit(ray, out HitRecord record))
                {
                    if (record.T > 0.001 && record.T < closestT)
                    {
                        closestT = record.T;
                        closestHit = record;
                    }
                }
            }

            if (closestHit == null)
                return new Vector3(0, 0, 0);

            Material mat = closestHit.Material;
            Vector3 P = closestHit.Point;
            Vector3 N = closestHit.Normal;
            Vector3 V = (ray.Origin - P).Normalize(); 

            // recebe I_E 
            Vector3 I = new Vector3(0, 0, 0);

            // ===== 4. K_A · I_A (Ambiente) =====
            I = I + mat.Color * _ambientLight * mat.Ka;

            // ===== 5. Σ_L ( K_D(N·L) + K_S(V·R)^n ) · S_L · I_L =====
            foreach (var light in _lights)
            {
                // A classe Light já sabe calcular direção e distância
                // independente do tipo (pontual, direcional, spot)
                Vector3 L = light.GetDirection(P);
                double distToLight = light.GetDistance(P);
                Vector3 I_L = light.GetEffectiveIntensity(P);

                // Se a intensidade efetiva é zero (ex: fora do cone do spot), pula
                if (I_L.X <= 0 && I_L.Y <= 0 && I_L.Z <= 0)
                    continue;

                // --- S_L: Shadow Ray ---
                double S_L = 1.0;
                Ray shadowRay = new Ray(P + N * 0.001, L);

                foreach (var obj in _objects)
                {
                    if (obj.Hit(shadowRay, out HitRecord shadowHit))
                    {
                        if (shadowHit.T > 0.001 && shadowHit.T < distToLight)
                        {
                            S_L = 0.0;
                            break;
                        }
                    }
                }

                // --- K_D · (N · L) ---
                double NdotL = Math.Max(0, Vector3.DotProduct(N, L));
                double diffuse = mat.Kd * NdotL;

                // --- K_S · (V · R)^n ---
                Vector3 R = (2.0 * NdotL * N - L).Normalize();
                double VdotR = Math.Max(0, Vector3.DotProduct(V, R));
                double specular = mat.Ks * Math.Pow(VdotR, mat.Shininess);

                // --- (difuso + especular) · S_L · I_L ---
                Vector3 directLight =
                    (mat.Color * diffuse + new Vector3(specular, specular, specular))
                    * S_L
                    * I_L;

                I = I + directLight;
            }

            // ===== 6. Clampar [0, 1] =====
            I = new Vector3(
                Math.Min(1, Math.Max(0, I.X)),
                Math.Min(1, Math.Max(0, I.Y)),
                Math.Min(1, Math.Max(0, I.Z))
            );

            return I;
        }
    }
}
