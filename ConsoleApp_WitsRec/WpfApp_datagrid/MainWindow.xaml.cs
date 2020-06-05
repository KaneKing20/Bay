using System;
using System.Collections.Generic;
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

namespace WpfApp_datagrid
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            McDataGrid.ItemsSource = LoadData();
        }

        private List<Wits7> LoadData()
        {
            List<Wits7> wits7s = new List<Wits7>();
            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "13",
                Desc = "Inc."
            });

            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "15",
                Desc = "Azi."
            });

            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "22",
                Desc = "Flag."
            });

            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "23",
                Desc = "Total Gravity."
            });

            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "24",
                Desc = "Total Magnet."
            });

            wits7s.Add(new Wits7()
            {
                RecordID = "07",
                ItemID = "25",
                Desc = "Temperture."
            });

            return wits7s;
        }

        private List<Author> LoadCollectionData()
        {
            List<Author> authors = new List<Author>();
            authors.Add(new Author()
            {
                ID = 101,
                Name = "Mahesh Chand",
                BookTitle = "Graphics Programming with GDI+",
                DOB = new DateTime(1975, 2, 23),
                IsMVP = false
            });

            authors.Add(new Author()
            {
                ID = 201,
                Name = "Mike Gold",
                BookTitle = "Programming C#",
                DOB = new DateTime(1982, 4, 12),
                IsMVP = true
            });

            authors.Add(new Author()
            {
                ID = 244,
                Name = "Mathew Cochran",
                BookTitle = "LINQ in Vista",
                DOB = new DateTime(1985, 9, 11),
                IsMVP = true
            });

            return authors;
        }
    }
}
