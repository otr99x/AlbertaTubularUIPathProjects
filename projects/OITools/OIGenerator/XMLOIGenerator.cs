using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
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
        private string mSupplierDunns;
        private HttpClient mHttpClient;
        private string mOIPayload;
        private byte[] mAttachment;
        private HttpRequestMessage mHttpRequestMessage;
        private string mSoapPayload;
        private string mUniqueTrackingIdentifier;

        public XMLOIGenerator()
        {
            mHttpClient = getWebClient();
            mSupplierDunns = "0000";
            mOIPayload = null;
            mAttachment = null;
            mHttpRequestMessage = null;
            mSoapPayload = null;
            mUniqueTrackingIdentifier = getUniqueIdentifier();
        }

        public string getSoapPayload()
        {
            return mSoapPayload;
        }

        public string getOIPayload()
        {
            return mOIPayload;
        }

        public void generateRequest(string supplierDunns, string attachmentFilePath, Invoice invoiceObj)
        {
            setOIPayloadFromInvoiceObject(invoiceObj);
            mSupplierDunns = supplierDunns;
            setFileByteArray(attachmentFilePath);
            setSoapPayload();
            setHttpRequest();
        }

        public void generateRequest(string supplierDunns, string attachmentFilePath, string jsonData)
        {
            setOIPayloadFromJSON(jsonData);
            mSupplierDunns = supplierDunns;
            setFileByteArray(attachmentFilePath);
            setSoapPayload();
            setHttpRequest();
        }

        private string getUniqueIdentifier()
        {
            return "UNIQ";
        }

        private HttpClient getWebClient()
        {
            X509Certificate2 clientCert = GetMyCert();
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCert);

            HttpClient client = new HttpClient();

            return client;

        }

        private X509Certificate2 GetMyCert()
        {
            X509Certificate2 cert = null;

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates.Find
            (
                X509FindType.FindBySerialNumber, "1fd9", true
            );
            if (certCollection.Count > 0)
            {
                cert = certCollection[0];
            }
            store.Close();

            return cert;
        }

        private void setOIPayloadFromJSON(string jsonData)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();
            Invoice invoiceObj = js.Deserialize<Invoice>(jsonData);
            setOIPayloadFromInvoiceObject(invoiceObj);
        }

        private void setOIPayloadFromInvoiceObject(Invoice invoiceObj)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(OpenImageInvoice));
            OpenImageInvoice invoice = new OpenImageInvoice();
            OpenImageInvoiceInvoiceHeader header = new OpenImageInvoiceInvoiceHeader();
            header.InvoiceNumber = invoiceObj.invoiceNumber;
            header.LongDescription = invoiceObj.companyName;
            invoice.InvoiceHeader = header;
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, invoice);
            mOIPayload = writer.ToString();
        }

        private void setFileByteArray(string filename)
        {
            mAttachment = File.ReadAllBytes(filename);
        }

        private void setSoapPayload()
        {
            XmlDocument payloadDoc = new XmlDocument();
            payloadDoc.LoadXml(mOIPayload);

            //create the soap envelopoe
            Envelope soapEnvelope = new Envelope();
            Header soapHeader = new Header();
            soapEnvelope.Header = soapHeader;
            Body soapBody = new Body();
            soapEnvelope.Body = soapBody;

            //load the body:

            XmlDocument document = new XmlDocument();
            soapBody.Any = new XmlElement[1];
            document.LoadXml("<DOHeaderSOAP xmlns=\"http://www.digitaloilfield.com/ocp\">" + payloadDoc.DocumentElement.OuterXml + "</DOHeaderSOAP>");
            soapBody.Any[0] = document.DocumentElement;


            //load the header

            XNamespace ns = string.Empty;
            XNamespace defaultns = "http://www.digitaloilfield.com/ocp";
            XElement element = new XElement(defaultns + "DoHeaderSoap",
               new XElement(defaultns + "DeliveryInformation",
                            new XElement(defaultns + "DocumentType", "INVOICEIMAGE"),
                            new XElement(defaultns + "TrackingIdentifier", mUniqueTrackingIdentifier),
                            new XElement(defaultns + "ReceiverIdentifier", "20010337", new XAttribute("identifierType", "DUNSNumber")),
                            new XElement(defaultns + "SenderIdentifier", mSupplierDunns, new XAttribute("identifierType", "DUNSNumber"))));

            document.LoadXml(element.ToString());
            soapHeader.Any = new XmlElement[1];
            soapHeader.Any[0] = document.DocumentElement;



            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, soapEnvelope);
            mSoapPayload = writer.ToString();

        }

        private void setHttpRequest()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri("http://google.com"));
            // set the basic authorization ----- Not needed
            //byte[] credentials = Encoding.UTF8.GetBytes("username:password1234");
            //message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            // additional headers

            // now add the multipart content
            //MultipartContent multicontent = new MultipartContent("text/xml", "MIME-BOUNDARY");
            MultipartContent multicontent = new MultipartContent("MIME-BOUNDARY");
            message.Content = multicontent;

            string soapPayload = mSoapPayload;
            HttpContent soapContent = new StringContent(soapPayload, Encoding.UTF8, "text/xml");
            multicontent.Add(soapContent);
            byte[] attachmentPayload = mAttachment;
            HttpContent attachmentContent = new ByteArrayContent(attachmentPayload);
            multicontent.Add(attachmentContent);
            // now get the hash of the entire stream --- Not needed
            //Task<Stream> task = multicontent.ReadAsStreamAsync();
            //task.Wait();
            //Stream multicontentStream = task.Result;
            //string hash = hashData("Some OI Key", multicontentStream);

            // now add this hash to the header of the request
            //message.Headers.Add("mac", hash);

            mHttpRequestMessage = message;
        }
        /*
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

        private void setFileString64(string filename)
        {
            byte[] afile = File.ReadAllBytes(filename);
            mAttachment = Convert.ToBase64String(afile);
        }

        public string getSoapPayload2(string OIPayload)
        {
            XmlDocument payloadDoc = new XmlDocument();
            payloadDoc.LoadXml(OIPayload);
            
            //create the soap envelopoe
            Envelope soapEnvelope = new Envelope();
            Header soapHeader = new Header();
            soapEnvelope.Header = soapHeader;
            Body soapBody = new Body();
            soapEnvelope.Body = soapBody;

            //load the body:

            XmlDocument document = new XmlDocument();
            soapBody.Any = new XmlElement[1];
            document.LoadXml("<DOHeaderSOAP xmlns=\"http://www.digitaloilfield.com/ocp\">" + payloadDoc.DocumentElement.OuterXml + "</DOHeaderSOAP>");
            soapBody.Any[0] = document.DocumentElement;

            
            //load the header

            string clientDunns = "ClientDunnsNumber";
            string trackingIdentifier = "some unique identifier";
            XElement payloadElement = XElement.Parse(OIPayload);
            XNamespace ns = string.Empty;
            XNamespace defaultns = "http://www.digitaloilfield.com/ocp";
            XElement element = new XElement(defaultns + "DoHeaderSoap",
               new XElement(defaultns + "DeliveryInformation",
                            new XElement(defaultns + "DocumentType", "INVOICEIMAGE"),
                            new XElement(defaultns + "TrackingIdentifier", trackingIdentifier),
                            new XElement(defaultns + "ReceiverIdentifier", "20010337", new XAttribute("identifierType", "DUNSNumber")),
                            new XElement(defaultns + "SenderIdentifier", clientDunns, new XAttribute("identifierType", "DUNSNumber"))));

            document.LoadXml(element.ToString());
            soapHeader.Any = new XmlElement[1];
            soapHeader.Any[0] = document.DocumentElement;

            

            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, soapEnvelope);
            return writer.ToString();

        }
        
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
        */

    }
}
