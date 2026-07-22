using RayTracer.Core;
using RayTracer.Scene;
using System.IO;
using System.Diagnostics;

const int width = 400;
const int height = 300;

// Configuração da Câmera Inicial
Vector3 eye = new Vector3(0, 3, 7);
Vector3 lookAt = new Vector3(0, 0, 0);
Vector3 up = new Vector3(0, 1, 0);
double fovRadianos = 50 * Math.PI / 180.0;
double distFocus = Math.Sqrt((eye - lookAt).LengthSquared());
double aperture = 0.2; 
Camera cam = new Camera(width, height, eye, lookAt, up, fovRadianos, aperture, distFocus);

// ===== Texturas Procedurais =====
Texture texChao = Texture.Checkerboard(512, 16, new Vector3(0.8, 0.8, 0.8), new Vector3(0.2, 0.2, 0.2));
Texture texCaixa = Texture.Checkerboard(256, 4, new Vector3(0.6, 0.3, 0.1), new Vector3(0.4, 0.2, 0.05));
Texture texTerraFallback = Texture.Checkerboard(256, 8, new Vector3(0.1, 0.3, 0.8), new Vector3(0.1, 0.8, 0.3));
Texture texTerra = new Texture("Textures/earth.jpg"); 

// ===== Materiais =====
Material matChao = new Material(texChao, ka: 0.2, kd: 0.8, ks: 0.1, shininess: 8);
Material matTerra = new Material(texTerra, ka: 0.1, kd: 0.8, ks: 0.4, shininess: 32);
Material matCaixa = new Material(texCaixa, ka: 0.1, kd: 0.8, ks: 0.2, shininess: 16);
Material matPolido = new Material(new Vector3(0.9, 0.1, 0.1), ka: 0.1, kd: 0.7, ks: 0.9, shininess: 128);
Material matEspelho = new Material(new Vector3(0.95, 0.95, 0.95), ka: 0.02, kd: 0.05, ks: 0.9, shininess: 200) { Reflectivity = 0.85 };
Material matVidro = new Material(new Vector3(0.95, 0.95, 1.0), ka: 0.0, kd: 0.02, ks: 0.9, shininess: 200) { Transparency = 0.9, RefractiveIndex = 1.5 };

// ===== Geometria (Carregada uma vez) =====
Mesh chao = ObjLoader.Load("Models/plane.obj", matChao);
Mesh caixa = ObjLoader.Load("Models/cube.obj", matCaixa);
Mesh teapot = ObjLoader.Load("Models/teapot.obj", matPolido);
Sphere terra = new Sphere(new Vector3(0, 0, 0), 1.0, matTerra);
Sphere espelho = new Sphere(new Vector3(0, 0, 0), 1.0, matEspelho);
Sphere vidro = new Sphere(new Vector3(0, 0, 0), 1.0, matVidro);

// ===== ANIMATOR =====
Animator animator = new Animator();

// Animação 1: Câmera Orbitando
var camTrack = new AnimationTrack<Vector3>();
camTrack.AddKeyframe(0.0, new Vector3(0, 3, 7));
camTrack.AddKeyframe(1.5, new Vector3(7, 3, 0));
camTrack.AddKeyframe(3.0, new Vector3(0, 3, 7));
animator.PositionTracks["camera"] = camTrack;

// Animação 2: Teapot Girando 360 graus
var teapotRot = new AnimationTrack<Vector3>();
teapotRot.AddKeyframe(0.0, new Vector3(0, -30, 0));
teapotRot.AddKeyframe(3.0, new Vector3(0, 330, 0));
animator.RotationTracks["teapot"] = teapotRot;
// Posição base do teapot (constante, mas precisa estar na track para o transform não zerar)
var teapotPos = new AnimationTrack<Vector3>();
teapotPos.AddKeyframe(0.0, new Vector3(3.0, 0.0, -1));
animator.PositionTracks["teapot"] = teapotPos;
var teapotScale = new AnimationTrack<Vector3>();
teapotScale.AddKeyframe(0.0, new Vector3(0.5, 0.5, 0.5));
animator.ScaleTracks["teapot"] = teapotScale;

// Animação 3: Esfera de vidro flutuando (ping-pong)
var vidroPos = new AnimationTrack<Vector3>();
vidroPos.AddKeyframe(0.0, new Vector3(-0.3, 0.5, 2.0));
vidroPos.AddKeyframe(1.5, new Vector3(-0.3, 1.5, 2.0));
vidroPos.AddKeyframe(3.0, new Vector3(-0.3, 0.5, 2.0));
animator.PositionTracks["vidro"] = vidroPos;
var vidroScale = new AnimationTrack<Vector3>();
vidroScale.AddKeyframe(0.0, new Vector3(0.5, 0.5, 0.5));
animator.ScaleTracks["vidro"] = vidroScale;

