using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VimassFVA
{
    internal class Response
    {
        public int msgCode { get; set; }
        public String msgContent { get; set; }
        public Object result { get; set; }
        public double total { get; set; }
        public double totalMoney { get; set; }
        public int total2 { get; set; }
    }
}
