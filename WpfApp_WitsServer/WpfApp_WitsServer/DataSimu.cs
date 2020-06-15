using LASHelperLib.LAS;
using System;
using System.Collections.Generic;
using System.IO;
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
        private List<DataSourceLib.DataSourceModel> dataset;
        private int index = 0;
        //一个完整的传输信息，包含所有的某个时刻或者某个深度对应的数据组合。
        public string Simu(WitsConfig witsConfig)
        {
            GetDatasFromLasFile();

            
            int dataNum = witsConfig.WitsChart7.Count();
            _logData = new LogData();
            Strbuilder = new StringBuilder();
            Strbuilder.Append("&&\r\n");

            for (int i = 0; i < dataNum; i++)
            {
                _logData.ChartNo = witsConfig.WitsChart7[i].RecordID;
                _logData.ChannelNo = witsConfig.WitsChart7[i].ItemID;
                _logData.DataValue = dataset[i].Samples[index].Value;//(random.NextDouble() * 100).ToString("0.00");//此处需要修改为las文件中的数据
                Strbuilder.Append(_logData.DataEntry());
            }

            if (index >= dataset[0].Samples.Count-1)
            {
                index = 0;
            }
            else
            {
                index++;
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

            //

        }

        //从las文件中获取到所有的数据，数据都放在dataset中，dataset[3].Samples[0].Value代表的就是第4列第一行的数据，gt的第一个值。
        public void  GetDatasFromLasFile()
        {
            //
            string fileName = @"E:\xt18\6.Repos\GitHub\Bay\WpfApp_WitsServer\WpfApp_WitsServer\Files\sv.las";
            var data_Set = new LasDataSet();
            string str = string.Empty;
            
            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader tr = new StreamReader(new BufferedStream(stream)))
                data_Set.Load(tr);

            if (data_Set.ErrorDescription != null)
            {
                System.Windows.MessageBox.Show("dataset.errorDescription return null");
            }

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader tr = new StreamReader(new BufferedStream(stream)))
                str = tr.ReadToEnd();

            dataset = data_Set.DataSources;
            string[] DepthString = new string[dataset.Count];
            string strTest = dataset[3].Samples[0].Value;

            //System.Windows.MessageBox.Show(strTest);

            
           
        }
    }
        
    
}
