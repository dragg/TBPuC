using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Library
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    public interface IServiceContract
    {
        [OperationContract]
        int[] Mult(Matrix m1, Matrix m2, int begin, int count);

        [OperationContract]
        string Message();
    }
}
