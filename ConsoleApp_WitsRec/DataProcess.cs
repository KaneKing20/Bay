using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp_WitsRec
{
    class DataProcess
    {
        public LogData Data { set; get; } = new LogData();
        public List<LogData> LogDatas { set; get; } = new List<LogData>();

        public void Parse(string logString)
        {
            string[] splitStrs = logString.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var str in splitStrs)
            {
                if (str != "&&" && str != "!!") {
                    Data = new LogData();
                    Data.ChartNo = str.Substring(0, 2);
                    Data.ChannelNo = str.Substring(2, 2);
                    Data.DataValue = str.Substring(4, str.Length - 4);
                    LogDatas.Add(Data);
                }
            }
            
        }
        public void Show()
        {
            Console.WriteLine("Chart \tChannel\tValue");
            foreach (var item in LogDatas)
            {
                Console.WriteLine($"{item.ChartNo}\t{item.ChannelNo}\t{item.DataValue}");
            }
            LogDatas.Clear();
        }
    }
}
