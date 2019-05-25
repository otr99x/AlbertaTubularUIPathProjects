using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
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
        private string mOIHeader;
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
            mOIHeader = null;
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

        public string getOIHeader()
        {
            return mOIHeader;
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
            //setOIHeader();
            setFileByteArray(mAttachmentFilePath);
            //setSoapPayload();
            setHttpRequest();
        }

        public void generateRequest(string supplierDunns, string supplierDept, string attachmentFilePath, string jsonData)
        {
            setOIPayloadFromJSON(jsonData);
            mSupplierDunns = supplierDunns;
            mSupplierDept = supplierDept;
            mAttachmentFilePath = attachmentFilePath;
            //setOIHeader();
            setFileByteArray(mAttachmentFilePath);
            //setSoapPayload();
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
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12 | (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(AllwaysGoodCertificate);

            return client;
        }

        private static bool AllwaysGoodCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        private X509Certificate2 GetMyCert()
        {
            X509Certificate2 cert = null;

            var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
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

        private void setOIHeader()
        {
            DeliveryInformation deliveryInfo = new DeliveryInformation();
            deliveryInfo.DocumentType = deliveryInfoDocType.IMAGEINVOICE;

            deliveryInfo.TrackingIdentifier = new DeliveryInformationTrackingIdentifier[1];
            deliveryInfo.TrackingIdentifier[0] = new DeliveryInformationTrackingIdentifier();
            //deliveryInfo.TrackingIdentifier[0].indicatorSpecified = true;
            //deliveryInfo.TrackingIdentifier[0].indicator = DeliveryInformationTrackingIdentifierIndicator.correlationId;
            deliveryInfo.TrackingIdentifier[0].Value = mUniqueTrackingIdentifier;


            deliveryInfo.ReceiverIdentifier = new tradingPartnerIdType();
            deliveryInfo.ReceiverIdentifier.identifierType = tradingPartnerIdTypeIdentifierType.DUNSNumber;
            deliveryInfo.ReceiverIdentifier.Value = "200103377";

            deliveryInfo.SenderIdentifier = new tradingPartnerIdType();
            deliveryInfo.SenderIdentifier.identifierType = tradingPartnerIdTypeIdentifierType.DUNSNumber;
            deliveryInfo.SenderIdentifier.Value = mSupplierDunns;

            XmlSerializer serializer = new XmlSerializer(typeof(DeliveryInformation));
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.digitaloilfield.com/ocp");
            using (StringWriterUtf8 writer = new StringWriterUtf8())
            {
                serializer.Serialize(writer, deliveryInfo, ns);
                mOIHeader = writer.ToString();
            }
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
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "http://www.digitaloilfield.com/ocp");

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

            using (StringWriterUtf8 writer = new StringWriterUtf8())
            {
                serializer.Serialize(writer, invoice, ns);
                mOIPayload = writer.ToString();
            }
        }

        private void setFileByteArray(string filename)
        {
            mAttachment = Encoding.UTF8.GetBytes(Convert.ToBase64String(File.ReadAllBytes(filename)));

        }

        private void setSoapPayload()
        {
            XmlDocument payloadDoc = new XmlDocument();
            payloadDoc.LoadXml(mOIPayload);
            XmlDocument headerDoc = new XmlDocument();
            headerDoc.LoadXml(mOIHeader);

            //create the soap envelopoe
            Envelope soapEnvelope = new Envelope();
            Header soapHeader = new Header();
            soapEnvelope.Header = soapHeader;
            Body soapBody = new Body();
            soapEnvelope.Body = soapBody;

            //load the body:
            XmlDocument documentBody = new XmlDocument();
            documentBody.LoadXml("<DOReceiveSOAP xmlns=\"http://www.digitaloilfield.com/ocp\">" + payloadDoc.DocumentElement.OuterXml + "</DOReceiveSOAP>");
            soapBody.Any = new XmlElement[1];
            soapBody.Any[0] = documentBody.DocumentElement;

            //load the header
            XmlDocument documentHeader = new XmlDocument();
            documentHeader.LoadXml("<DOHeaderSOAP xmlns=\"http://www.digitaloilfield.com/ocp\">" + headerDoc.DocumentElement.OuterXml + "</DOHeaderSOAP>");
            soapHeader.Any = new XmlElement[1];
            soapHeader.Any[0] = documentHeader.DocumentElement;

            XmlSerializer serializer = new XmlSerializer(typeof(Envelope));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("SOAP-ENV", "http://schemas.xmlsoap.org/soap/envelope/");
            namespaces.Add("SOAP-ENC", "http://schemas.xmlsoap.org/soap/encoding/");
            namespaces.Add("xsd", "http://www.w3.org/1999/XMLSchema");
            namespaces.Add("xsi", "http://www.w3.org/1999/XMLSchema-instance");

            using (StringWriterUtf8 writer = new StringWriterUtf8())
            {
               serializer.Serialize(writer, soapEnvelope, namespaces);
                mSoapPayload = writer.ToString();
            }
        }

         private void setHttpRequest()
        {
            var message = new HttpRequestMessage(HttpMethod.Post, new Uri(mEndPointURL));
 
            // now add the multipart content
            MultipartContent multicontent = new MultipartContent("Mixed", "MIME-BOUNDARY");
            message.Content = multicontent;
 
            string soapPayload = mSoapPayload;
            HttpContent soapContent = new StringContent(mOIPayload, Encoding.UTF8, "application/xml");
            soapContent.Headers.Add("Content-Id", "<BodyPart>");
            soapContent.Headers.Add("Content-Transfer-Encoding", "8bit");
            multicontent.Add(soapContent);

            byte[] attachmentPayload = mAttachment;
            HttpContent attachmentContent = new ByteArrayContent(attachmentPayload);
            //attachmentContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("inline");
            //attachmentContent.Headers.ContentDisposition.Parameters.Add(new NameValueHeaderValue("filename", "\"" + Path.GetFileName(mAttachmentFilePath) + "\""));
            attachmentContent.Headers.ContentType = new MediaTypeHeaderValue("application/pdf");
            attachmentContent.Headers.ContentType.Parameters.Add(new NameValueHeaderValue("name", "\"" + Path.GetFileName(mAttachmentFilePath) + "\""));
            attachmentContent.Headers.Add("Content-Transfer-Encoding", "base64");
            attachmentContent.Headers.Add("Content-Id", "<" + mUniqueTrackingIdentifier + ">");

            multicontent.Add(attachmentContent);

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
 
    }
}