// Garantir que a pasta frames exista
Directory.CreateDirectory("frames");

// ===== LOOP DE TEMPO (Geração da Sequência) =====
double duration = 3.0; // 3 segundos
int fps = 10;          // 10 frames por segundo (rápido para teste, pode subir para 24)
int totalFrames = (int)(duration * fps);

Vector3[,] buffer = new Vector3[height, width];
var sw = Stopwatch.StartNew();

for (int frame = 0; frame < totalFrames; frame++)
{
    double time = (double)frame / fps;
    Console.WriteLine($"\n=== Renderizando Frame {frame+1}/{totalFrames} (t={time:F2}s) ===");

    // 1. Atualizar a Câmera
    cam.Eye = animator.PositionTracks["camera"].Evaluate(time);
    cam.FocusDistance = Math.Sqrt((cam.Eye - cam.LookAt).LengthSquared()); // Manter foco no centro

    // 2. Reconstruir a Hierarquia da Cena com as transformações atuais
    SceneNode raiz = new SceneNode();
    
    // Objetos Estáticos
    raiz.AddChild(new SceneNode(Transform.Scale(4), chao));
    raiz.AddChild(new SceneNode(Transform.Translate(-1.5, 1.0, -1) * Transform.RotateY(-45), terra));
    raiz.AddChild(new SceneNode(Transform.Translate(1.0, 0.8, 1.0) * Transform.Scale(0.8), espelho));
    raiz.AddChild(new SceneNode(Transform.Translate(-3.0, 1.0, -2) * Transform.RotateY(30) * Transform.RotateX(15), caixa));

    // Objetos Animados (buscando matriz do Animator)
    raiz.AddChild(new SceneNode(animator.GetTransformAt("vidro", time), vidro));
    raiz.AddChild(new SceneNode(animator.GetTransformAt("teapot", time), teapot));

    // 3. Reconstruir e Preparar o World (BVH precisa ser gerada por frame pois as transformações mudam)
    World world = new World();
    foreach (var obj in raiz.Flatten()) world.Add(obj);
    world.AddLight(new PointLight(new Vector3(3, 5, 4), new Vector3(1, 1, 1)));
    world.AddLight(new PointLight(new Vector3(-4, 3, -2), new Vector3(0.3, 0.3, 0.5)));
    world.BuildBVH();

    // 4. Renderizar o Frame
    int linhasConcluidas = 0;
    Parallel.For(0, height, i =>
    {
        int row = height - 1 - i;
        int samplesPerPixel = 16; 
        for (int j = 0; j < width; j++)
        {
            Vector3 pixelColor = new Vector3(0, 0, 0);
            for (int s = 0; s < samplesPerPixel; s++)
            {
                double uOffset = Random.Shared.NextDouble();
                double vOffset = Random.Shared.NextDouble();
                Ray ray = cam.GetRay(j + uOffset, row + vOffset);
                pixelColor += world.Trace(ray);
            }
            buffer[i, j] = pixelColor * (1.0 / samplesPerPixel);
        }
        
        int progresso = System.Threading.Interlocked.Increment(ref linhasConcluidas);
        if (progresso % 50 == 0) Console.Write($".");
    });
    Console.WriteLine();

    // 5. Salvar o Frame em disco
    string filename = $"frames/frame_{frame:D3}.ppm";
    using (StreamWriter writer = new StreamWriter(filename))
    {
        writer.WriteLine("P3\n{0} {1}\n255", width, height);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                Vector3 c = buffer[i, j];
                // Gamma correction simples x^(1/2.2) para cores ficarem mais vivas
                double r = Math.Pow(Math.Clamp(c.X, 0, 1), 1.0 / 2.2);
                double g = Math.Pow(Math.Clamp(c.Y, 0, 1), 1.0 / 2.2);
                double b = Math.Pow(Math.Clamp(c.Z, 0, 1), 1.0 / 2.2);
                writer.WriteLine($"{(int)(255.999 * r)} {(int)(255.999 * g)} {(int)(255.999 * b)}");
            }
        }
    }
}

sw.Stop();
Console.WriteLine($"\nAnimação Completa ({totalFrames} frames) em {sw.Elapsed.TotalMinutes:F1} minutos!");