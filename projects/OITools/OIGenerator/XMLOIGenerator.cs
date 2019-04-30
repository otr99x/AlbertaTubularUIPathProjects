using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace OIGenerator
{
    public class XMLOIGenerator
    {
         public string getXMLFromJSON(string jsonData)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Invoice invoiceObj = js.Deserialize<Invoice>(jsonData);
            return getXMLFromInvoiceObject(invoiceObj);
        }
        public string getXMLFromInvoiceObject(Invoice invoiceObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OpenImageInvoice));
            OpenImageInvoice invoice = new OpenImageInvoice();
            OpenImageInvoiceInvoiceHeader header = new OpenImageInvoiceInvoiceHeader();
            header.InvoiceNumber = invoiceObj.invoiceNumber;
            header.LongDescription = invoiceObj.companyName;
            invoice.InvoiceHeader = header;
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, invoice);
            return writer.ToString();
        }

        public string hashData(string key, string message)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA256 hmac = new HMACSHA256(keyByte);

            byte[] messageByte = encoding.GetBytes(message);
            byte[] hashMessage = hmac.ComputeHash(messageByte);

            return Convert.ToBase64String(hashMessage);
        }

        public string hashData(string key, Stream stream)
        {
            System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
            byte[] keyByte = encoding.GetBytes(key);
            HMACSHA256 hmac = new HMACSHA256(keyByte);

            byte[] hashMessage = hmac.ComputeHash(stream);

            return Convert.ToBase64String(hashMessage);
       }

        public string getFileString64(string filename)
        {
            byte[] afile = File.ReadAllBytes(filename);
            return Convert.ToBase64String(afile);
        }

        public byte[] getFileByteArray(string filename)
        {
            return File.ReadAllBytes(filename);
        }

        /* Build the 
         * 
         */
        public HttpRequestMessage getHttpRequest(string OIPayload)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://google.com"));
            // set the basic authorization
            byte[] credentials = Encoding.UTF8.GetBytes("username:password1234");
            message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            // additional headers

            // now add the multipart content
            MultipartContent multicontent = new MultipartContent("text/xml", "MIME-BOUNDARY");
            message.Content = multicontent;

            string soapPayload = getSoapPayload(OIPayload);
            HttpContent soapContent = new StringContent(soapPayload, Encoding.UTF8, "text/xml");
            multicontent.Add(soapContent);
            byte[] attachmentPayload = getFileByteArray(@"c:\test.pdf");
            HttpContent attachmentContent = new ByteArrayContent(attachmentPayload);
            multicontent.Add(attachmentContent);
            // now get the hash of the entire stream
            Task<Stream> task = multicontent.ReadAsStreamAsync();
            task.Wait();
            Stream multicontentStream = task.Result;
            string hash = hashData("Some OI Key", multicontentStream);

            // now add this hash to the header of the request
            message.Headers.Add("mac", hash);
             
            
            return message;
        }


        /* Generate the SoapPayload according to the SOAP specifications
         * Put the Open Invoice payload in the SOAP body
         * 
         */
        public string getSoapPayload(string OIPayload)
        {
            string clientDunns = "ClientDunnsNumber";
            string trackingIdentifier = "some unique identifier";
            XElement payloadElement = XElement.Parse(OIPayload);
            XNamespace ns = string.Empty;
            XNamespace defaultns = "http://www.digitaloilfield.com/ocp";
            XNamespace ns2 = "http://schemas.xmlsoap.org/soap/envelope/";
            XNamespace ns3 = "http://schemas.xmlsoap.org/soap/encoding/";
            XNamespace ns4 = "http://www.w3.org/1999/XMLSchema";
            XNamespace ns5 = "http://www.w3.org/1999/XMLSchema-instance";
            XNamespace ns6 = "http://schemas.xmlsoap.org/soap/encoding/";

            XElement element = new XElement(ns2 + "Envelope", new XAttribute(XNamespace.Xmlns + "SOAP-ENV", ns2), new XAttribute(XNamespace.Xmlns + "SOAP-ENC", ns3), new XAttribute(XNamespace.Xmlns + "xsd", ns4), new XAttribute(XNamespace.Xmlns + "xsi", ns5), new XAttribute(XNamespace.Xmlns + "encodingStyle", ns6),
                new XElement(ns2 + "Header",
                    new XElement(defaultns + "DoHeaderSOAP",
                        new XElement(defaultns + "DeliveryInformation",
                            new XElement(defaultns + "DocumentType", "INVOICEIMAGE"), 
                            new XElement(defaultns + "TrackingIdentifier", trackingIdentifier),
                            new XElement(defaultns + "ReceiverIdentifier", "20010337", new XAttribute("identifierType", "DUNSNumber")),
                            new XElement(defaultns + "SenderIdentifier", clientDunns, new XAttribute("identifierType", "DUNSNumber"))))),
                new XElement(ns2 + "Body",
                    new XElement(defaultns + "DOReceiveSOAP",
                        payloadElement)));

            return element.ToString();

        }

    }
}
