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

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ServerBroadcast broadcast = new ServerBroadcast();
        List<IPEndPoint> services = new List<IPEndPoint>();
        List<TimeSpan> interval;
        ManualResetEvent eventStart = new ManualResetEvent(false);
        AutoResetEvent eventEnd = new AutoResetEvent(false);
        object sync = new object();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            //Открыть для каждого подключение и вызвать метод

            var endPoints = broadcast.AvailableEndPoints.ToArray();
            interval = new List<TimeSpan>(endPoints.Length);
            var binding = new NetTcpBinding();
            binding.Security.Mode = SecurityMode.None;

            bool[] flag = new bool[endPoints.Length];

            for (int i = 0; i < endPoints.Length; i++)
            {
                //new Thread(() =>
                    {
                        DateTime begin = DateTime.Now;
                        //eventStart.WaitOne();
                        ServiceEndpoint se = new ServiceEndpoint(ContractDescription.GetContract(typeof(IServiceContract)),
                            binding, new EndpointAddress(string.Format("net.tcp://{0}:{1}/", endPoints[i].Address, 2013)));
                        ChannelFactory<IServiceContract> fac = new ChannelFactory<IServiceContract>(se);
                        try
                        {
                            fac.Open();
                            IServiceContract proxy = fac.CreateChannel();
                            begin = DateTime.Now;//Фиксируем начальное время
                            MessageBox.Show(proxy.Message());
                            interval[i] = DateTime.Now - begin;//Считаем промежуток времени!
                        }
                        catch (Exception ex)
                        {
                            //interval[i] = begin - begin;//Чтобы показать, что ничего не отработано!
                            //broadcast.AvailableEndPoints.Remove(endPoints[i]);
                            broadcast.AvailableEndPoints.RemoveAll(u => u.Address == endPoints[i].Address);
                        }
                        flag[i] = true;
                        lock (sync)
                        {
                            bool tmp = true;
                            foreach (var item in flag)
                            {
                                if (item == true)
                                {
                                    continue;
                                }
                                tmp = false;
                                break;
                            }
                            if (tmp == true)
                            {
                                eventEnd.Set();//Подсчеты завершены!
                            }
                        }
                    }//).Start();
            }
            eventStart.Set();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            broadcast.StartListeningServers();
        }
    }
}
