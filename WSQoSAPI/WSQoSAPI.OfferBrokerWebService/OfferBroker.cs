using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Configuration;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Uddi;
using Microsoft.Uddi.Api;
using Microsoft.Uddi.Binding;
using Microsoft.Uddi.Service;
using Microsoft.Uddi.ServiceType;
using WSQoSAPI;

namespace WSQoSAPI.OfferBrokerWebService
{

    [System.Web.Services.WebServiceBinding(Name = "OfferBroker", Namespace = "http://wsqos.org/")]
    [System.ComponentModel.DesignerCategory("code")]
    [System.Diagnostics.DebuggerStepThrough]
    public class OfferBroker : System.Web.Services.Protocols.SoapHttpClientProtocol
    {

        public OfferBroker()
        {
        }

        public System.IAsyncResult BeginGetBestOffer(System.Xml.XmlNode Requirements, string UddiTModelKey, System.AsyncCallback callback, object asyncState)
        {
            // trial
            return null;
        }

        public System.IAsyncResult BeginGetTrustedProviders(System.AsyncCallback callback, object asyncState)
        {
            // trial
            return null;
        }

        public System.IAsyncResult BeginGetUddiReqistryLocation(System.AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetUddiReqistryLocation\uFFFD", new object[0], callback, asyncState);
        }

        public WSQoSAPI.OfferBrokerWebService.RemoteOffer EndGetBestOffer(System.IAsyncResult asyncResult)
        {
            object[] objArr = EndInvoke(asyncResult);
            return (WSQoSAPI.OfferBrokerWebService.RemoteOffer)objArr[0];
        }

        public System.Xml.XmlNode EndGetTrustedProviders(System.IAsyncResult asyncResult)
        {
            object[] objArr = EndInvoke(asyncResult);
            return (System.Xml.XmlNode)objArr[0];
        }

        public string EndGetUddiReqistryLocation(System.IAsyncResult asyncResult)
        {
            // trial
            return null;
        }

        [System.Web.Services.Protocols.SoapDocumentMethod("http://wsqos.org/GetBestOffer", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public WSQoSAPI.OfferBrokerWebService.RemoteOffer GetBestOffer(System.Xml.XmlNode Requirements, string UddiTModelKey)
        {
            object[] objArr2 = new object[] {
                                              Requirements, 
                                              UddiTModelKey };
            object[] objArr1 = Invoke("GetBestOffer\uFFFD", objArr2);
            return (WSQoSAPI.OfferBrokerWebService.RemoteOffer)objArr1[0];
        }

        [System.Web.Services.Protocols.SoapDocumentMethod("http://wsqos.org/GetTrustedProviders", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public System.Xml.XmlNode GetTrustedProviders()
        {
            object[] objArr = Invoke("GetTrustedProviders\uFFFD", new object[0]);
            return (System.Xml.XmlNode)objArr[0];
        }

        [System.Web.Services.Protocols.SoapDocumentMethod("http://wsqos.org/GetUddiReqistryLocation", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

    } // class OfferBroker

}

