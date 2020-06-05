using Energistics;
using Energistics.Protocol.Discovery;
using Microsoft.Win32;
using MyPDSV0._9.Porxies;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
namespace MyPDSV0._9
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public EtpClient Client { set; get; }
        public CancellationTokenSource EtpClientTokenSource { get; private set; }
        public EtpChannelStreamingProxy EtpClientProxy { get; private set; }
        private string StreamingJasonFileName { get; set; }

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
                ETPM_TextBox.ScrollToEnd();
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
                    Dispatcher.Invoke(new Action(() => { FileIn_TextBox.Text = dialog.FileName; }));
                    StreamingJasonFileName = dialog.FileName;
                    Streaming.IsEnabled = true;
                }

                catch (Exception ex)
                {
                    LogClientOutput("Error opening file: " + ex.ToString());
                }
            }

        }

        private void Streaming_Click(object sender, RoutedEventArgs e)
        {
            var json = File.ReadAllText(StreamingJasonFileName);
            var model = JsonConvert.DeserializeObject<Models.Simulation>(json);
            System.Windows.MessageBox.Show(model.Interval.ToString());
            EtpClientProxy = new EtpChannelStreamingProxy(model.EtpVersion, LogClientOutput);
            EtpClientTokenSource = new CancellationTokenSource();
            var token = EtpClientTokenSource.Token;

            Task.Run(async () =>
            {
                using (EtpClientTokenSource)
                {
                    try
                    {
                        LogClientOutput($"ETP Client simulation starting. URL:{model.EtpConnection.Uri}");
                        await EtpClientProxy.Start(model, token);
                        LogClientOutput("ETP Client simulation stopped.");
                    }
                    catch (Exception ex)
                    {
                        LogClientOutput("An Error occurred: " + ex.ToString());
                    }
                    finally
                    {
                        EtpClientTokenSource = null;
                    }

                }

            }, token);
        }

        private void StreamingStop_Click(object sender, RoutedEventArgs e)
        {
            EtpClientTokenSource.Cancel();
        }
    }
    }

