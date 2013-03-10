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
        private List<TimeSpan> interval;//Интервалы времени, потраченные для расчетов на каждой подключенной машине
        private ManualResetEvent eventStart = new ManualResetEvent(false);
        private AutoResetEvent eventEnd = new AutoResetEvent(false);
        private object sync = new object();
        private InterfaceInfo inf = new InterfaceInfo(100);
        private AutoResetEvent closeWin = new AutoResetEvent(false);
        private System.Timers.Timer timerNotify = new System.Timers.Timer(15 * 1000);

        private Library.Matrix m1;
        private Library.Matrix m2;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            int size = inf.Size;
            interval = new List<TimeSpan>(broadcast.AvailableEndPoints.Count);//Интервалов времени будет столько, сколько изначально компьютеров уведомили нас о своей работоспособности!

            Library.Matrix m1 = new Library.Matrix(size);
            Library.Matrix m2 = new Library.Matrix(size);

            //Split(0, size * size);//передавать считать не более 80*80 элементов!
        }

        void Split(int begin, int count)
        {
            var endPoints = broadcast.AvailableEndPoints.ToArray();
            
            for (int i = 0; i < endPoints.Length; i++)
            {
                Thread th = new Thread(Calc);
                th.Start();
            }
        }

        private void Calc(object obj)
        {
            var par = obj as Parametrs;
            DateTime begin;
            
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
                begin = DateTime.Now;//Фиксируем начальное время
                proxy.Mult(m1, m2, par.position, par.count);
                interval[par.index] += DateTime.Now - begin;//Считаем промежуток времени!
            }
            catch (Exception ex)
            {
                broadcast.AvailableEndPoints.RemoveAll(u => u.Address == par.ip.Address);
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
