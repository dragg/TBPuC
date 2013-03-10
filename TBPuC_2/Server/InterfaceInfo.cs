using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class InterfaceInfo
    {
        private int size;
        public int Size 
        {
            get { return size; }
            set { size = value; }
        }

        public InterfaceInfo(int size)
        {
            this.size = size;
        }
    }
}
