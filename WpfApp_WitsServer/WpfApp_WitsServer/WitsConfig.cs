using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_WitsServer
{
    class WitsConfig
    {
        public List<WitsChart> WitsChart7 { set; get; }

        public WitsConfig()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            WitsChart7 = new List<WitsChart>();
            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "12",
                Desc = "Depth."
            });

            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "13",
                Desc = "Inc."
            });

            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "15",
                Desc = "Azi."
            });

            //WitsChart7.Add(new WitsChart()
            //{
            //    RecordID = "07",
            //    ItemID = "22",
            //    Desc = "Flag."
            //});

            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "23",
                Desc = "Total Gravity."
            });

            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "24",
                Desc = "Total Magnet."
            });

            WitsChart7.Add(new WitsChart()
            {
                RecordID = "07",
                ItemID = "25",
                Desc = "Temperture."
            });


        }
    }
}
