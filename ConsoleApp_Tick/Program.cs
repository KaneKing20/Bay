using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleApp_Tick
{
    class Program
    {
        static void Main(string[] args)
        {
            Timer timer = new Timer(2000);
            timer.Elapsed += Timer_Elapsed;
            //timer.AutoReset = true;
            timer.Start();
            Console.ReadKey();
        }

        private static void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Console.WriteLine("Timer ring....");
        }
    }
}
