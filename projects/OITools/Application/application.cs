using OIGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml.Linq;

namespace MyApplication
{
    class Application
    {
        static void Main(string[] args)
        {
            // These values need to be looked up from OpenInvoice
            string supplierDUNS = "127369973";
            string supplierDept = "01";

            // These values will be parsed from the invoice
            Invoice invoiceObj = new Invoice();
            invoiceObj.companyCode = "RYAN";
            invoiceObj.companyName = "Ryan's Coffee Services Ltd";
            invoiceObj.invoiceNumber = "B42475";
            invoiceObj.invoiceDate = "2019-04-08";
            invoiceObj.invoiceType = "Original Invoice";
            invoiceObj.taxType = "GST";
            invoiceObj.gstTotal = "2.78";
            invoiceObj.invoiceTotal = "84.78";
            invoiceObj.currencyCode = "CAD";

            // Dev OpenInvoice URI
            XMLOIGenerator generator = new XMLOIGenerator(supplierDUNS, supplierDept, @"https://onboard.openinvoice.com/docp/api/supply-chain/v1/invoices:5553");

            // Prod OpenInvoice URI
            //XMLOIGenerator generator = new XMLOIGenerator(supplierDUNS, supplierDept, @"https://api.openinvoice.com/docp/api/supply-chain/v1/invoices:5553");

            generator.generateRequest(supplierDUNS, supplierDept, @"c:\test.pdf", invoiceObj);
            //Console.WriteLine(generator.getOIHeader());
            //Console.WriteLine(generator.getOIPayload());
            Console.WriteLine(generator.getRequestContent());
            Console.WriteLine(generator.send());
            Console.ReadLine();
        }
    }
}
