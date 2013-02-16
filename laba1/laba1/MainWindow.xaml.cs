using System;
using System.Collections.Generic;
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

namespace laba1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        object sync = new object();
        static private int N = 100;
        private int workingThread;
        TimeSpan[] arrSpan = new TimeSpan[N];

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

            for (int i = 1; i <= 300; i++)//i - количество потоков
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
                arrSpan[i-1] = timeEnd - timeStart;
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
            CreateAndWork(N);
        }
    }
}
