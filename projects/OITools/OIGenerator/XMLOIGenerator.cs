using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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
        private string mSupplierDept;
        private string mAttachmentFilePath;
        private HttpClient mHttpClient;
        private string mOIPayload;
        private byte[] mAttachment;
        private HttpRequestMessage mHttpRequestMessage;
        private string mSoapPayload;
        private string mUniqueTrackingIdentifier;
        private string mEndPointURL;

        public XMLOIGenerator(string supplierDunns, string supplierDept, string endpoint)
        {
            mHttpClient = getWebClient();
            mSupplierDunns = supplierDunns;
            mSupplierDept = supplierDept;
            mEndPointURL = endpoint;
            mOIPayload = null;
            mAttachmentFilePath = null;
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

        public string getRequestContent()
        {
            var response = string.Empty;
            var t = Task.Run(() => mHttpRequestMessage.Content.ReadAsStringAsync());
            t.Wait();
            response = t.Result;
            return response;
        }

        public void generateRequest(string supplierDunns, string supplierDept, string attachmentFilePath, Invoice invoiceObj)
        {
            setOIPayloadFromInvoiceObject(invoiceObj);
            mSupplierDunns = supplierDunns;
            mSupplierDept = supplierDept;
            mAttachmentFilePath = attachmentFilePath;
            setFileByteArray(mAttachmentFilePath);
            setSoapPayload();
            setHttpRequest();
        }

        public void generateRequest(string supplierDunns, string supplierDept, string attachmentFilePath, string jsonData)
        {
            setOIPayloadFromJSON(jsonData);
            mSupplierDunns = supplierDunns;
            mSupplierDept = supplierDept;
            mAttachmentFilePath = attachmentFilePath;
            setFileByteArray(attachmentFilePath);
            setSoapPayload();
            setHttpRequest();
        }

        private string getUniqueIdentifier()
        {
            return DateTime.Now.ToString("yyyyMMddHHmmss");
        }

        private HttpClient getWebClient()
        {
            X509Certificate2 clientCert = GetMyCert();
            HttpClientHandler handler = new HttpClientHandler();
            handler.ClientCertificates.Add(clientCert);

            HttpClient client = new HttpClient(handler);
             //client.DefaultRequestHeaders.Host = "onboard.openinvoice.com:5553";
            byte[] credentials = Encoding.UTF8.GetBytes("approver@albertatubular:Oildex18");
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

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

            header.Partner = new OpenImageInvoiceInvoiceHeaderPartner[2];

            // Buyer Company
            header.Partner[0] = new OpenImageInvoiceInvoiceHeaderPartner();
            header.Partner[0].PartnerTypeSpecified = true;
            header.Partner[0].PartnerType = OpenImageInvoiceInvoiceHeaderPartnerPartnerType.Buyer;
            header.Partner[0].Company = new OpenImageInvoiceInvoiceHeaderPartnerCompany();
            header.Partner[0].Company.CompanyCode = new OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCode[1];
            header.Partner[0].Company.CompanyCode[0] = new OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCode();
            header.Partner[0].Company.CompanyCode[0].CompanyCodeType = OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCodeCompanyCodeType.DUNSnumber;
            header.Partner[0].Company.CompanyCode[0].Value = mSupplierDunns;
            header.Partner[0].DepartmentOrGroup = new departmentOrGroupType();
            header.Partner[0].DepartmentOrGroup.DepartmentCode = mSupplierDept;

            // Supplier Company
            header.Partner[1] = new OpenImageInvoiceInvoiceHeaderPartner();
            header.Partner[1].PartnerTypeSpecified = true;
            header.Partner[1].PartnerType = OpenImageInvoiceInvoiceHeaderPartnerPartnerType.Supplier;
            header.Partner[1].Company = new OpenImageInvoiceInvoiceHeaderPartnerCompany();
            header.Partner[1].Company.CompanyCode = new OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCode[1];
            header.Partner[1].Company.CompanyCode[0] = new OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCode();
            header.Partner[1].Company.CompanyCode[0].CompanyCodeType = OpenImageInvoiceInvoiceHeaderPartnerCompanyCompanyCodeCompanyCodeType.VendorNumber;
            header.Partner[1].Company.CompanyCode[0].Value = invoiceObj.companyCode;
            header.Partner[1].Company.CompanyName = invoiceObj.companyName;

            // Invoice Fields
            header.InvoiceDate = Convert.ToDateTime(invoiceObj.invoiceDate);
            header.InvoiceType = invoiceObj.invoiceType;
            header.Total = Convert.ToDecimal(invoiceObj.invoiceTotal);

            // Tax Fields
            header.Tax = new Tax[1];
            header.Tax[0] = new Tax();
            header.Tax[0].TotalSpecified = true;
            header.Tax[0].Total = Convert.ToDecimal(invoiceObj.gstTotal);
            header.Tax[0].TaxType = invoiceObj.taxType;

            // Currency
            header.CurrencyCode = invoiceObj.currencyCode;

            invoice.InvoiceHeader = header;

            // Attachment Reference
            invoice.AttachmentReference = new OpenImageInvoiceAttachmentReference();
            invoice.AttachmentReference.href = "cid:openImageAttachment" + mUniqueTrackingIdentifier;
            invoice.AttachmentReference.Value = mUniqueTrackingIdentifier;

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, invoice);
            mOIPayload = writer.ToString();
        }

        private void setFileByteArray(string filename)
        {
            mAttachment = Encoding.UTF8.GetBytes(Convert.ToBase64String(File.ReadAllBytes(filename)));

            //mAttachment = File.ReadAllBytes(filename);
            //string mAttachmentStr =  Convert.ToBase64String(mAttachment);
            //mAttachment = Encoding.UTF8.GetBytes(mAttachmentStr);
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
            document.LoadXml("<DOReceiveSOAP xmlns=\"http://www.digitaloilfield.com/ocp\">" + payloadDoc.DocumentElement.OuterXml + "</DOReceiveSOAP>");
            soapBody.Any[0] = document.DocumentElement;

            //load the header
            XNamespace ns = string.Empty;
            XNamespace defaultns = "http://www.digitaloilfield.com/ocp";
            XElement element = new XElement(defaultns + "DoHeaderSoap",
               new XElement(defaultns + "DeliveryInformation",
                            new XElement(defaultns + "DocumentType", "INVOICEIMAGE"),
                            new XElement(defaultns + "TrackingIdentifier", mUniqueTrackingIdentifier),
                            new XElement(defaultns + "ReceiverIdentifier", "200103377", new XAttribute("identifierType", "DUNSNumber")),
                            new XElement(defaultns + "SenderIdentifier", mSupplierDunns, new XAttribute("identifierType", "DUNSNumber"))));

            document.LoadXml(element.ToString());
            soapHeader.Any = new XmlElement[1];
            soapHeader.Any[0] = document.DocumentElement;

            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, soapEnvelope);
            mSoapPayload = writer.ToString();
        }

        public void setSoapPayload2()
        {
            string clientDunns = mSupplierDunns;
            string trackingIdentifier = mUniqueTrackingIdentifier;
            XElement payloadElement = XElement.Parse(mOIPayload);
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
                            new XElement(defaultns + "ReceiverIdentifier", "200103377", new XAttribute("identifierType", "DUNSNumber")),
                            new XElement(defaultns + "SenderIdentifier", clientDunns, new XAttribute("identifierType", "DUNSNumber"))))),
                new XElement(ns2 + "Body",
                    new XElement(defaultns + "DOReceiveSOAP",
                        payloadElement)));

            mSoapPayload = element.ToString();
        }

        private void setHttpRequest()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(mEndPointURL));
            //byte[] credentials = Encoding.UTF8.GetBytes("approver@albertatubular:Oildex18");
            //message.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
            // additional headers
            //message.Headers.Host = "onboard.openinvoice.com:5555";
 
            // now add the multipart content
            MultipartContent multicontent = new MultipartContent("Related", "MIME-BOUNDARY");
            message.Content = multicontent;

            string soapPayload = mSoapPayload;
            HttpContent soapContent = new StringContent(soapPayload, Encoding.UTF8, "text/xml");
            soapContent.Headers.Add("Content-Id", "<BodyPart>");
            soapContent.Headers.Add("Content-Transfer-Encoding", "8bit");
            multicontent.Add(soapContent);

            byte[] attachmentPayload = mAttachment;
            HttpContent attachmentContent = new ByteArrayContent(attachmentPayload);
            attachmentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileName = mAttachmentFilePath
            };

            attachmentContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            attachmentContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("name", "\"" + mAttachmentFilePath + "\""));
            attachmentContent.Headers.Add("Content-Transfer-Encoding", "base64");
            attachmentContent.Headers.Add("Content-Id", "<cid: openImageAttachment" + mUniqueTrackingIdentifier + ">");

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

        public string send()
        {
            var response = string.Empty;
            var t = Task.Run(() => sendRequest());
            t.Wait();
            response = t.Result;

            // HTTP 200 = Success
            // HTTP 400 = Malformed Request
            return response;
        }

        private async Task<string> sendRequest()
        {
            var response = string.Empty;
            using (mHttpClient)
            {
                HttpResponseMessage result = await mHttpClient.SendAsync(mHttpRequestMessage);
                if (result.IsSuccessStatusCode)
                {
                    response = result.StatusCode.ToString();
                }
                else
                {
                    // get the response message
                    response = await result.Content.ReadAsStringAsync();
                }
            }
            return response;
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
        
        */

    }
}
