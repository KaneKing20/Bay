using Avro.Specific;
using Energistics;
using Energistics.Common;
using Energistics.Datatypes;
using Energistics.Protocol.ChannelStreaming;
using Energistics.Protocol.Core;
using Energistics.Protocol.Discovery;
using Energistics.Protocol.Store;
using Microsoft.Win32;
using MyPDS.Models;
using Newtonsoft.Json;
using PDS.WITSMLstudio.Desktop.Core.Connections;
using PDS.WITSMLstudio.Desktop.Core.ViewModels;
using PDS.WITSMLstudio.Framework;
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

namespace MyPDS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ServerUrl = "ws://39.105.96.31/witsml.web/api/etp";
        private const string AppName = "EtpClientTests";
        private const string AppVersion = "1.0";
        private const string UserName = "";
        private const string Password = " ";
        private const string EtpSubProtocol = "energistics-tp";
        public EtpClient Client { set; get; }
       // public EtpSettings Model { get; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {          
            Client = new EtpClient(ServerUrl, "MyPDS", "1.0.0.0");
            Client.Register<IDiscoveryCustomer, DiscoveryCustomerHandler>();
            
            RegisterEventHandlers(Client.Handler<IDiscoveryCustomer>(),
                x => x.OnGetResourcesResponse += OnGetResourcesResponse);

            Client.Output = LogClientOutput;
            Client.Open();

            
            //System.Windows.MessageBox.Show(Client.IsOpen.ToString());
        }

        private void OnGetResourcesResponse(object sender, ProtocolEventArgs<GetResourcesResponse, string> e)
        {
            LogObjectDetails(e);
        }

        private void LogObjectDetails<T>(ProtocolEventArgs<T> e) where T : ISpecificRecord
        {
            //Details.SetText();
            this.Details.Dispatcher.Invoke(new Action(() => { this.Details.AppendText(string.Format(
                "// Header:{2}{0}{2}{2}// Body:{2}{1}{2}",
                Client.Serialize(e.Header, true),
                Client.Serialize(e.Message, true),
                Environment.NewLine)); }));
        }

        internal void LogClientOutput(string message)
        {
            LogClientOutput(message, false);
        }

        /// <summary>
        /// Logs the client output.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="logDetails">if set to <c>true</c> logs the detail message.</param>
        internal void LogClientOutput(string message, bool logDetails)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            if (logDetails)
                LogDetailMessage(message);

            this.Details.Dispatcher.Invoke(new Action(() => { this.Details.AppendText(string.Concat(
                message.StartsWith("{") ? string.Empty : "// ",
                message,
                Environment.NewLine)); }));
           
        }
        internal void LogDetailMessage(string header, string message = null)
        {
            Details.AppendText(string.Concat(
                header.StartsWith("{") ? string.Empty : "// ",
                header,
                Environment.NewLine));

            if (string.IsNullOrWhiteSpace(message))
                return;

            Details.AppendText(string.Concat(
                message.StartsWith("{") ? string.Empty : "// ",
                message,
                Environment.NewLine));
        }

        private THandler RegisterEventHandlers<THandler>(THandler handler, params Action<THandler>[] actions) where THandler : IProtocolHandler
        {
            handler.OnAcknowledge += OnAcknowledge;
            handler.OnProtocolException += OnProtocolException;
            actions.ForEach(action => action(handler));
            return handler;
        }

        private void OnAcknowledge(object sender, ProtocolEventArgs<Acknowledge> e)
        {
            LogObjectDetails(e);
        }

        private void OnProtocolException(object sender, ProtocolEventArgs<ProtocolException> e)
        {
            LogObjectDetails(e);
        }

      
        private void GetUriBtn_Click(object sender, RoutedEventArgs e)
        {
            var DisHandler = Client.Handler<IDiscoveryCustomer>();
            DisHandler.GetResources("eml://");
                
        }

        private void CleanBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Details.Dispatcher.Invoke(new Action(() => this.Details.Clear()));
        }

        private void ReadJason_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog()
            {
                Title = "Open Simulation Configuration Settings file...",
                Filter = "JSON Files|*.json;*.js|All Files|*.*"
            };
            if (dialog.ShowDialog(Application.Current.MainWindow).GetValueOrDefault())
            {
                try
                {
                    var json = File.ReadAllText(dialog.FileName);
                    var model = JsonConvert.DeserializeObject<Models.Simulation>(json);

                    //var viewModel = new SimulationViewModel(Runtime)
                    //{
                    //    Model = model,
                    //    DisplayName = model.Name
                    //};

                    //ActivateItem(viewModel);
                }
                catch (Exception ex)
                {
                    //Runtime.ShowError("Error opening file.", ex);
                    System.Windows.MessageBox.Show("Error opening file.", ex.ToString());
                }
            }
        }
    }
}

