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
using LASHelperLib;
using LASHelperLib.LAS;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string[]> AllDatasFromLas { set; get; }//用于存储某个las文件的所有数据，List中每个数组记录了对应的6个元素（dept,inc,azi,gt,bt,temp)
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Loadlas_Click(object sender, RoutedEventArgs e)
        {
            string fileName = @"E:\xt18\6.Repos\GitHub\Bay\WpfApp_WitsServer\WpfApp_WitsServer\Files\sv.las";
            var data_Set = new LasDataSet();
            string str = string.Empty;
            AllDatasFromLas = new List<string[]>();

            using (Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            using (StreamReader tr = new StreamReader(new BufferedStream(stream)))
            data_Set.Load(tr);

            if (data_Set.ErrorDescription != null)
            {
                System.Windows.MessageBox.Show("dataset.errorDescription return null");
            }
            
            using(Stream stream = new FileStream(fileName,FileMode.Open,FileAccess.Read))
            using(StreamReader tr = new StreamReader(new BufferedStream(stream)))
                str = tr.ReadToEnd();

            List<DataSourceLib.DataSourceModel> dataset = data_Set.DataSources;
            string[] DepthString = new string[dataset.Count];
            string strTest = dataset[3].Samples[0].Value;

            System.Windows.MessageBox.Show(strTest);
            //foreach (var item in dataset)
            //{
            //    foreach (var item2 in item.Samples)
            //    {
            //        System.Windows.MessageBox.Show(item2.Depth + "," + item2.Value);

            //    }
            //}
        }
    }
}
