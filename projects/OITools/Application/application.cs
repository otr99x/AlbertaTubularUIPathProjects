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
            Console.WriteLine("about to start the application");
            XMLOIGenerator generator = new XMLOIGenerator("SupplierDunns", @"https://google.ca/api/getdata");

            Invoice invoiceObj = new Invoice();
            invoiceObj.companyName = "Diversified";
            invoiceObj.invoiceNumber = "1234";

            generator.generateRequest("supplier_dunns", @"c:\test.pdf", invoiceObj);
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
