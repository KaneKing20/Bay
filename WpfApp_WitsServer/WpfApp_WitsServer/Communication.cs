using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_WitsServer
{
    class Communication
    {
        private IPAddress _ip;
        public IPAddress IP { get { return _ip; } set { _ip = value; } }

        private int _port;
        public int Port { get { return _port; } set { _port = value; } }

        private string _logString;
        public string LogString { get { return _logString; } set { _logString = value; } }

        private IPEndPoint _ipe;
        private Socket _socket;

        public IPEndPoint ClientEndPoint { set; get; }
        public Socket ClientSocket { set; get; }

        private DataSimu dataSimu ;
        //private int index = 0;
        public Communication(string ip, int port)
        {
            IP = IPAddress.Parse(ip);
            Port = port;
            _ipe = new IPEndPoint(IP, Port);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _socket.Bind(_ipe);
            _socket.Listen(15);
            dataSimu = new DataSimu();
        }

        public void Listen(int backlog =15)
        {
            
           // _clientSocket.Listen(backlog);
            //return _clientSocket.Accept();
        }

        public void Accept()
        {
            ClientSocket =  _socket.Accept();
            ClientEndPoint = (IPEndPoint)ClientSocket.RemoteEndPoint;
        }

        //public void ConnectToServer()
        //{
        //    try
        //    {
        //        _clientSocket.Connect(_ipe);
        //        Console.WriteLine("Connected to {0} @ Port:{1}.", IP.ToString(), Port.ToString());
        //    }
        //    catch (SocketException e)
        //    {
        //        Console.WriteLine("Failed to Connect Server -{0}", e.ToString());
        //        return;
        //    }
        //}

        public void SndData(WitsConfig witsConfig)
        {
           
                string str = dataSimu.Simu(witsConfig);//每次取las文件中下一行的数据
                byte[] sndByte = System.Text.Encoding.Default.GetBytes(str);
                ClientSocket.Send(sndByte);

            
        }

        //public string RecData()
        //{
        //    byte[] recByte = new byte[4096];
        //    int bytes = 0;
        //    try
        //    {
        //        bytes = _clientSocket.Receive(recByte, recByte.Length, 0);
        //    }
        //    catch (Exception e)
        //    {

        //        Console.WriteLine(e.ToString());
        //    }

        //    LogString = System.Text.Encoding.Default.GetString(recByte, 0, bytes);
        //    return LogString;
        //}

        //public void Close()
        //{
        //    _clientSocket.Close();
        //    Console.WriteLine("Close.");
        //}
    }
}
