using Energistics;
using Energistics.Protocol.Discovery;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        private void Browser_Button_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Open Simulation Configuration Setting file...",
                Filter="Json|*.json;*.js|All Files|*.*"
            };
            if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                try
                {
                    ///<summary>
                    ///读取Jason文件，序列化为model类对象。
                    /// </summary>
                    var json = File.ReadAllText(dialog.FileName);
                    Dispatcher.Invoke(new Action(()=> {FileIn_TextBox.Text=dialog.FileName; ETPM_TextBox.AppendText(json); }));
                    var model = JsonConvert.DeserializeObject<Models.Simulation>(json);

                    ///<summary>
                    ///
                    /// </summary>
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }
    }
}
