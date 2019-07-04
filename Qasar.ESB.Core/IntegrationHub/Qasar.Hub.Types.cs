
namespace Qasar.ESB
{
    using System.Xml.Serialization;
    using System;

    /// <summary>
    /// All classes used by the hub implement this interface so that they can be generically passed
    /// in and out of the Pipeline
    /// </summary>
    public interface IEntity
    {

    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://qasar.shellysaunders.co.uk/Header.xsd", TypeName = "Credentials")]
    public class Credentials
    {
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "LoginID")]
        public string LoginID;

        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "Password")]
        public string Password;

        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "User")]
        public string User;

        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "Source")]
        public string Source;

        public Credentials()
        {
        }
        public Credentials(string loginID, string password, string user, string source)
        {
            this.LoginID = loginID;
            this.Password = password;
            this.User = user;
            this.Source = source;
        }

    }

    /// <summary>
    /// Global Request Types
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(TypeName = "RequestType")]
    public enum RequestType
    {
        Create,
        Update,
        Renew,
        Cancel,
    }

    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://qasar.shellysaunders.co.uk/PolicySubmission.xsd", TypeName="PolicyRequest")]
    public class PolicyRequest : IEntity
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "Policy")]
        public string Policy;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "PolicyRef")]
        public string PolicyRef;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "ProductCode")]
        public string ProductCode;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "Action")]
        public PolicyRequestAction Action;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(ElementName = "Credentials")]
        public Credentials Credentials;
        
        public PolicyRequest()
        {
        }

        public PolicyRequest(string policy, Credentials credentials, string policyRef, string productCode, PolicyRequestAction action)
        {
            this.Policy = policy;
            this.Credentials = credentials;
            this.PolicyRef = policyRef;
            this.ProductCode = productCode;
            this.Action = action;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://qasar.shellysaunders.co.uk/PolicySubmission.xsd", TypeName = "RequestAction")]
        public enum PolicyRequestAction
        {

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "Cancel")]
            Cancel,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "Create")]
            Create,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "Reinstate")]
            Reinstate,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "Renew")]
            Renew,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "Update")]
            Update,

            /// <remarks/>
            [System.Xml.Serialization.XmlEnumAttribute(Name = "None")]
            None,
        }

    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://qasar.shellysaunders.co.uk/PolicyResponse.xsd", TypeName = "PolicyResponse")]
    public class PolicyResponse : IEntity
    {

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "OverallResult")]
        public string OverallResult;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(ElementName = "Result")]
        public string Result;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "PolicyRef")]
        public string PolicyRef;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName = "ProductCode")]
        public string ProductCode;
        
        public PolicyResponse()
        {
        }

        public PolicyResponse(string policyRef, string action, string productCode, string overallResult, string result)
        {
            this.OverallResult = overallResult;
            this.Result = result;
            this.PolicyRef = policyRef;
            this.ProductCode = productCode;
        }
    }

}
