using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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
using System.Timers;
using Timer = System.Timers.Timer;
using MahApps.Metro.Controls;
namespace WpfApp_WitsServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Communication comm;
        private Timer timer;
        private WitsConfig _witsConfig;
       // private Thread _thread;
        public MainWindow()
        {
            InitializeComponent();
            comm = new Communication("127.0.0.1", 6699);
            _witsConfig = new WitsConfig();
            Wits_DataGrid.ItemsSource = _witsConfig.WitsChart7;
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            //string str = "&&\r\n08011000.88\r\n0802899.78\r\n080323.59\r\n!!\r\n";
            timer.Start();          
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {           
            try
            {
                comm.SndData(_witsConfig);
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => { ResBox.AppendText(ex.ToString()); });
            }

        }

        private void ConnBtn_Click(object sender, RoutedEventArgs e)
        {
            //ResBox.AppendText("Listening....\n");
            comm.Accept();            
            Dispatcher.Invoke(() =>
            {
                ResBox.AppendText($"{comm.ClientEndPoint.Address.ToString()},{comm.ClientEndPoint.Port.ToString()}连接过来了");
            });
            //while (false)
            //{
            //    _client = comm.Accept();
            //    _thread = new Thread(DoWork);
            //    _thread.Start();
            //}

        }

        //private void DoWork()
        //{
        //    IPEndPoint _iPEndPoint = (IPEndPoint)_client.RemoteEndPoint;
        //    Dispatcher.Invoke(() =>
        //    {
        //        ResBox.AppendText($"{_iPEndPoint.Address.ToString()},{_iPEndPoint.Port.ToString()}连接过来了");
        //    });
           
        //}
    }
}
