using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp_datagrid
{
    class Author
    {
        public int ID { set; get; }
        public string Name { get; set; }
        public DateTime DOB { get; set; }
        public string BookTitle { set; get; }
        public bool IsMVP { get; set; }

     
    }
}
