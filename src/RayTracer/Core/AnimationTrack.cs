using System;
using System.Collections.Generic;
using System.Linq;

namespace RayTracer.Core
{
    public class AnimationTrack<T>
    {
        private List<Keyframe<T>> _keyframes = new List<Keyframe<T>>();

        public void AddKeyframe(double time, T value)
        {
            _keyframes.Add(new Keyframe<T>(time, value));
            // Manter sempre ordenado por tempo
            _keyframes = _keyframes.OrderBy(k => k.Time).ToList();
        }

        public T Evaluate(double time)
        {
            if (_keyframes.Count == 0)
                return default; // Fallback se não tiver animação

            if (_keyframes.Count == 1)
                return _keyframes[0].Value;

            // Se o tempo é antes do primeiro frame, retorna o primeiro
            if (time <= _keyframes.First().Time)
                return _keyframes.First().Value;

            // Se o tempo é depois do último frame, retorna o último
            if (time >= _keyframes.Last().Time)
                return _keyframes.Last().Value;

            // Encontrar os dois keyframes adjacentes
            for (int i = 0; i < _keyframes.Count - 1; i++)
            {
                var kf1 = _keyframes[i];
                var kf2 = _keyframes[i + 1];

                if (time >= kf1.Time && time <= kf2.Time)
                {
                    double t = (time - kf1.Time) / (kf2.Time - kf1.Time);
                    return Lerp(kf1.Value, kf2.Value, t);
                }
            }

            return _keyframes.Last().Value; // Fallback
        }

        private T Lerp(T a, T b, double t)
        {
            if (typeof(T) == typeof(Vector3))
            {
                Vector3 va = a as Vector3;
                Vector3 vb = b as Vector3;
                Vector3 result = new Vector3(
                    va.X + (vb.X - va.X) * t,
                    va.Y + (vb.Y - va.Y) * t,
                    va.Z + (vb.Z - va.Z) * t
                );
                return (T)(object)result;
            }
            else if (typeof(T) == typeof(double))
            {
                double da = Convert.ToDouble(a);
                double db = Convert.ToDouble(b);
                double result = da + (db - da) * t;
                return (T)(object)result;
            }

            throw new NotSupportedException($"Lerp não suportado para o tipo {typeof(T)}");
        }
    }
}
