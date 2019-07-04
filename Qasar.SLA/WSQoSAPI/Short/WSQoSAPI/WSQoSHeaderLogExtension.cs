using System;
using System.IO;
using System.Web.Services.Protocols;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSHeaderLogExtension : SoapExtension
    {

        private string filename;
        private Stream newStream;
        private Stream oldStream;
        private string serviceName;

        public WSQoSHeaderLogExtension()
        {
            filename = "\uFFFD";
            serviceName = "service\uFFFD";
        }

        private void Copy(Stream from, Stream to)
        {
            TextReader textReader = new StreamReader(from);
            TextWriter textWriter = new StreamWriter(to);
            textWriter.WriteLine(textReader.ReadToEnd());
            textWriter.Flush();
        }

        public void WriteInput(SoapMessage message)
        {
            // trial
        }

        public void WriteOutput(SoapMessage message)
        {
            DateTime dateTime = DateTime.Now;
            FileStream fileStream = new FileStream(filename, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream);
            streamWriter.WriteLine("\uFFFD");
            streamWriter.WriteLine("\uFFFD");
            streamWriter.WriteLine(" ---------------------------------- calling \uFFFD" + serviceName + " at \uFFFD" + dateTime + ". WS-QoS header value is: \uFFFD");
            XmlDocument xmlDocument = new XmlDocument();
            newStream.Position = (long)0;
            xmlDocument.Load(newStream);
            XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("WSQoSSoapHeader\uFFFD");
            if (xmlNodeList.Count > 0)
                streamWriter.WriteLine(xmlNodeList[0].OuterXml);
            streamWriter.Flush();
            fileStream.Close();
            newStream.Position = (long)0;
            Copy(newStream, oldStream);
        }

        public override Stream ChainStream(Stream stream)
        {
            // trial
            return null;
        }

        public override object GetInitializer(Type WebServiceType)
        {
            return WebServiceType.GetType().ToString() + ".log\uFFFD";
        }

        public override object GetInitializer(LogicalMethodInfo methodInfo, SoapExtensionAttribute attribute)
        {
            // trial
            return null;
        }

        public override void Initialize(object initializer)
        {
            // trial
        }

        public override void ProcessMessage(SoapMessage message)
        {
            switch (message.Stage)
            {
                case SoapMessageStage.AfterSerialize:
                    WriteOutput(message);
                    return;

                case SoapMessageStage.BeforeDeserialize:
                    WriteInput(message);
                    return;

                default:
                    throw new Exception("invalid stage\uFFFD");

                case SoapMessageStage.BeforeSerialize:
                case SoapMessageStage.AfterDeserialize:
                    break;
            }
        }

    } // class WSQoSHeaderLogExtension

}

