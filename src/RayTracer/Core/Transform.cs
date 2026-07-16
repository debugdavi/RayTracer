using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public static class Transform
    {
        public static Matrix4x4 Translate(double tx, double ty, double tz)
        {
            Matrix4x4 m = Matrix4x4.Identity();
            m.M[0, 3] = tx;
            m.M[1, 3] = ty;
            m.M[2, 3] = tz;
            return m;
        }

        public static Matrix4x4 Scale(double sx, double sy, double sz)
        {
            Matrix4x4 m = Matrix4x4.Identity();
            m.M[0, 0] = sx;
            m.M[1, 1] = sy;
            m.M[2, 2] = sz;
            return m;
        }

        public static Matrix4x4 Scale(double s) => Scale(s, s, s);

        public static Matrix4x4 RotateX(double angleDegrees)
        {
            double rad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            Matrix4x4 m = Matrix4x4.Identity();
            m.M[1, 1] = cos; m.M[1, 2] = -sin;
            m.M[2, 1] = sin; m.M[2, 2] = cos;
            return m;
        }

        public static Matrix4x4 RotateY(double angleDegrees)
        {
            double rad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            Matrix4x4 m = Matrix4x4.Identity();
            m.M[0, 0] = cos; m.M[0, 2] = sin;
            m.M[2, 0] = -sin; m.M[2, 2] = cos;
            return m;
        }

        public static Matrix4x4 RotateZ(double angleDegrees)
        {
            double rad = angleDegrees * Math.PI / 180.0;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            Matrix4x4 m = Matrix4x4.Identity();
            m.M[0, 0] = cos; m.M[0, 1] = -sin;
            m.M[1, 0] = sin; m.M[1, 1] = cos;
            return m;
        }
    }
}
