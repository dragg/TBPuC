using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class ServiceContract : IServiceContract
    {
        public int[] Mult(Matrix m1, Matrix m2, int begin, int count)
        {
            return m1.Mult(m2, begin, count);
        }

        public string Message()
        {
            return string.Format("I am {0}", Dns.GetHostName());
        }
    }
}
