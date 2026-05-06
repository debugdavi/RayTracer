using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class Camera
    {
        public int Nx { get; set; }
        public int Ny { get; set; }
        public Vector3 Eye { get; set; }
        public Vector3 LookAt { get; set; }
        public Vector3 Up { get; set; }
        public double Fov { get; set; }
        public Vector3 W => (Eye - LookAt).Normalize();
        public Vector3 U => Vector3.CrossProduct(Up, W).Normalize();
        public Vector3 V => Vector3.CrossProduct(W, U);

        public Camera(int nx, int ny, Vector3 eye, Vector3 lookAt, Vector3 up, double fov)
        {
            Nx = nx;
            Ny = ny;
            Eye = eye;
            LookAt = lookAt;
            Up = up;
            Fov = fov;
        }

        public Ray GetRay(int i, int j)
        {
            double height = 2 * Math.Tan(Fov / 2);
            double width = height * (Nx / Ny);
            
            double s = ((i + 0.5) / Nx - 0.5) * width;
            double t = ((j + 0.5) / Ny - 0.5) * height;

            Ray d = new Ray(Eye, (s * U + t * V - W).Normalize());
            return d;
        }
    }
}
