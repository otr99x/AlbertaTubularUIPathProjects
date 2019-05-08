using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OIGenerator
{
    public class Invoice
    {
        public string companyName { get; set; }
        public string invoiceNumber { get; set; }
        public string invoiceDate { get; set; }
        public string gstTotal { get; set; }
        public string invoiceTotal { get; set; }
    }
}
