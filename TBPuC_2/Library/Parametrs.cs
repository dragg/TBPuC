using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    public class Parametrs
    {
        public int position { get; set; }
        public int count { get; set; }
        public Matrix matrix1 { get; set; }
        public Matrix matrix2 { get; set; }
        public IPEndPoint ip { get; set; }
        public int index { get; set; }

        public Parametrs(Matrix m1, Matrix m2, int pos, int count, int i, IPEndPoint point)
        {
            matrix1 = m1;
            matrix2 = m2;
            position = pos;
            this.count = count;
            index = i;
            ip = point;
        }
    }
}
