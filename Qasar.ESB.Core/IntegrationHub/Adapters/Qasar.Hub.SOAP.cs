using System;
using System.Data;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Threading;


namespace Qasar.ESB.Adapter
{

    /// <summary>
    /// SOAPAdapter
    /// </summary>
    public static class SOAPAdapter
    {
        /// <summary>
        /// Sync request-response SOAP submit to a Web Service
        /// </summary>
        /// <param name="content">Xml SOAP Body</param>
        /// <param name="url">Web Service Url</param>
        /// <param name="action">Web Service action</param>
        /// <returns></returns>
        public static string RequestReply(XmlDocument content, string url, string action)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(content);
            return RequestReplyInternal(soapEnvelopeXml, url, action);
        }

        /// <summary>
        /// As above but takes a string for content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string RequestReply(string content, string url, string action)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(content);
            return RequestReplyInternal(soapEnvelopeXml, url, action);
        }

        private static string RequestReplyInternal(XmlDocument soapEnvelopeXml, string url, string action)
        {
            HttpWebRequest webRequest = CreateWebRequest(url, action);

            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            return ExecuteWebRequest(webRequest);

        }

        /// <summary>
        /// Callback to a Web Service - the thread is suspended until the callback returns
        /// </summary>
        /// <param name="content">Xml SOAP Body</param>
        /// <param name="url">Web Service Url</param>
        /// <param name="action">Web Service action</param>
        /// <returns></returns>
        public static string Callback(XmlDocument content, string url, string action)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(content);
            return CallbackInternal(soapEnvelopeXml, url, action);
        }

        /// <summary>
        /// As above but takes a string as content.
        /// </summary>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public static string Callback(string content, string url, string action)
        {
            XmlDocument soapEnvelopeXml = CreateSoapEnvelope(content);
            return CallbackInternal(soapEnvelopeXml, url, action);
        }

        private static string CallbackInternal(XmlDocument soapEnvelopeXml, string url, string action)
        {
            HttpWebRequest webRequest = CreateWebRequest(url, action);

            InsertSoapEnvelopeIntoWebRequest(soapEnvelopeXml, webRequest);

            // begin async call to web request.
            IAsyncResult asyncResult = webRequest.BeginGetResponse(null, null);

            // suspend this thread until call is complete.
            asyncResult.AsyncWaitHandle.WaitOne(Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ExternalCallbackWebServiceTimeOut"]), false);

            string soapResult;

            // get the response from the completed web request.
            using (WebResponse webResponse = webRequest.EndGetResponse(asyncResult))
            using (StreamReader rd = new StreamReader(webResponse.GetResponseStream()))
            {
                soapResult = rd.ReadToEnd();
            }

            return soapResult;
        }

        const string _soapEnvelope =
@"<soap:Envelope xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:soap='http://schemas.xmlsoap.org/soap/envelope/'>
<soap:Body></soap:Body></soap:Envelope>";

        /// <summary>
        /// Creates a soap envelope by wrapping the given content xml document in a soap envelope
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static XmlDocument CreateSoapEnvelope(XmlDocument content)
        {
            // create an empty soap envelope
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(_soapEnvelope);

            // insert the content document.
            XmlNode body = soapEnvelopeXml.ChildNodes[0];
            body.AppendChild(soapEnvelopeXml.ImportNode(content.DocumentElement, true));

            return soapEnvelopeXml;
        }

        /// <summary>
        /// Creates a soap envelope by string manipulation before creating the xml document.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private static XmlDocument CreateSoapEnvelope(string content)
        {
            StringBuilder sb = new StringBuilder(_soapEnvelope);
            sb.Insert(sb.ToString().IndexOf("</soap:Body>"), content);

            // create an empty soap envelope
            XmlDocument soapEnvelopeXml = new XmlDocument();
            soapEnvelopeXml.LoadXml(sb.ToString());

            return soapEnvelopeXml;
        }

        /// <summary>
        /// Create a web request object for the given url and action
        /// </summary>
        /// <param name="url"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateWebRequest(string url, string action)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.Headers.Add("SOAPAction", action);
            webRequest.ContentType = "text/xml;charset=\"utf-8\"";
            webRequest.Accept = "text/xml";
            webRequest.Method = "POST";
            //webRequest.Timeout = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ExternalWebServiceTimeOut"]);
            return webRequest;
        }

        /// <summary>
        /// Insert the soap envelope into the web request.
        /// </summary>
        /// <param name="soapEnvelopeXml"></param>
        /// <param name="webRequest"></param>
        private static void InsertSoapEnvelopeIntoWebRequest(XmlDocument soapEnvelopeXml, HttpWebRequest webRequest)
        {
            using (Stream stream = webRequest.GetRequestStream())
            {
                soapEnvelopeXml.Save(stream);
            }
        }

        /// <summary>
        /// Execute the web request and return the result as a string
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns></returns>
        private static string ExecuteWebRequest(HttpWebRequest webRequest)
        {
            string result;

            using (WebResponse webResponse = webRequest.GetResponse())
            using (StreamReader reader = new StreamReader(webResponse.GetResponseStream()))
            {
                result = reader.ReadToEnd();
            }
            return result;
        }
    }
}