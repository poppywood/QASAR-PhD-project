using System;
using System.Data;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.ComponentModel;
using Qasar.ESB;

namespace Qasar.ESBService
{
    [System.Web.Services.WebServiceBindingAttribute(Name = "Qasar.ESB", Namespace = "qasar.shellysaunders.co.uk")]
    [System.Web.Services.WebServiceAttribute(Namespace = "qasar.shellysaunders.co.uk")]
    public class ESB : System.Web.Services.WebService, IMyHub
    {

        public ESB()
        {
        }

        /// <summary>
        /// This method accepts policy data
        /// </summary>
        /// <param name="request">A PolicyRequest message</param>
        /// <returns>A PolicyResponse message</returns>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("submitPolicyIn", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare)]
        [return: System.Xml.Serialization.XmlElementAttribute("Response", Namespace = "http://qasar.shellysaunders.co.uk/PolicyResponse.xsd")]
        public virtual PolicyResponse SubmitPolicy([System.Xml.Serialization.XmlElementAttribute(Namespace = "http://qasar.shellysaunders.co.uk/PolicySubmission.xsd", ElementName = "Request")] PolicyRequest request)
        {
            Qasar.ESB.Core.IntegrationHub hub = new Qasar.ESB.Core.IntegrationHub();
            return hub.SubmitPolicy(request);
        }

    }
}
