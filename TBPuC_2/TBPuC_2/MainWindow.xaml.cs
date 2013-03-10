using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
using System.ServiceModel;
using System.Threading;
using System.ServiceModel.Description;
using time = System.Timers;

namespace Service
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class Setting
        {
            public static IPAddress IPAdress = Dns.GetHostByName(Dns.GetHostName()).AddressList[0];

            public static int port = 2013;
        }

        time.Timer timer = new time.Timer(15 * 1000);

        ServerBroadcast broadcast = new ServerBroadcast();

        AutoResetEvent eventClose = new AutoResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            new Thread(() => Host()).Start();
            btGo.IsEnabled = false;
            btStop.IsEnabled = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectionInfo info = new ConnectionInfo(false, Setting.IPAdress, Setting.port);
            MainGrid.DataContext = info;
            broadcast.NotifyClients();
            timer.Elapsed += timer_Elapsed;
        }

        private void Host()
        {
            var binding = new NetTcpBinding();
            binding.MaxBufferSize = Int32.MaxValue;
            binding.MaxReceivedMessageSize = Int32.MaxValue;
            binding.Security.Mode = SecurityMode.None;
            ServiceHost host = new ServiceHost(typeof(ServiceContract));
            host.AddServiceEndpoint(typeof(IServiceContract), binding, string.Format("net.tcp://{0}:{1}/", Setting.IPAdress, Setting.port));
            host.Open();

            Connect();//Изменить статус в граф. интерфейсе
            eventClose.WaitOne();//Ожидание закрытия окна
            host.Close();
        }

        void Connect()
        {
            Dispatcher.Invoke(() =>
                {
                    (MainGrid.DataContext as ConnectionInfo).ToConnect();
                    var inf = MainGrid.DataContext; 
                    MainGrid.DataContext = null;
                    MainGrid.DataContext = inf;
                });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            eventClose.Set();
        }

        private void Notify(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            broadcast.NotifyClients();
            timer.Start();
        }

        private void timer_Elapsed(object sender, time.ElapsedEventArgs e)
        {
            broadcast.NotifyClients();
            timer.Start();
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            Connect();
            eventClose.Set();
            btGo.IsEnabled = true;
            btStop.IsEnabled = false;
        }
    }
}
