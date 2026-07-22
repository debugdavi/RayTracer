using RayTracer.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Scene
{
    public static class ObjLoader
    {
        public static Mesh Load(string filePath, Material material)
        {
            Mesh mesh = new Mesh(material);

            List<Vector3> vertices = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector3> texCoords = new List<Vector3>();

            string[] lines = File.ReadAllLines(filePath);

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                // Ignorar linhas vazias e comentários
                if (string.IsNullOrEmpty(line) || line.StartsWith("#"))
                    continue;

                string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // ===== v: vértice =====
                if (parts[0] == "v" && parts.Length >= 4)
                {
                    double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    double z = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    vertices.Add(new Vector3(x, y, z));
                }
                // ===== vt: coordenada de textura =====
                else if (parts[0] == "vt" && parts.Length >= 3)
                {
                    double u = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double vt = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    texCoords.Add(new Vector3(u, vt, 0));
                }
                // ===== vn: normal do vértice =====
                else if (parts[0] == "vn" && parts.Length >= 4)
                {
                    double x = double.Parse(parts[1], CultureInfo.InvariantCulture);
                    double y = double.Parse(parts[2], CultureInfo.InvariantCulture);
                    double z = double.Parse(parts[3], CultureInfo.InvariantCulture);
                    normals.Add(new Vector3(x, y, z));
                }
                // ===== f: face =====
                else if (parts[0] == "f" && parts.Length >= 4)
                {
                    // Parsear os índices de cada vértice da face
                    List<int> faceVertices = new List<int>();
                    List<int> faceTexCoords = new List<int>();
                    List<int> faceNormals = new List<int>();

                    for (int i = 1; i < parts.Length; i++)
                    {
                        ParseFaceVertex(parts[i], out int vIdx, out int vtIdx, out int vnIdx);
                        faceVertices.Add(vIdx);
                        faceTexCoords.Add(vtIdx);
                        faceNormals.Add(vnIdx);
                    }

                    // Triangulação: se a face tem mais de 3 vértices,
                    // dividimos em triângulos usando "fan triangulation"
                    //
                    //   V0 --- V3        V0-V1-V2  e  V0-V2-V3
                    //   |  \    |
                    //   |   \   |
                    //   |    \  |
                    //   V1 --- V2
                    //
                    for (int i = 1; i < faceVertices.Count - 1; i++)
                    {
                        int idx0 = faceVertices[0];
                        int idx1 = faceVertices[i];
                        int idx2 = faceVertices[i + 1];

                        Vector3 v0 = vertices[idx0];
                        Vector3 v1 = vertices[idx1];
                        Vector3 v2 = vertices[idx2];

                        bool hasNormals = faceNormals[0] >= 0 && faceNormals[i] >= 0
                                          && faceNormals[i + 1] >= 0 && normals.Count > 0;

                        bool hasUVs = faceTexCoords[0] >= 0 && faceTexCoords[i] >= 0
                                      && faceTexCoords[i + 1] >= 0 && texCoords.Count > 0;

                        Triangle tri;

                        if (hasNormals && hasUVs)
                        {
                            // Smooth shading + texturas
                            tri = new Triangle(v0, v1, v2,
                                normals[faceNormals[0]], normals[faceNormals[i]], normals[faceNormals[i + 1]],
                                texCoords[faceTexCoords[0]], texCoords[faceTexCoords[i]], texCoords[faceTexCoords[i + 1]],
                                material);
                        }
                        else if (hasNormals)
                        {
                            // Smooth shading, sem textura
                            tri = new Triangle(v0, v1, v2,
                                normals[faceNormals[0]], normals[faceNormals[i]], normals[faceNormals[i + 1]],
                                material);
                        }
                        else
                        {
                            // Flat shading, sem textura
                            tri = new Triangle(v0, v1, v2, material);
                        }

                        mesh.AddTriangle(tri);
                    }
                }
                // Linhas como 'mtllib', 'usemtl', 's', 'o', 'g' são ignoradas
            }

            Console.WriteLine($"OBJ carregado: {vertices.Count} vértices, " +
                              $"{texCoords.Count} UVs, " +
                              $"{normals.Count} normais, {mesh.Triangles.Count} triângulos");

            return mesh;
        }

        /// <summary>
        /// Parseia um vértice de face nos formatos:
        ///   "1"        → só vértice
        ///   "1//1"     → vértice//normal
        ///   "1/1/1"    → vértice/textura/normal
        ///   "1/1"      → vértice/textura (sem normal)
        ///
        /// Retorna índices 0-based (OBJ é 1-based, então subtraímos 1).
        /// Retorna -1 para vtIdx/vnIdx se não houver textura/normal.
        /// </summary>
        private static void ParseFaceVertex(string token, out int vIdx, out int vtIdx, out int vnIdx)
        {
            vnIdx = -1;
            vtIdx = -1;

            if (token.Contains("//"))
            {
                // formato: v//vn
                string[] split = token.Split("//");
                vIdx = int.Parse(split[0]) - 1;   // 1-based → 0-based
                vnIdx = int.Parse(split[1]) - 1;
            }
            else if (token.Contains("/"))
            {
                // formato: v/vt ou v/vt/vn
                string[] split = token.Split('/');
                vIdx = int.Parse(split[0]) - 1;
                if (split.Length >= 2 && !string.IsNullOrEmpty(split[1]))
                    vtIdx = int.Parse(split[1]) - 1;
                if (split.Length >= 3 && !string.IsNullOrEmpty(split[2]))
                    vnIdx = int.Parse(split[2]) - 1;
            }
            else
            {
                // formato: v (só o índice do vértice)
                vIdx = int.Parse(token) - 1;
            }
        }
    }
}
