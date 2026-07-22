# 🌟 RayTracer

Renderizador baseado em Ray Tracing desenvolvido como projeto da disciplina de **Computação Gráfica** na faculdade.

O objetivo do projeto é implementar, de forma incremental, os principais conceitos de um motor de renderização por traçado de raios, desde a geração de raios primários até a produção de uma animação curta com objetos texturizados, iluminação realista e movimento.

---

## ✨ Funcionalidades Implementadas

### Renderização Base
- **Raios Primários** — Câmera perspectiva com geração de raios por pixel
- **Interseção com Esferas** — Equação quadrática analítica
- **Interseção com Triângulos** — Algoritmo de Möller-Trumbore com coordenadas baricêntricas
- **Carregamento de Malhas (.OBJ)** — Suporte a vértices, normais, UVs e triangulação por fan

### Iluminação e Sombreamento
- **Modelo de Phong** — Componentes ambiente, difusa e especular
- **Shadow Rays** — Sombras por raios de verificação de oclusão
- **Múltiplos tipos de luz** — Pontual, Direcional e Spot (cone)
- **Smooth Shading** — Interpolação baricêntrica de normais por vértice

### Ray Tracing Recursivo
- **Reflexão** — Superfícies espelhadas com controle de reflectividade
- **Refração** — Lei de Snell para materiais transparentes (vidro, água, diamante)
- **Reflexão Interna Total** — Tratamento correto do ângulo crítico
- **Aproximação de Fresnel (Schlick)** — Mistura realista entre reflexão e refração

### Texturas
- **Texturas de Arquivo** — Carregamento de PNG/JPG via StbImageSharp
- **Texturas Procedurais** — Gerador de padrão Checkerboard com cores customizáveis
- **Mapeamento UV Esférico** — Projeção via `atan2`/`asin`
- **Mapeamento UV em Triângulos** — Interpolação baricêntrica de coordenadas de textura

### Transformações e Hierarquia
- **Matrizes 4×4** — Multiplicação, inversão (Gauss-Jordan), transformação de ponto/direção/normal
- **Transformações Geométricas** — Translação, Escala, Rotação (X, Y, Z)
- **TransformedObject** — Raio transformado para object space, resultado convertido de volta para world space
- **Grafo de Cena (SceneNode)** — Hierarquia pai-filho com `Flatten()` recursivo que acumula transformações

### Estrutura de Aceleração
- **AABB (Axis-Aligned Bounding Box)** — Teste de interseção via Slab Method
- **BVH (Bounding Volume Hierarchy)** — Árvore binária que reduz a complexidade de interseção de O(n) para O(log n)

### Antialiasing
- **Supersampling Estocástico** — Múltiplas amostras por pixel (16 spp) com jitter aleatório

### Profundidade de Campo (Depth of Field)
- **Simulação de Lente** — Abertura configurável e distância focal
- **Efeito Bokeh** — Desfoque natural de objetos fora do plano de foco

### Sistema de Animação
- **Keyframes** — Pares (tempo, valor) para definir estados ao longo do tempo
- **AnimationTrack** — Interpolação linear (Lerp) entre keyframes para Vector3 e double
- **Animator** — Controlador que orquestra tracks de posição, rotação e escala, devolvendo matrizes de transformação por instante de tempo
- **Geração de Sequência de Frames** — Loop temporal que reconstrói a cena e renderiza cada frame como `.ppm`

### Otimizações
- **Renderização Paralela** — `Parallel.For` com progresso via `Interlocked`
- **Cache de Câmera** — Vetores U, V, W e dimensões do plano de projeção pré-calculados, evitando recálculo por raio
- **BVH interna em Meshes** — Malhas complexas (ex: teapot com 6320 triângulos) usam BVH própria
- **Gamma Correction** — Correção `x^(1/2.2)` na saída para cores mais fiéis

---

## 🛠️ Tecnologias

- **Linguagem:** C# (.NET 8)
- **Biblioteca de Imagem:** [StbImageSharp](https://github.com/StbSharp/StbImageSharp) (carregamento de texturas PNG/JPG)
- **Formato de Saída:** PPM (convertido para MP4 via FFmpeg)

---

## 🚀 Como Executar

```bash
cd src/RayTracer
dotnet run
```

O programa gera uma sequência de frames `.ppm` na pasta `frames/`. Para converter em vídeo:

```bash
cd frames
ffmpeg -framerate 10 -i frame_%03d.ppm -c:v libx264 -pix_fmt yuv420p animacao.mp4
```

---

## 📁 Estrutura do Projeto

```
src/RayTracer/
├── Core/
│   ├── Interfaces/
│   │   └── IHittable.cs
│   ├── AABB.cs
│   ├── AnimationTrack.cs
│   ├── Animator.cs
│   ├── Camera.cs
│   ├── HitRecord.cs
│   ├── Keyframe.cs
│   ├── Material.cs
│   ├── Matrix4x4.cs
│   ├── Ray.cs
│   ├── Texture.cs
│   ├── Transform.cs
│   └── Vector3.cs
├── Scene/
│   ├── BVHNode.cs
│   ├── DirectionalLight.cs
│   ├── Light.cs
│   ├── Mesh.cs
│   ├── ObjLoader.cs
│   ├── PointLight.cs
│   ├── SceneNode.cs
│   ├── Sphere.cs
│   ├── SpotLight.cs
│   ├── TransformedObject.cs
│   ├── Triangle.cs
│   └── World.cs
├── Models/
│   ├── teapot.obj
│   ├── cube.obj
│   └── plane.obj
├── Textures/
│   └── earth.jpg
└── Program.cs
```
