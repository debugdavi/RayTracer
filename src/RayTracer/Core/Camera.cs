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
        public double Aperture { get; set; }
        public double FocusDistance { get; set; }

        public Vector3 W => (Eye - LookAt).Normalize();
        public Vector3 U => Vector3.CrossProduct(Up, W).Normalize();
        public Vector3 V => Vector3.CrossProduct(W, U);

        public Camera(int nx, int ny, Vector3 eye, Vector3 lookAt, Vector3 up, double fov, double aperture = 0.0, double focusDistance = 1.0)
        {
            Nx = nx;
            Ny = ny;
            Eye = eye;
            LookAt = lookAt;
            Up = up;
            Fov = fov;
            Aperture = aperture;
            FocusDistance = focusDistance;
        }

        public Ray GetRay(double i, double j)
        {
            double height = 2 * Math.Tan(Fov / 2);
            double width = height * ((double)Nx / Ny);
            
            double s = (i / Nx - 0.5) * width;
            double t = (j / Ny - 0.5) * height;

            Vector3 dir = s * U + t * V - W;

            if (Aperture <= 0.0)
            {
                return new Ray(Eye, dir.Normalize());
            }

            // Depth of Field
            Vector3 focalPoint = Eye + dir * FocusDistance;

            // Ponto aleatório no disco unitário
            double rdX, rdY;
            do
            {
                rdX = 2.0 * Random.Shared.NextDouble() - 1.0;
                rdY = 2.0 * Random.Shared.NextDouble() - 1.0;
            } while (rdX * rdX + rdY * rdY >= 1.0);

            double lensRadius = Aperture / 2.0;
            Vector3 offset = (rdX * U + rdY * V) * lensRadius;
            Vector3 newOrigin = Eye + offset;

            return new Ray(newOrigin, (focalPoint - newOrigin).Normalize());
        }
    }
}
