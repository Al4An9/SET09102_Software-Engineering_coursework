using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NBMFS.Models
{
    //constructor of sms
    public class Sms
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public string MType { get; set; }
        public string Sender { get; set; }
        
        public Sms()
        {
            Header = string.Empty;
            Body = string.Empty;
        }
    }
}
