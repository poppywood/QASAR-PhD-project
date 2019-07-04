using System;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using Qasar.ESB.Adapter;
using Qasar.ESB.Helpers;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// WebServicePipe builds SOAP messages, sends them to the appropriate Web Service
    /// and returns the response
    /// </summary>
    public class CallbackWebServiceFilter : FilterBase
    {
        /// <summary>
        /// Name
        /// </summary>
        public override string Name { get { return "CallbackWebServiceFilter"; } }

        /// <summary>
        /// Code
        /// </summary>
        public override string Code { get { return "WEC"; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public CallbackWebServiceFilter()
        {
        }

        /// <summary>
        /// Enables a POST of a SOAP message to a web service using a callback 
        /// in case of long running transaction 
        /// </summary>
        /// <param name="data">The complete SOAP msg</param>
        /// <returns>The SOAP response</returns>
        protected override void ProcessData(PipeData data)
        {
            string action = data.Rule.Value(this.Code, "ACT", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            
            string url = data.Rule.Value(this.Code, "URL", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data, action);
            //data.Request = SOAPAdapter.Callback(data.Request, url, action);
            //temp fix to just pass rubbish on to each service
            data.Request = SOAPAdapter.Callback("hi", url, action);
        }
    }
}
