using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [Serializable]
    public class Matrix
    {
        public int[,] matrix;
        int n;
        Random rnd = new Random();

        public Matrix(int n)
        {
            this.n = n;
            matrix = new int[n, n];
            init();
        }

        private void init()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    matrix[i, j] = rnd.Next();
                }
            }
        }

        public int[] Mult(Matrix m, int position, int count)
        {
            int[] c = new int[count];
            int i = position / n;
            int j = position % n;
            for (int k = 0; k < count; k++)
            {
                for (int r = 0; r < n; r++)
                {
                    c[k] += matrix[i, r] * m.matrix[r, j];
                }
                if (j == n - 1)
                {
                    j = 0;
                    ++i;
                }
                else
                {
                    ++j;
                }
            }
            return c;
        }
    }
}
