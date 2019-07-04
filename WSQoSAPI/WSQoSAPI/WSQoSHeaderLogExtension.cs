using System;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSHeaderLogExtension : System.Web.Services.Protocols.SoapExtension
    {

        private string filename;
        private System.IO.Stream newStream;
        private System.IO.Stream oldStream;
        private string serviceName;

        public WSQoSHeaderLogExtension()
        {
            filename = "\uFFFD";
            serviceName = "service\uFFFD";
        }

        private void Copy(System.IO.Stream from, System.IO.Stream to)
        {
            System.IO.TextReader textReader = new System.IO.StreamReader(from);
            System.IO.TextWriter textWriter = new System.IO.StreamWriter(to);
            textWriter.WriteLine(textReader.ReadToEnd());
            textWriter.Flush();
        }

        public void WriteInput(System.Web.Services.Protocols.SoapMessage message)
        {
            // trial
        }

        public void WriteOutput(System.Web.Services.Protocols.SoapMessage message)
        {
            System.DateTime dateTime = System.DateTime.Now;
            System.IO.FileStream fileStream = new System.IO.FileStream(filename, System.IO.FileMode.Append, System.IO.FileAccess.Write);
            System.IO.StreamWriter streamWriter = new System.IO.StreamWriter(fileStream);
            streamWriter.WriteLine("\uFFFD");
            streamWriter.WriteLine("\uFFFD");
            streamWriter.WriteLine(" ---------------------------------- calling \uFFFD" + serviceName + " at \uFFFD" + dateTime + ". WS-QoS header value is: \uFFFD");
            System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
            newStream.Position = (long)0;
            xmlDocument.Load(newStream);
            System.Xml.XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("WSQoSSoapHeader\uFFFD");
            if (xmlNodeList.Count > 0)
                streamWriter.WriteLine(xmlNodeList[0].OuterXml);
            streamWriter.Flush();
            fileStream.Close();
            newStream.Position = (long)0;
            Copy(newStream, oldStream);
        }

        public override System.IO.Stream ChainStream(System.IO.Stream stream)
        {
            // trial
            return null;
        }

        public override object GetInitializer(System.Type WebServiceType)
        {
            return WebServiceType.GetType().ToString() + ".log\uFFFD";
        }

        public override object GetInitializer(System.Web.Services.Protocols.LogicalMethodInfo methodInfo, System.Web.Services.Protocols.SoapExtensionAttribute attribute)
        {
            // trial
            return null;
        }

        public override void Initialize(object initializer)
        {
            // trial
        }

        public override void ProcessMessage(System.Web.Services.Protocols.SoapMessage message)
        {
            switch (message.Stage)
            {
                case System.Web.Services.Protocols.SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    return;

                case System.Web.Services.Protocols.SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    return;

                default:
                    throw new System.Exception("invalid stage\uFFFD");

                case System.Web.Services.Protocols.SoapMessageStage.BeforeSerialize:
                case System.Web.Services.Protocols.SoapMessageStage.AfterDeserialize:
                    break;
            }
        }

    } // class WSQoSHeaderLogExtension

}

