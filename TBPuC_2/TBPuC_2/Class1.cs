
using System;
using System.ServiceModel;
using System.Threading;

namespace SecondLabOnWCF
{
//    public class Server
//    {
//        #region public properties

//        public String Scheme
//        {
//            get
//            {
//                return "net.tcp";
//            }
//        }

//        public Int32 Port
//        {
//            get
//            {
//                return 22145;
//            }
//        }

//        public String Host
//        {
//            get
//            {
//                return "localhost";
//                /*return Dns.GetHostEntry(Dns.GetHostName()).AddressList[0];*/
//                /*return Dns.GetHostName();*/
//            }
//        }

//        #endregion

//        #region public methods

//        public void StartServer()
//        {
//            CloseServer();

//            Thread thread = new Thread(StartServerInternal);
//            thread.IsBackground = true;
//            thread.Name = "Start server thread";
//            thread.Start();
//        }

//        public void CloseServer()
//        {
//            if (_serviceHost != null)
//                _serviceHost.Close();
//        }

//        #endregion

//        #region private methods

//        private void StartServerInternal()
//        {
//            UriBuilder builder = new UriBuilder(Scheme, Host, Port);
//            /*String uri = String.Format("net.tcp://{0}:22145",
//                                       Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]);*/
//            _serviceHost = new ServiceHost(
//                typeof(CalculatingService),
//                /*ew Uri(uri)*/
//                builder.Uri
//                );

//            _serviceHost.AddServiceEndpoint(
//                typeof(ICalculatingService),
//                /*new BasicHttpBinding(),*/
//                Settings.GetBinding(),
//                "");

//            /*
//            ServiceMetadataBehavior behavior = new
//                ServiceMetadataBehavior();
//            behavior.HttpGetEnabled = true;

//            _serviceHost.Description.Behaviors.Add(behavior);
//            _serviceHost.AddServiceEndpoint(
//            typeof(IMetadataExchange),
//                MetadataExchangeBindings.CreateMexHttpBinding(),
//                "mex");
//             */

//            _serviceHost.Open();
//        }

//        #endregion

//        /*public const String UriString = "http://localhost:22145/TVP";*/
//        private ServiceHost _serviceHost;
//    }
}
