﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.0.30319.33440.
// 
namespace OIGenerator {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.digitaloilfield.com/ocp")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.digitaloilfield.com/ocp", IsNullable=false)]
    public partial class DeliveryInformation {
        
        private deliveryInfoDocType documentTypeField;
        
        private System.DateTime dateTimeField;
        
        private bool dateTimeFieldSpecified;
        
        private DeliveryInformationTrackingIdentifier[] trackingIdentifierField;
        
        private tradingPartnerIdType receiverIdentifierField;
        
        private tradingPartnerIdType senderIdentifierField;
        
        private string numberOfAttachmentsField;
        
        /// <remarks/>
        public deliveryInfoDocType DocumentType {
            get {
                return this.documentTypeField;
            }
            set {
                this.documentTypeField = value;
            }
        }
        
        /// <remarks/>
        public System.DateTime DateTime {
            get {
                return this.dateTimeField;
            }
            set {
                this.dateTimeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool DateTimeSpecified {
            get {
                return this.dateTimeFieldSpecified;
            }
            set {
                this.dateTimeFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("TrackingIdentifier")]
        public DeliveryInformationTrackingIdentifier[] TrackingIdentifier {
            get {
                return this.trackingIdentifierField;
            }
            set {
                this.trackingIdentifierField = value;
            }
        }
        
        /// <remarks/>
        public tradingPartnerIdType ReceiverIdentifier {
            get {
                return this.receiverIdentifierField;
            }
            set {
                this.receiverIdentifierField = value;
            }
        }
        
        /// <remarks/>
        public tradingPartnerIdType SenderIdentifier {
            get {
                return this.senderIdentifierField;
            }
            set {
                this.senderIdentifierField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="integer")]
        public string NumberOfAttachments {
            get {
                return this.numberOfAttachmentsField;
            }
            set {
                this.numberOfAttachmentsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.digitaloilfield.com/ocp")]
    public enum deliveryInfoDocType {
        
        /// <remarks/>
        IMAGEINVOICE,
        
        /// <remarks/>
        DOCUMENTSTATUSUPDATE,
        
        /// <remarks/>
        UAPINVOICE,
        
        /// <remarks/>
        INVOICE,
        
        /// <remarks/>
        INVOICERESPONSE,
        
        /// <remarks/>
        FIELDTICKET,
        
        /// <remarks/>
        FIELDTICKETRESPONSE,
        
        /// <remarks/>
        PURCHASEORDER,
        
        /// <remarks/>
        PURCHASEORDERRESPONSE,
        
        /// <remarks/>
        PROJECT,
        
        /// <remarks/>
        COSTCOLLECTION,
        
        /// <remarks/>
        AFE,
        
        /// <remarks/>
        CC,
        
        /// <remarks/>
        DORESPONSE,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.digitaloilfield.com/ocp")]
    public partial class DeliveryInformationTrackingIdentifier {
        
        private DeliveryInformationTrackingIdentifierIndicator indicatorField;
        
        private bool indicatorFieldSpecified;
        
        private string valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public DeliveryInformationTrackingIdentifierIndicator indicator {
            get {
                return this.indicatorField;
            }
            set {
                this.indicatorField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool indicatorSpecified {
            get {
                return this.indicatorFieldSpecified;
            }
            set {
                this.indicatorFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.digitaloilfield.com/ocp")]
    public enum DeliveryInformationTrackingIdentifierIndicator {
        
        /// <remarks/>
        messageId,
        
        /// <remarks/>
        correlationId,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.digitaloilfield.com/ocp")]
    public partial class tradingPartnerIdType {
        
        private tradingPartnerIdTypeIdentifierType identifierTypeField;
        
        private string valueField;
        
        public tradingPartnerIdType() {
            this.identifierTypeField = tradingPartnerIdTypeIdentifierType.DUNSNumber;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public tradingPartnerIdTypeIdentifierType identifierType {
            get {
                return this.identifierTypeField;
            }
            set {
                this.identifierTypeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.33440")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.digitaloilfield.com/ocp")]
    public enum tradingPartnerIdTypeIdentifierType {
        
        /// <remarks/>
        DUNSNumber,
    }
}
