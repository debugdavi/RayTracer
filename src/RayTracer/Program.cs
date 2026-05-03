// See https://aka.ms/new-console-template for more information

using RayTracer.Core;
using RayTracer.Scene;

int width = 400;
int height = 200;

using var writer = new StreamWriter("image.ppm");

writer.WriteLine("P3");
writer.WriteLine($"{width} {height}");
writer.WriteLine("255");

Vector3 origin = new Vector3(0, 0, 0);
Sphere sphere = new Sphere(new Vector3(0, 0, -1), 0.5);

for (int j = height - 1; j >= 0; j--)
{
    for (int i = 0; i < width; i++)
    {
        double u = (double)i / width;
        double v = (double)j / height;
        double aspectRatio = (double)width / height;

        Vector3 direction = new Vector3((2 * u - 1) * aspectRatio,
                                        (2 * v - 1),
                                        -1);

        Ray ray = new Ray(origin, direction);

        if (sphere.Hit(ray, out double t))
        {
            Vector3 point = ray.At(t);
            Vector3 normal = (point - sphere.Center).Normalize();

            Vector3 color = new Vector3(
                0.5 * (normal.X + 1),
                0.5 * (normal.Y + 1),
                0.5 * (normal.Z + 1)
            );

            int ir = (int)(255.999 * color.X);
            int ig = (int)(255.999 * color.Y);
            int ib = (int)(255.999 * color.Z);

            writer.WriteLine($"{ir} {ig} {ib}");
        }
        else
        {
            writer.WriteLine("0 0 0");
        }
    }
}