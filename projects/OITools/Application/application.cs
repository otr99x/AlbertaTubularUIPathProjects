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

            Console.WriteLine("about to start the application");

            Invoice invoiceObj = new Invoice();
            invoiceObj.companyName = "Ryan's Coffee Services Ltd";
            invoiceObj.invoiceNumber = "B42475";
            invoiceObj.invoiceDate = "2019-04-08";
            invoiceObj.gstTotal = "2.78";
            invoiceObj.invoiceTotal = "84.78";

            XMLOIGenerator generator = new XMLOIGenerator(supplierDUNS, @"https://google.ca/api/getdata");

            generator.generateRequest(supplierDUNS, @"E:\Data\UiPath\AlbertaTubular\Attachments\Ryan\Inv_B42475_from_Ryans_Coffee_Service_Ltd._8584.pdf", invoiceObj);
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
