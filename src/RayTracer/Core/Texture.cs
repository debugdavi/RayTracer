using StbImageSharp;
using System;
using System.IO;

namespace RayTracer.Core
{
    /// <summary>
    /// Carrega uma imagem e permite consultar a cor em coordenadas UV.
    /// UV = (0,0) canto inferior-esquerdo, (1,1) canto superior-direito.
    /// </summary>
    public class Texture
    {
        private byte[] _pixels;
        private int _width;
        private int _height;
        private int _components;

        /// <summary>
        /// Carrega textura de um arquivo de imagem (PNG, JPG, BMP, TGA).
        /// </summary>
        public Texture(string filePath)
        {
            using (var stream = File.OpenRead(filePath))
            {
                var image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlue);
                _pixels = image.Data;
                _width = image.Width;
                _height = image.Height;
                _components = 3;
            }

            Console.WriteLine($"Textura carregada: {filePath} ({_width}x{_height})");
        }

        /// <summary>
        /// Construtor interno para texturas geradas proceduralmente.
        /// </summary>
        private Texture(byte[] pixels, int width, int height)
        {
            _pixels = pixels;
            _width = width;
            _height = height;
            _components = 3;
        }

        /// <summary>
        /// Gera uma textura checkerboard (xadrez) proceduralmente.
        /// Útil para testar se o mapeamento UV está funcionando.
        /// </summary>
        public static Texture Checkerboard(int size = 512, int squares = 8,
            Vector3? color1 = null, Vector3? color2 = null)
        {
            Vector3 c1 = color1 ?? new Vector3(0.9, 0.9, 0.9); // branco
            Vector3 c2 = color2 ?? new Vector3(0.15, 0.15, 0.15); // quase preto

            byte[] pixels = new byte[size * size * 3];
            int squareSize = size / squares;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool isLight = ((x / squareSize) + (y / squareSize)) % 2 == 0;
                    Vector3 c = isLight ? c1 : c2;

                    int idx = (y * size + x) * 3;
                    pixels[idx] = (byte)(c.X * 255);
                    pixels[idx + 1] = (byte)(c.Y * 255);
                    pixels[idx + 2] = (byte)(c.Z * 255);
                }
            }

            Console.WriteLine($"Textura checkerboard gerada: {size}x{size}, {squares}x{squares} quadrados");
            return new Texture(pixels, size, size);
        }

        /// <summary>
        /// Retorna a cor da textura na posição (u, v).
        /// u e v são valores entre 0 e 1.
        /// Usa wrapping: valores fora de [0,1] são "embrulhados" (repetição).
        /// </summary>
        public Vector3 Sample(double u, double v)
        {
            // Wrap UV para [0, 1]: 1.3 → 0.3, -0.2 → 0.8
            u = u - Math.Floor(u);
            v = v - Math.Floor(v);

            // Inverter V: imagens têm Y de cima pra baixo, UV tem Y de baixo pra cima
            v = 1.0 - v;

            // Converter UV para coordenadas de pixel
            int x = (int)(u * (_width - 1));
            int y = (int)(v * (_height - 1));

            // Garantir limites
            x = Math.Clamp(x, 0, _width - 1);
            y = Math.Clamp(y, 0, _height - 1);

            // Buscar pixel no array (RGB, 3 bytes por pixel)
            int index = (y * _width + x) * _components;

            double r = _pixels[index] / 255.0;
            double g = _pixels[index + 1] / 255.0;
            double b = _pixels[index + 2] / 255.0;

            return new Vector3(r, g, b);
        }
    }
}
