using Energistics;
using Energistics.Protocol.Discovery;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace MyPDSV0._9
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public EtpClient Client { set; get; }
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Connect2Server_Button_Click(object sender, RoutedEventArgs e)
        {
            Client = new EtpClient(ServerUri_TextBox.Text, AppName_TextBox.Text, Version_TextBox.Text);
            Client.Register<IDiscoveryCustomer, DiscoveryCustomerHandler>();
            Client.Output = LogClientOutput;
            Client.Open();
            
            //System.Windows.MessageBox.Show(Proto_ListBox.SelectedIndex.ToString());
            //foreach (var item in Proto_ListBox.SelectedItems)
            //{
            //    System.Windows.MessageBox.Show(item.ToString());
            //}

        }

        internal void LogClientOutput(string message)
        {
            LogClientOutput(message, false);
        }

        internal void LogClientOutput(string message, bool logDetails)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (logDetails)
                LogDetailMessage(message);  

            Dispatcher.Invoke(new Action(() => {
                ETPM_TextBox.AppendText(message);
            }));

            
        }

        internal void LogDetailMessage(string message)
        {
            
        }

        private void GetUri_Button_Click(object sender, RoutedEventArgs e)
        {
            var DisHandler = Client.Handler<IDiscoveryCustomer>();
            DisHandler.GetResources(SearchUri_TextBox.Text);
        }
    }
}
