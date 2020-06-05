using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace WpfApp_WitsServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private Communication comm;
        private Socket _client;
        public MainWindow()
        {
            InitializeComponent();
            comm = new Communication("127.0.0.1", 6699);
            
        }

        private void SendBtn_Click(object sender, RoutedEventArgs e)
        {
            
            //string str = "&&\r\n08011000.88\r\n0802899.78\r\n080323.59\r\n!!\r\n";
            DataSimu dataSimu = new DataSimu();
            string str = dataSimu.Simu();
            
            comm.SndData(str);
        }

        private void ConnBtn_Click(object sender, RoutedEventArgs e)
        {
            _client = comm.BindListenAccept();
            IPEndPoint ipEndClient = (IPEndPoint)_client.RemoteEndPoint;
            ResBox.AppendText($"Connect with {ipEndClient.Address} at port {ipEndClient.Port}");

        }
    }
}
