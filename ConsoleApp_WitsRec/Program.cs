using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_WitsRec
{
    class Program
    {
        static void Main(string[] args)
        {
            Communication comm = new Communication("127.0.0.1", 6699);
            comm.ConnectToServer();
            
            DataProcess dataParse = new DataProcess();
            while (true)
            {               
                dataParse.Parse(comm.RecData());
                dataParse.Show();
            }
            
        }
    }
}
