using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml;

namespace WSQoSAPI.OfferBrokerWebService
{

    [WebServiceBinding(Name = "OfferBroker", Namespace = "http://wsqos.org/")]
    [DesignerCategory("code")]
    [DebuggerStepThrough]
    public class OfferBroker : SoapHttpClientProtocol
    {

        public OfferBroker()
        {
        }

        public IAsyncResult BeginGetBestOffer(XmlNode Requirements, string UddiTModelKey, AsyncCallback callback, object asyncState)
        {
            // trial
            return null;
        }

        public IAsyncResult BeginGetTrustedProviders(AsyncCallback callback, object asyncState)
        {
            // trial
            return null;
        }

        public IAsyncResult BeginGetUddiReqistryLocation(AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetUddiReqistryLocation\uFFFD", new object[0], callback, asyncState);
        }

        public RemoteOffer EndGetBestOffer(IAsyncResult asyncResult)
        {
            object[] objArr = EndInvoke(asyncResult);
            return (RemoteOffer)objArr[0];
        }

        public XmlNode EndGetTrustedProviders(IAsyncResult asyncResult)
        {
            object[] objArr = EndInvoke(asyncResult);
            return (XmlNode)objArr[0];
        }

        public string EndGetUddiReqistryLocation(IAsyncResult asyncResult)
        {
            // trial
            return null;
        }

        [SoapDocumentMethod("http://wsqos.org/GetBestOffer", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public RemoteOffer GetBestOffer(XmlNode Requirements, string UddiTModelKey)
        {
            object[] objArr2 = new object[] {
                                              Requirements, 
                                              UddiTModelKey };
            object[] objArr1 = Invoke("GetBestOffer\uFFFD", objArr2);
            return (RemoteOffer)objArr1[0];
        }

        [SoapDocumentMethod("http://wsqos.org/GetTrustedProviders", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public XmlNode GetTrustedProviders()
        {
            object[] objArr = Invoke("GetTrustedProviders\uFFFD", new object[0]);
            return (XmlNode)objArr[0];
        }

        [SoapDocumentMethod("http://wsqos.org/GetUddiReqistryLocation", RequestNamespace = "http://wsqos.org/", ResponseNamespace = "http://wsqos.org/", Use = (System.Web.Services.Description.SoapBindingUse)2, ParameterStyle = (System.Web.Services.Protocols.SoapParameterStyle)2)]
        public string GetUddiReqistryLocation()
        {
            // trial
            return null;
        }

    } // class OfferBroker

}

