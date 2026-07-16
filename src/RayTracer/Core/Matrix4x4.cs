using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RayTracer.Core
{
    public class Matrix4x4
    {
        public double[,] M { get; private set; }
        public Matrix4x4()
        {
            M = new double[4, 4];
        }

        public Matrix4x4(double[,] values)
        {
            M = values;
        }
        public static Matrix4x4 Identity()
        {
            Matrix4x4 m = new Matrix4x4();
            m.M[0, 0] = 1; m.M[1, 1] = 1; m.M[2, 2] = 1; m.M[3, 3] = 1;
            return m;
        }

        public static Matrix4x4 operator *(Matrix4x4 a, Matrix4x4 b)
        {
            Matrix4x4 result = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    for (int k = 0; k < 4; k++)
                        result.M[i, j] += a.M[i, k] * b.M[k, j];
            return result;
        }

        public Vector3 TransformPoint(Vector3 p)
        {
            double x = M[0, 0] * p.X + M[0, 1] * p.Y + M[0, 2] * p.Z + M[0, 3];
            double y = M[1, 0] * p.X + M[1, 1] * p.Y + M[1, 2] * p.Z + M[1, 3];
            double z = M[2, 0] * p.X + M[2, 1] * p.Y + M[2, 2] * p.Z + M[2, 3];
            return new Vector3(x, y, z);
        }

        public Vector3 TransformDirection(Vector3 d)
        {
            double x = M[0, 0] * d.X + M[0, 1] * d.Y + M[0, 2] * d.Z;
            double y = M[1, 0] * d.X + M[1, 1] * d.Y + M[1, 2] * d.Z;
            double z = M[2, 0] * d.X + M[2, 1] * d.Y + M[2, 2] * d.Z;
            return new Vector3(x, y, z);
        }

        public Vector3 TransformNormal(Vector3 n)
        {
            // Usamos as COLUNAS da inversa (= linhas da transposta da inversa)
            // como se fosse TransformDirection da transposta
            double x = M[0, 0] * n.X + M[1, 0] * n.Y + M[2, 0] * n.Z;
            double y = M[0, 1] * n.X + M[1, 1] * n.Y + M[2, 1] * n.Z;
            double z = M[0, 2] * n.X + M[1, 2] * n.Y + M[2, 2] * n.Z;
            return new Vector3(x, y, z).Normalize();
        }

        public Matrix4x4 Transpose()
        {
            Matrix4x4 t = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    t.M[i, j] = M[j, i];
            return t;
        }

        public Matrix4x4 Inverse()
        {
            // Cria matriz aumentada [M | I]
            double[,] aug = new double[4, 8];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    aug[i, j] = M[i, j];
                    aug[i, j + 4] = (i == j) ? 1.0 : 0.0;
                }
            }

            // Eliminação de Gauss-Jordan
            for (int col = 0; col < 4; col++)
            {
                // Pivotamento parcial: encontra a maior entrada na coluna
                int maxRow = col;
                for (int row = col + 1; row < 4; row++)
                {
                    if (Math.Abs(aug[row, col]) > Math.Abs(aug[maxRow, col]))
                        maxRow = row;
                }

                // Troca linhas
                if (maxRow != col)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        double temp = aug[col, j];
                        aug[col, j] = aug[maxRow, j];
                        aug[maxRow, j] = temp;
                    }
                }

                double pivot = aug[col, col];
                if (Math.Abs(pivot) < 1e-10)
                    return Identity(); // Matriz singular, retorna identidade

                // Normaliza a linha do pivô
                for (int j = 0; j < 8; j++)
                    aug[col, j] /= pivot;

                // Elimina as outras linhas
                for (int row = 0; row < 4; row++)
                {
                    if (row != col)
                    {
                        double factor = aug[row, col];
                        for (int j = 0; j < 8; j++)
                            aug[row, j] -= factor * aug[col, j];
                    }
                }
            }

            // Extrai a inversa da parte direita
            Matrix4x4 inv = new Matrix4x4();
            for (int i = 0; i < 4; i++)
                for (int j = 0; j < 4; j++)
                    inv.M[i, j] = aug[i, j + 4];

            return inv;
        }
    }
}