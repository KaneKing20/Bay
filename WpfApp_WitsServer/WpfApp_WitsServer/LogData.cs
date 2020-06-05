using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_WitsServer
{
    class LogData
    {
        public string ChartNo { set; get; } = "-999";
        public string ChannelNo { set; get; } = "-999";
        public string DataValue { set; get; } = "-999";

        public string DataEntry()
        {
            return ChartNo + ChannelNo + DataValue + "\r\n";
        }
    }
}
