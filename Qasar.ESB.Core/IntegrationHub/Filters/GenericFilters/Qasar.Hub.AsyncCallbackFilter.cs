using System;
using System.Collections.Generic;
using System.Text;
using Qasar.ESB.Helpers;

namespace Qasar.ESB.Filter
{
    /// <summary>
    /// AsyncCallbackFilter
    /// </summary>
    public class AsyncCallbackFilter : FilterBase
    {
        /// <summary>
        /// Name
        /// </summary>
        public override string Name { get { return "AsyncCallbackFilter"; } }
        
        /// <summary>
        /// Code
        /// </summary>
        public override string Code { get { return "ACF"; } }

        /// <summary>
        /// Constructor
        /// </summary>
        public AsyncCallbackFilter()
        {
        }

        /// <summary>
        /// This filter should run even if a failure has occured earlier in the pipeline, so set
        /// SkipOnFailure to return false.
        /// </summary>
        protected override bool SkipOnFailure
        {
            get{ return false; }
        }

        /// <summary>
        /// Process
        /// </summary>
        /// <param name="data"></param>
        protected override void ProcessData(PipeData data)
        {
            string url = data.Rule.Value(this.Code, "URL", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);
            string action = data.Rule.Value(this.Code, "ACT", data.ProductCode, Convert.ToString(data.RequestType), data.SubAction, Convert.ToString(data.Source), data.UserId, data);

            PolicyResponse policyResponse = GetPolicyResponse(data);
            System.Xml.XmlDocument policyResponseXml = GetPolicyResponseAsXml(policyResponse);

            string result = Adapter.SOAPAdapter.RequestReply(policyResponseXml, url, action);

            // TODO: what result do we want and what do we need to do with it?
        }

        /// <summary>
        /// Create a policy response object from the pipe data.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static PolicyResponse GetPolicyResponse(PipeData data)
        {
            IEntity entity = Pipeline.PostProcessor.Parse(data);
            PolicyResponse policyResponse = entity as PolicyResponse;
            if (policyResponse == null) throw new ApplicationException("Unexpected request type");
            return policyResponse;
        }

        /// <summary>
        /// Convert the policy response object into a suitable soap body by serializing it as xml
        /// and wrapping it with a Callback method.
        /// </summary>
        /// <param name="policyResponse"></param>
        /// <returns></returns>
        private static System.Xml.XmlDocument GetPolicyResponseAsXml(PolicyResponse policyResponse)
        {
            // create the Callback element
            System.Xml.XmlDocument callbackXml = new System.Xml.XmlDocument();
            callbackXml.LoadXml("<Callback xmlns='http://www.Qasar-ina.com/' />");

            // create an xml serializer for the policyResponse object
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(PolicyResponse));

            // serialize the policyResponse object into an xml document
            System.Xml.XmlDocument responseXml = new System.Xml.XmlDocument();
            using (System.IO.StringWriter writer = new System.IO.StringWriter())
            {
                serializer.Serialize(writer, policyResponse);
                responseXml.LoadXml(writer.ToString());
            }

            // insert the serialized policyResponse into the callback element
            callbackXml.DocumentElement.AppendChild(
                callbackXml.ImportNode(responseXml.DocumentElement, true));

            // return the combined xml document
            return callbackXml;
        }
    }
}
