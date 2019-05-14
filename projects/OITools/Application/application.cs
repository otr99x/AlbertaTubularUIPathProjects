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
            string supplierDUNS = "247021421";
            string supplierDept = "Accounts Payable";

            Console.WriteLine("about to start the application");

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

            XMLOIGenerator generator = new XMLOIGenerator(supplierDUNS, supplierDept, @"https://google.ca/api/getdata");

            generator.generateRequest(supplierDUNS, supplierDept, @"E:\Data\UiPath\AlbertaTubular\Attachments\Ryan\RyanLogo.pdf", invoiceObj);
            Console.WriteLine("-------------OI Payload---------------");
            Console.WriteLine(generator.getOIPayload());
            Console.WriteLine("-------------Soap Payload---------------");
            Console.WriteLine(generator.getSoapPayload());
            Console.WriteLine("-------------Request Content Payload---------------");
            Console.WriteLine(generator.getRequestContent());
            //generator.send();

            /*
            JavaScriptSerializer js = new JavaScriptSerializer();
            string jsonData = js.Serialize(invoiceObj);
            string invoiceXML = generator.getXMLFromJSON(jsonData);
            Console.WriteLine(invoiceXML);
            Console.WriteLine("----------------------------------");
            string soapPayload = generator.getSoapPayload2(invoiceXML);
            Console.WriteLine(soapPayload);
            Console.WriteLine(generator.getFileString64(@"c:\test.pdf"));
            Console.WriteLine(generator.hashData("Some Key", "This is the data to encrypt"));
            XElement element = XElement.Parse("<OpenImageInvoice></OpenImageInvoice>");
            Console.WriteLine(element.ToString());
            Console.WriteLine(generator.getSoapPayload("<OpenImageInvoice></OpenImageInvoice>"));
            */
        }
    }
}
