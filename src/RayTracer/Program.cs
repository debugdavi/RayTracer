using RayTracer.Core;
using RayTracer.Scene;

const int width = 400;
const int height = 300;

// Câmera posicionada um pouco acima, olhando para baixo
Vector3 eye = new Vector3(0, 3, 7);
Vector3 lookAt = new Vector3(0, 0, 0);
Vector3 up = new Vector3(0, 1, 0);
double fovRadianos = 50 * Math.PI / 180.0;

Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos);

// ===== Texturas Procedurais (Não precisam de arquivo!) =====
// 1. Chão Xadrez (Tiling funciona perfeitamente pois Sample tem UV wrapping)
Texture texChao = Texture.Checkerboard(512, 16, new Vector3(0.8, 0.8, 0.8), new Vector3(0.2, 0.2, 0.2));
// 2. Textura listrada para simular uma caixa de madeira/dado
Texture texCaixa = Texture.Checkerboard(256, 4, new Vector3(0.6, 0.3, 0.1), new Vector3(0.4, 0.2, 0.05));
// 3. Xadrez azulzinho (substituto pra Terra se não tiver a imagem)
Texture texTerraFallback = Texture.Checkerboard(256, 8, new Vector3(0.1, 0.3, 0.8), new Vector3(0.1, 0.8, 0.3));

// Descomente a linha abaixo para usar a foto real da Terra se você baixou:
Texture texTerra = new Texture("Textures/earth.jpg"); 
//Texture texTerra = texTerraFallback; 

// ===== Materiais =====
Material matChao = new Material(texChao, ka: 0.2, kd: 0.8, ks: 0.1, shininess: 8);
Material matTerra = new Material(texTerra, ka: 0.1, kd: 0.8, ks: 0.4, shininess: 32);
Material matCaixa = new Material(texCaixa, ka: 0.1, kd: 0.8, ks: 0.2, shininess: 16);
Material matPolido = new Material(new Vector3(0.9, 0.1, 0.1), ka: 0.1, kd: 0.7, ks: 0.9, shininess: 128);

// NOVO: Material espelhado (reflexão)
Material matEspelho = new Material(new Vector3(0.95, 0.95, 0.95), ka: 0.02, kd: 0.05, ks: 0.9, shininess: 200)
    { Reflectivity = 0.85 };

// NOVO: Material vidro (refração + reflexão via Fresnel)
Material matVidro = new Material(new Vector3(0.95, 0.95, 1.0), ka: 0.0, kd: 0.02, ks: 0.9, shininess: 200)
    { Transparency = 0.9, RefractiveIndex = 1.5 };

// ===== Geometria =====
Mesh chao = ObjLoader.Load("Models/plane.obj", matChao);
Mesh caixa = ObjLoader.Load("Models/cube.obj", matCaixa);
Mesh teapot = ObjLoader.Load("Models/teapot.obj", matPolido);
Sphere terra = new Sphere(new Vector3(0, 0, 0), 1.0, matTerra);
Sphere espelho = new Sphere(new Vector3(0, 0, 0), 1.0, matEspelho);
Sphere vidro = new Sphere(new Vector3(0, 0, 0), 1.0, matVidro);

// ===== Hierarquia de Cena =====
SceneNode raiz = new SceneNode();

// 1. Chão (Escalado para ser bem grande)
raiz.AddChild(new SceneNode(Transform.Scale(4), chao));

// 2. Terra (Centro-esquerda)
Matrix4x4 terraTransform = Transform.Translate(-1.5, 1.0, -1) * Transform.RotateY(-45);
raiz.AddChild(new SceneNode(terraTransform, terra));

// 3. Esfera Espelhada (Centro)
Matrix4x4 espelhoTransform = Transform.Translate(1.0, 0.8, 1.0) * Transform.Scale(0.8);
raiz.AddChild(new SceneNode(espelhoTransform, espelho));

// 4. Esfera de Vidro (Centro-direita, menor)
Matrix4x4 vidroTransform = Transform.Translate(-0.3, 0.5, 2.0) * Transform.Scale(0.5);
raiz.AddChild(new SceneNode(vidroTransform, vidro));

// 5. Caixa (Fundo esquerdo)
Matrix4x4 caixaTransform = Transform.Translate(-3.0, 1.0, -2) * Transform.RotateY(30) * Transform.RotateX(15);
raiz.AddChild(new SceneNode(caixaTransform, caixa));

// 6. Teapot (Fundo direito)
Matrix4x4 teapotTransform = Transform.Translate(3.0, 0.0, -1) * Transform.RotateY(-30) * Transform.Scale(0.5);
raiz.AddChild(new SceneNode(teapotTransform, teapot));

// ===== Aplanar Hierarquia =====
World world = new World();
foreach (var obj in raiz.Flatten()) world.Add(obj);

// ===== Iluminação =====
world.AddLight(new PointLight(new Vector3(3, 5, 4), new Vector3(1, 1, 1)));       // Luz principal
world.AddLight(new PointLight(new Vector3(-4, 3, -2), new Vector3(0.3, 0.3, 0.5))); // Luz de preenchimento azulada

// ===== Render Paralelo =====
string path = "render.ppm";
Vector3[,] buffer = new Vector3[height, width];
var sw = System.Diagnostics.Stopwatch.StartNew();

Console.WriteLine($"Renderizando Cena Showcase ({width}x{height})...");
int linhasConcluidas = 0;

Parallel.For(0, height, i =>
{
    int row = height - 1 - i;
    for (int j = 0; j < width; j++)
    {
        Ray ray = cam.GetRay(j, row);
        buffer[i, j] = world.Trace(ray);
    }
    
    // Contador seguro para threads
    int progresso = System.Threading.Interlocked.Increment(ref linhasConcluidas);
    if (progresso % 10 == 0)
        Console.WriteLine($"  {progresso}/{height} linhas concluídas...");
});
sw.Stop();
Console.WriteLine($"Pronto em {sw.Elapsed.TotalSeconds:F1}s!");

// Gravar PPM
using (StreamWriter writer = new StreamWriter(path))
{
    writer.WriteLine("P3\n{0} {1}\n255", width, height);
    for (int i = 0; i < height; i++)
        for (int j = 0; j < width; j++)
        {
            Vector3 c = buffer[i, j];
            writer.WriteLine($"{(int)(255.999 * c.X)} {(int)(255.999 * c.Y)} {(int)(255.999 * c.Z)}");
        }
}
Console.WriteLine($"Salvo em: {Path.GetFullPath(path)}");