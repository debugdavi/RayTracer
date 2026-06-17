using RayTracer.Core;
using RayTracer.Scene;

const int width = 400;
const int height = 300;

Vector3 eye = new Vector3(0, 1, 3);             
Vector3 lookAt = new Vector3(0, 0.5, 0);         
Vector3 up = new Vector3(0, 1, 0);
double fovDesejado = 60;                         
double fovRadianos = fovDesejado * Math.PI / 180.0;

Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos);

// Material do Teapot
Material matTeapot = new Material(
    new Vector3(0.7, 0.3, 0.15),   // Marrom/terracota
    ka: 0.1, kd: 0.7, ks: 0.6, shininess: 64
);

Mesh teapot = ObjLoader.Load("Models/teapot.obj", matTeapot);

World world = new World();
world.Add(teapot);

// ===== Luzes =====
world.AddLight(new PointLight(
    new Vector3(3, 3, 3),
    new Vector3(1, 1, 1)
));

world.AddLight(new DirectionalLight(
    new Vector3(-1, -1, -0.5),
    new Vector3(0.3, 0.3, 0.3)
));

// ===== Render =====
string path = "render.ppm";
using (StreamWriter writer = new StreamWriter(path))
{
    writer.WriteLine("P3");
    writer.WriteLine($"{width} {height}");
    writer.WriteLine("255");

    for (int i = height - 1; i >= 0; i--)
    {
        // Progresso (o teapot tem muitos triângulos, pode demorar)
        if (i % 100 == 0)
            Console.WriteLine($"Renderizando linha {height - i}/{height}...");

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