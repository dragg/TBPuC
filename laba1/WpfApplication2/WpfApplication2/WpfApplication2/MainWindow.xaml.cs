using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

namespace WpfApplication2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public class inf
        {
            private int count;
            public int Count { get { return count; } set { count = value; } }
            private int size;
            public int Size { get { return size; } set { size = value; } }
            public inf(int count, int size)
            {
                Count = count;
                Size = size;
            }
        }

        object sync = new object();
        static private int N = 30;
        private int workingThread;
        static private int CountThread = 100;
        TimeSpan[] arrSpan = new TimeSpan[CountThread];

        AutoResetEvent EndThreads = new AutoResetEvent(false);
        ManualResetEvent GoThreads = new ManualResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void CreateAndWork(int n)
        {
            Matrix matrix1 = new Matrix(n);
            Matrix matrix2 = new Matrix(n);
            int count = 0;
            Dispatcher.Invoke(() =>
            {
                count = (MainGrid.DataContext as inf).Count;
            });
            arrSpan = new TimeSpan[count];
            for (int i = 1; i <= count; i++)//i - количество потоков
            {
                GoThreads.Reset();//сбросили события для старта всех нитей
                workingThread = i;
                int elemOnThread = n * n / i;
                int residue = n * n % i;
                int position = 0;
                //----------------------------------создание и запуск нитей-------------------------
                Thread[] masThread = new Thread[i];
                for (int j = 0; j < i; j++)
                {
                    masThread[j] = new Thread(CalcThread);
                    Parametrs par = new Parametrs(matrix1, matrix2, position, (residue == 0 ? elemOnThread : (elemOnThread + 1)));
                    if (residue != 0)
                    {
                        residue--;
                    }
                    position += (residue == 0 ? elemOnThread : (elemOnThread + 1));
                    masThread[j].Start(par);
                }
                //----------------------------------------------------------------------------------

                DateTime timeStart = DateTime.Now;
                GoThreads.Set();//запуск всех нитей

                EndThreads.WaitOne();//ожидание завершение подсчетов
                DateTime timeEnd = DateTime.Now;
                arrSpan[i - 1] = timeEnd - timeStart;
            }
        }

        void CalcThread(object obj)
        {
            GoThreads.WaitOne();//ожидаем старта работы

            (obj as Parametrs).matrix1.Mult((obj as Parametrs).matrix2, (obj as Parametrs).position, (obj as Parametrs).count);

            lock (sync)
            {
                workingThread--;
                if (workingThread == 0)
                {
                    EndThreads.Set();
                }
            }
        }

        private void GoCalc(object sender, RoutedEventArgs e)
        {
            Thread th = new Thread(delegate()
            {
                int n = 100;
                Dispatcher.Invoke(() =>
                    {
                        btGo.IsEnabled = false;
                        n = (MainGrid.DataContext as inf).Size;
                    });
                CreateAndWork(n);
            });
            th.Start();
            Thread draw = new Thread(delegate()
                {
                    th.Join();
                    ObservableCollection<TimePoint> timeChart = new ObservableCollection<TimePoint> { };
                    for (int i = 0; i < arrSpan.Length; i++)
                    {
                        timeChart.Add(new TimePoint { TimeSet = arrSpan[i].TotalMilliseconds, ThreadId = i });
                    }
                    Dispatcher.Invoke(() =>
                    {
                        ChartThread.ItemsSource = timeChart;
                        btGo.IsEnabled = true;
                    });
                });
            draw.Start();
        }

        private void Window_Loaded_1(object sender, RoutedEventArgs e)
        {
            inf inf = new inf(CountThread, N);
            MainGrid.DataContext = inf;
        }
    }
}

