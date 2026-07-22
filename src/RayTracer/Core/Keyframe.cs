using System;

namespace RayTracer.Core
{
    public class Keyframe<T>
    {
        public double Time { get; set; }
        public T Value { get; set; }

        public Keyframe(double time, T value)
        {
            Time = time;
            Value = value;
        }
    }
}
