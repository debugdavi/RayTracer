using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class Vector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public Vector3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 a, Vector3 b) => new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        public static Vector3 operator -(Vector3 a, Vector3 b) => new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        public static Vector3 operator *(Vector3 v, double t) => new Vector3(v.X * t, v.Y * t, v.Z * t);
        public static double DotProduct(Vector3 a, Vector3 b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z; 
        public double LengthSquared() => DotProduct(this, this);
        public Vector3 Normalize()
        {
            double length = Math.Sqrt(LengthSquared()); // fazendo o calculo da raiz quadrada para obter o modulo/magnitude do vetor 

            if (length == 0)
                return new Vector3(0, 0, 0);

            return new Vector3(X / length, Y / length, Z / length); // retornando um novo vetor com as componentes divididas pelo modulo para obter o vetor unitário
        }

    }
}
