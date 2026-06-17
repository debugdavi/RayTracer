using RayTracer.Core;
using RayTracer.Scene;

const int width = 1980;
const int height = 1280;

Vector3 eye = new Vector3(0, 0, 1);
Vector3 lookAt = new Vector3(0, 0, -1);
Vector3 up = new Vector3(0, 1, 0);

double fovDesejado = 90;
double fovRadianos = fovDesejado * Math.PI / 180.0;

Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos);

// criação de materiais
Material matVermelho = new Material(
    new Vector3(0.8, 0.1, 0.1),   // Vermelho — plástico
    ka: 0.1, kd: 0.7, ks: 0.5, shininess: 32
);

Material matAzul = new Material(
    new Vector3(0.1, 0.2, 0.9),   // Azul — mais brilhante
    ka: 0.1, kd: 0.6, ks: 0.8, shininess: 64
);

Material matVerde = new Material(
    new Vector3(0.1, 0.8, 0.2),   // Verde — fosco
    ka: 0.1, kd: 0.9, ks: 0.1, shininess: 8
);

World world = new World();
world.Add(new Sphere(new Vector3(0, 0, -1), 0.5, matVermelho));           
world.Add(new Sphere(new Vector3(1, 0, -1), 0.3, matAzul));       
world.Add(new Sphere(new Vector3(-1, 0, -1), 0.3, matVerde));

// Luz pontual — como uma lâmpada (acima-direita)
world.AddLight(new PointLight(
    new Vector3(2, 2, 1),
    new Vector3(1, 1, 1)
));

// Luz direcional — como o sol (vem de cima)
world.AddLight(new DirectionalLight(
    new Vector3(0, -1, -0.5),         // direção: de cima para baixo
    new Vector3(0.3, 0.3, 0.3)        // fraca, para simular luz ambiente direcional
));

// Luz spot — como uma lanterna (foca na esfera central)
world.AddLight(new SpotLight(
    new Vector3(0, 2, 0),             // posição: acima
    new Vector3(0, -1, -1),           // aponta para baixo-frente
    new Vector3(0.8, 0.8, 0.6),       // levemente amarela
    cutoffAngleDegrees: 30,            // cone de 30°
    falloffExponent: 2.0               // queda suave
));

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