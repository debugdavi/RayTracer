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
        private int _maxDepth = 5; // Profundidade máxima de recursão

        public void Add(IHittable obj) => _objects.Add(obj);
        public void AddLight(Light light) => _lights.Add(light);

        /// <summary>
        /// Traça um raio na cena. Agora é RECURSIVO:
        /// - depth = 0: raio primário (da câmera)
        /// - depth > 0: raio de reflexão ou refração
        /// - depth >= maxDepth: para a recursão (retorna preto)
        /// </summary>
        public Vector3 Trace(Ray ray, int depth = 0)
        {
            // ===== Condição de parada da recursão =====
            if (depth >= _maxDepth)
                return new Vector3(0, 0, 0);

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
            Vector3 texColor = mat.GetColor(closestHit.U, closestHit.V);

            // ===== Iluminação local (Phong) =====
            Vector3 localColor = ComputePhong(mat, texColor, P, N, V);

            // ===== Se o material não tem reflexão nem refração, retorna direto =====
            if (mat.Reflectivity <= 0 && mat.Transparency <= 0)
                return Clamp(localColor);

            // ===== Reflexão + Refração recursiva =====
            Vector3 finalColor = localColor;

            // Determinar se estamos entrando ou saindo do objeto
            bool entering = Vector3.DotProduct(ray.Direction, N) < 0;
            Vector3 faceNormal = entering ? N : new Vector3(-N.X, -N.Y, -N.Z);
            double n1 = entering ? 1.0 : mat.RefractiveIndex;  // meio de onde vem
            double n2 = entering ? mat.RefractiveIndex : 1.0;  // meio para onde vai

            double cosI = Math.Max(0, Vector3.DotProduct(faceNormal, V));

            // ===== Calcular coeficiente de Fresnel (Schlick) =====
            // Fresnel diz: quanto mais rasante o ângulo, mais reflexão
            double fresnel = 1.0;
            if (mat.Transparency > 0)
            {
                double r0 = (n1 - n2) / (n1 + n2);
                r0 = r0 * r0;
                fresnel = r0 + (1.0 - r0) * Math.Pow(1.0 - cosI, 5);
            }
            else if (mat.Reflectivity > 0)
            {
                fresnel = 1.0; // material opaco reflexivo: toda contribuição é reflexão
            }

            // ===== Reflexão =====
            Vector3 reflectedColor = new Vector3(0, 0, 0);
            if (mat.Reflectivity > 0 || mat.Transparency > 0)
            {
                // R = D - 2(D·N)N  (reflexão do raio incidente)
                Vector3 D = ray.Direction.Normalize();
                double DdotN = Vector3.DotProduct(D, faceNormal);
                Vector3 reflDir = (D - 2.0 * DdotN * faceNormal).Normalize();

                Ray reflRay = new Ray(P + faceNormal * 0.001, reflDir);
                reflectedColor = Trace(reflRay, depth + 1);  // ← RECURSÃO!
            }

            // ===== Refração =====
            Vector3 refractedColor = new Vector3(0, 0, 0);
            bool totalInternalReflection = false;

            if (mat.Transparency > 0)
            {
                double eta = n1 / n2;
                Vector3 D = ray.Direction.Normalize();
                double DdotN = Vector3.DotProduct(D, faceNormal);
                double sin2T = eta * eta * (1.0 - DdotN * DdotN);

                if (sin2T > 1.0)
                {
                    // Reflexão interna total — não há refração possível
                    totalInternalReflection = true;
                }
                else
                {
                    double cosT = Math.Sqrt(1.0 - sin2T);
                    Vector3 refrDir = (eta * D - (eta * DdotN + cosT) * faceNormal).Normalize();

                    // Offset para DENTRO do objeto (direção oposta à normal)
                    Ray refrRay = new Ray(P - faceNormal * 0.001, refrDir);
                    refractedColor = Trace(refrRay, depth + 1);  // ← RECURSÃO!
                }
            }

            // ===== Misturar tudo =====
            if (mat.Transparency > 0 && !totalInternalReflection)
            {
                // Material transparente: mistura reflexão e refração via Fresnel
                // Fresnel diz quanto é reflexão vs refração
                Vector3 transparentColor = fresnel * reflectedColor + (1.0 - fresnel) * refractedColor;

                // Mistura cor local (Phong) com transparência
                finalColor = (1.0 - mat.Transparency) * localColor + mat.Transparency * transparentColor;
            }
            else if (mat.Transparency > 0 && totalInternalReflection)
            {
                // Reflexão interna total: toda luz refletida
                finalColor = (1.0 - mat.Transparency) * localColor + mat.Transparency * reflectedColor;
            }
            else if (mat.Reflectivity > 0)
            {
                // Material opaco reflexivo (espelho): mistura Phong com reflexo
                finalColor = (1.0 - mat.Reflectivity) * localColor + mat.Reflectivity * reflectedColor;
            }

            return Clamp(finalColor);
        }

        /// <summary>
        /// Calcula a iluminação local de Phong (ambiente + difusa + especular + sombras).
        /// Extraído do Trace original para manter o código limpo.
        /// </summary>
        private Vector3 ComputePhong(Material mat, Vector3 texColor, Vector3 P, Vector3 N, Vector3 V)
        {
            // recebe I_E
            Vector3 I = new Vector3(0, 0, 0);

            // ===== K_A · I_A (Ambiente) =====
            I = I + texColor * _ambientLight * mat.Ka;

            // ===== Σ_L ( K_D(N·L) + K_S(V·R)^n ) · S_L · I_L =====
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
                    (texColor * diffuse + new Vector3(specular, specular, specular))
                    * S_L
                    * I_L;

                I = I + directLight;
            }

            return I;
        }

        /// <summary>
        /// Clampa cada componente para [0, 1].
        /// </summary>
        private Vector3 Clamp(Vector3 c)
        {
            return new Vector3(
                Math.Min(1, Math.Max(0, c.X)),
                Math.Min(1, Math.Max(0, c.Y)),
                Math.Min(1, Math.Max(0, c.Z))
            );
        }
    }
}
