using RayTracer.Core;
using RayTracer.Scene;

const int width = 800;
const int height = 600;

Vector3 eye = new Vector3(0, 4, 8);             
Vector3 lookAt = new Vector3(0, 1.5, 0);         
Vector3 up = new Vector3(0, 1, 0);
double fovDesejado = 45;                         
double fovRadianos = fovDesejado * Math.PI / 180.0;

Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos);

// Material do Teapot
Material matTeapot = new Material(
    new Vector3(0.7, 0.3, 0.15),   // Marrom/terracota
    ka: 0.1, kd: 0.7, ks: 0.6, shininess: 64
);

Material matVermelho = new Material(
    new Vector3(0.8, 0.1, 0.1),
    ka: 0.1, kd: 0.7, ks: 0.5, shininess: 32
);

Material matAzul = new Material(
    new Vector3(0.1, 0.2, 0.9),
    ka: 0.1, kd: 0.6, ks: 0.8, shininess: 64
);

Mesh teapot = ObjLoader.Load("Models/teapot.obj", matTeapot);
Sphere esfera = new Sphere(new Vector3(0, 0, 0), 1.0, matVermelho);
Sphere esfera2 = new Sphere(new Vector3(0, 0, 0), 1.0, matAzul);

SceneNode raiz = new SceneNode();

// Teapot: rotacionado 30° em Y, escalado para 0.5, movido para a esquerda
Matrix4x4 teapotTransform =
    Transform.Translate(-1.5, 0, 0) *
    Transform.RotateY(30) *
    Transform.Scale(0.5);

raiz.AddChild(new SceneNode(teapotTransform, teapot));

// Esfera vermelha: movida para a direita, escalada para 0.5
Matrix4x4 esferaTransform =
    Transform.Translate(1.5, 0.5, 0) *
    Transform.Scale(0.5);

raiz.AddChild(new SceneNode(esferaTransform, esfera));

// Esfera azul: acima do teapot, pequena
Matrix4x4 esfera2Transform =
    Transform.Translate(-1.5, 2.0, 0) *
    Transform.Scale(0.3);

raiz.AddChild(new SceneNode(esfera2Transform, esfera2));

// ===== "Aplanar" a hierarquia e adicionar ao World =====
World world = new World();
foreach (var obj in raiz.Flatten())
{
    world.Add(obj);
}

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

// Ray tracing recursivo e estruturas de aceleração 
// Verificar transformações hierárquicas

Console.WriteLine($"Renderização concluída! Arquivo salvo em: {Path.GetFullPath(path)}");