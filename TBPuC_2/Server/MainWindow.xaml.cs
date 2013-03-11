using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Library;
using Service;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerBroadcast broadcast = new ServerBroadcast();
        private List<IPEndPoint> services = new List<IPEndPoint>();
        //private List<TimeSpan> interval;//Интервалы времени, потраченные для расчетов на каждой подключенной машине
        private ManualResetEvent eventStart = new ManualResetEvent(false);
        private AutoResetEvent eventEnd = new AutoResetEvent(false);
        private object sync = new object();
        private InterfaceInfo inf = new InterfaceInfo(100);
        private AutoResetEvent closeWin = new AutoResetEvent(false);
        private System.Timers.Timer timerNotify = new System.Timers.Timer(3 * 1000);

        private Dictionary<IPEndPoint, TimeSpan> dicTimeSpan = new Dictionary<IPEndPoint, TimeSpan>();

        private Library.Matrix m1;
        private Library.Matrix m2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            btGo.IsEnabled = false;
            new Thread(() =>
                {
                    int size = inf.Size;
                    m1 = new Library.Matrix(size);
                    m2 = new Library.Matrix(size);

                    dicTimeSpan = new Dictionary<IPEndPoint, TimeSpan>();
                    foreach (var item in broadcast.AvailableEndPoints)
                    {
                        dicTimeSpan.Add(item, new TimeSpan());
                    }

                    Split(0, size * size, true);
                }).Start();
        }

        void Split(int begin, int count, bool first = false)
        {
            List<Thread> listThread = new List<Thread>();
            int workingPC = dicTimeSpan.Count;
            int elemOnPC = (count - begin) / workingPC;
            int residue = (count - begin) % workingPC;
            int position = begin;

            for (int i = 0; i < dicTimeSpan.Count; i++)
            {
                Parametrs param = new Parametrs(m1, m2, position, (residue == 0 ? elemOnPC : (elemOnPC + 1)), i, dicTimeSpan.Keys.ToArray()[i]);
                if (residue != 0)
                {
                    residue--;
                }
                position += (residue == 0 ? elemOnPC : (elemOnPC + 1));
                Thread th = new Thread(Calc);
                listThread.Add(th);
                th.Start(param);
            }

            foreach (var item in listThread)
            {
                item.Join();
            }
            if (first)
            {
                Dispatcher.Invoke(() =>
                {
                    btGo.IsEnabled = true;
                });
            }
        }

        private void Calc(object obj)
        {
            var par = obj as Parametrs;
            
            var binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.Security.Mode = SecurityMode.None;
            ServiceEndpoint se = new ServiceEndpoint(ContractDescription.GetContract(typeof(IServiceContract)),
                binding, new EndpointAddress(string.Format("net.tcp://{0}:{1}/", par.ip.Address, 2013)));
            ChannelFactory<IServiceContract> fac = new ChannelFactory<IServiceContract>(se);
            try
            {
                fac.Open();
                IServiceContract proxy = fac.CreateChannel();
                DateTime begin = DateTime.Now;//Фиксируем начальное время
                proxy.Mult(m1, m2, par.position, par.count);
                dicTimeSpan[par.ip] = DateTime.Now - begin;
            }
            catch (Exception ex)
            {
                broadcast.AvailableEndPoints.RemoveAll(u => u.Address == par.ip.Address);
                dicTimeSpan.Remove(par.ip);

                Split(par.position, par.count);//Разделяем упущенное задание на оставшиеся машины
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timerNotify.Elapsed += timerNotify_Elapsed;
            timerNotify.Start();
            broadcast.StartListeningServers();
            gridMain.DataContext = inf;
            new Thread(() =>
                {
                    Host();
                }).Start();
        }

        void timerNotify_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            timerNotify.Stop();
            broadcast.NotifyClients();//Уведомляем сами себя о том, что готовы работать! =)
            timerNotify.Start();
        }

        private void Host()
        {
            var binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.Security.Mode = SecurityMode.None;
            ServiceHost host = new ServiceHost(typeof(ServiceContract));
            host.AddServiceEndpoint(typeof(IServiceContract), binding, string.Format("net.tcp://{0}:{1}/", "localhost", 2013));
            host.Open();
            closeWin.WaitOne();
            host.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            closeWin.Set();
        }
    }
}
