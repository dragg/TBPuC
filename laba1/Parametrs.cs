using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace laba1
{
    public class Parametrs
    {
        public int position { get; set; }
        public int count { get; set; }
        public Matrix matrix1 { get; set; }
        public Matrix matrix2 { get; set; }

        public Parametrs(Matrix m1, Matrix m2, int pos, int count)
        {
            matrix1 = m1;
            matrix2 = m2;
            position = pos;
            this.count = count;
        }
    }
}
