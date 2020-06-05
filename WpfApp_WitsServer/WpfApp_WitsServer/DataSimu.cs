using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_WitsServer
{
    class DataSimu
    {
        private StringBuilder _strbuilder;
        public StringBuilder Strbuilder { set { _strbuilder = value; } get { return _strbuilder; } }
        private LogData _logData;
        private Random random = new Random();
     
        public string Simu(WitsConfig witsConfig)
        {
            int dataNum = witsConfig.WitsChart7.Count();
            _logData = new LogData();
            Strbuilder = new StringBuilder();
            Strbuilder.Append("&&\r\n");

            for (int i = 0; i < dataNum; i++)
            {
                _logData.ChartNo = witsConfig.WitsChart7[i].RecordID;
                _logData.ChannelNo = witsConfig.WitsChart7[i].ItemID;
                _logData.DataValue = (random.NextDouble() * 100).ToString("0.00");
                Strbuilder.Append(_logData.DataEntry());
            }

            //while (--dataNum>=0)
            //{
            //    _logData.ChartNo = "0"+random.Next(7,8).ToString();
            //    _logData.ChannelNo = random.Next(1,25).ToString();
            //    _logData.DataValue = (random.NextDouble()*100).ToString("0.00");
            //    Strbuilder.Append(_logData.DataEntry());
            //}

            Strbuilder.Append("!!\r\n");

            Console.WriteLine(Strbuilder.ToString());
            return Strbuilder.ToString();
        }
    }
}
