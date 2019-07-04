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
    public class WebServiceFilter : FilterBase
	{
        /// <summary>
        /// 
        /// </summary>
        public override string Name { get { return "WebServiceFilter"; } }
        /// <summary>
        /// 
        /// </summary>
        public override string Code { get { return "WEB"; } }
        /// <summary>
        /// 
        /// </summary>
        public WebServiceFilter()
		{
        }
		/// <summary>
		/// Enables a POST of a SOAP message to a web service.
		/// </summary>
		/// <param name="msg">The complete SOAP msg</param>
		/// <returns>The SOAP response</returns>
        protected override void ProcessData(PipeData data)
		{
            string url = data.Rule.Value(this.Code, "URL", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            string action = data.Rule.Value(this.Code, "ACT", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            data.Request = SOAPAdapter.RequestReply(data.Request, url, action);
		}
	}
}
