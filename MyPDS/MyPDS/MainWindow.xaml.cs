﻿using Avro.Specific;
using Energistics;
using Energistics.Common;
using Energistics.Protocol.Core;
using Energistics.Protocol.Discovery;
using Microsoft.Win32;
using Newtonsoft.Json;
using PDS.WITSMLstudio.Desktop.Core.Runtime;
using PDS.WITSMLstudio.Framework;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
        public CancellationTokenSource EtpClientTokenSource { get; private set; }
        public Proxies.EtpChannelStreamingProxy EtpClientProxy { get; private set; }
        public IRuntimeService Runtime { get; }
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

            Dispatcher.Invoke(new Action(() => { this.Details.AppendText(string.Concat(
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
                    EtpClientProxy = new Proxies.EtpChannelStreamingProxy(Runtime, model.EtpVersion, Log);
                    EtpClientTokenSource = new CancellationTokenSource();
                    var token = EtpClientTokenSource.Token;

                    Task.Run(async () =>
                    {
                        using (EtpClientTokenSource)
                        {
                            try
                            {
                                Log("ETP Client simulation starting. URL: {0}", model.EtpConnection.Uri);
                                await EtpClientProxy.Start(model, token);
                                Log("ETP Client simulation stopped.");
                            }
                            catch (Exception ex)
                            {
                                Log("An error occurred: " + ex);
                            }
                            finally
                            {
                                EtpClientTokenSource = null;
                            }
                        }
                    },
           token);
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

        private void Log(string message, params object[] values)
        {
            Log(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff - ") + string.Format(message, values));
        }

        private void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return;

            string showmsg = string.Concat(
                message.StartsWith("{") ? string.Empty : "// ",
                message,
                Environment.NewLine);

            this.Details.Dispatcher.Invoke(new Action(() =>
            {
                this.Details.AppendText(showmsg);
            }));
        }
    }
}

