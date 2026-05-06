using RayTracer.Core;
using RayTracer.Scene;

const int width = 400;
const int height = 200;


Vector3 eye = new Vector3(0, 0, 1);
Vector3 lookAt = new Vector3(0, 0, -1);
Vector3 up = new Vector3(0, 1, 0);
double fovDesejado = 90;
double fovRadianos = fovDesejado * Math.PI / 180.0;

Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos);

World world = new World();
world.Add(new Sphere(new Vector3(0, 0, -1), 0.5));        
world.Add(new Sphere(new Vector3(0, -100.5, -1), 100));    
world.Add(new Sphere(new Vector3(1, 0, -1), 0.3));       
world.Add(new Sphere(new Vector3(-1, 0, -1), 0.3));        

string path = "render.ppm";
using (StreamWriter writer = new StreamWriter(path))
{
    writer.WriteLine("P3");
    writer.WriteLine($"{width} {height}");
    writer.WriteLine("255");

    for (int i = height - 1; i >= 0; i--) 
    {
        for (int j = 0; j < width; j++)
        {
            Ray ray = cam.GetRay(j, i);


            Vector3 color = world.Trace(ray);

            int ir = (int)(255.999 * color.X);
            int ig = (int)(255.999 * color.Y);
            int ib = (int)(255.999 * color.Z);

            writer.WriteLine($"{ir} {ig} {ib}");
        }
    }
}

Console.WriteLine($"Renderização concluída! Arquivo salvo em: {Path.GetFullPath(path)}");