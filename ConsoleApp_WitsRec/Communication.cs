using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
namespace ConsoleApp_WitsRec
{
    class Communication
    {
        private IPAddress _ip;
        public IPAddress IP { get { return _ip; } set { _ip = value; } }

        private int  _port;
        public int Port { get { return _port; } set { _port = value; } }

        private string _logString;
        public string LogString { get { return _logString; } set { _logString = value; } }

        private IPEndPoint _ipe;
        private Socket _clientSocket;

        public Communication(string ip , int port) {
            IP = IPAddress.Parse(ip);
            Port = port;
            _ipe = new IPEndPoint(IP, Port);
            _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void ConnectToServer()
        {
            try
            {
                _clientSocket.Connect(_ipe);
                Console.WriteLine("Connected to {0} @ Port:{1}.",IP.ToString(),Port.ToString());
            }
            catch (SocketException e)
            {
                Console.WriteLine("Failed to Connect Server -{0}", e.ToString());
                return;
            }
        }

        public string RecData()
        {           
            byte[] recByte = new byte[4096];
            int bytes = 0;
            try
            {
                bytes = _clientSocket.Receive(recByte, recByte.Length, 0);
            }
            catch (Exception e)
            {

                Console.WriteLine(e.ToString());
            }

            LogString = System.Text.Encoding.Default.GetString(recByte, 0, bytes);           
            return LogString;            
        }

        public void Close()
        {
            _clientSocket.Close();
            Console.WriteLine("Close.");
        }

    }
}
