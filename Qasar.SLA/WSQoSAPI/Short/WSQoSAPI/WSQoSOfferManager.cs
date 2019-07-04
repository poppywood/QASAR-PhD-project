using System;
using System.Text;
using System.Threading;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSOfferManager
    {

        private ImportQoSDeclarationAttribute[] Imports;
        private DateTime NextExpire;
        public int OfferValidityTime;
        private string OutputFilePath;
        private Thread UpdateOffersThread;
        private bool UpdatingOffers;

        public WSQoSOfferManager(Type ServiceType, string OutputFilePath)
        {
            this.OutputFilePath = "./Service.wsdl\uFFFD";
            UpdatingOffers = true;
            OfferValidityTime = 120000;
            this.OutputFilePath = OutputFilePath;
            Imports = (ImportQoSDeclarationAttribute[])Attribute.GetCustomAttributes(ServiceType, typeof(ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateOffersThread = new Thread(new ThreadStart(AutoUpdateOffers));
            UpdateOffersThread.Start();
        }

        public WSQoSOfferManager(Type ServiceType, string OutputFilePath, int OfferValidityTime)
        {
            this.OutputFilePath = "./Service.wsdl\uFFFD";
            UpdatingOffers = true;
            this.OfferValidityTime = 120000;
            this.OfferValidityTime = OfferValidityTime;
            this.OutputFilePath = OutputFilePath;
            Imports = (ImportQoSDeclarationAttribute[])Attribute.GetCustomAttributes(ServiceType, typeof(ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateOffersThread = new Thread(new ThreadStart(AutoUpdateOffers));
            UpdateOffersThread.Start();
        }

        public void AutoUpdateOffers()
        {
            while (true)
            {
                if (UpdatingOffers)
                    UpdateOffers();
                Thread.Sleep(OfferValidityTime);
            }
        }

        public void Dispose()
        {
            // trial
        }

        private string GetOffers(XmlElement Source)
        {
            // trial
            return null;
        }

        public void ResumeUpdatingOffers()
        {
            // trial
        }

        public void StopUpdatingOffers()
        {
            UpdatingOffers = false;
        }

        public void UpdateOffers()
        {
            DateTime dateTime = DateTime.Now;
            NextExpire = dateTime.AddMilliseconds((double)OfferValidityTime);
            StringBuilder stringBuilder1 = new StringBuilder();
            stringBuilder1.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><wsqos xmlns=\"http://wsqos.org/\"><definition><offers>\uFFFD");
            for (int i1 = 0; i1 < Imports.Length; i1++)
            {
                if (Imports[i1].GetXml() != null)
                    stringBuilder1.Append(GetOffers(Imports[i1].GetXml()));
            }
            stringBuilder1.Append("</offers></definition></wsqos>\uFFFD");
            XmlDocument xmlDocument1 = new XmlDocument();
            xmlDocument1.LoadXml(stringBuilder1.ToString());
            int i2 = OutputFilePath.LastIndexOf(".\uFFFD");
            string s1 = OutputFilePath.Substring(i2 + 1, OutputFilePath.Length - (i2 + 1));
            if (s1 == "wsdl\uFFFD")
            {
                XmlDocument xmlDocument2 = new XmlDocument();
                xmlDocument2.Load(OutputFilePath);
                XmlNodeList xmlNodeList1 = xmlDocument2.GetElementsByTagName("service\uFFFD");
                if (xmlNodeList1.Count <= 0)
                    return;
                XmlElement xmlElement = (XmlElement)xmlNodeList1[0];
                XmlNodeList xmlNodeList2 = xmlElement.GetElementsByTagName("wsqos\uFFFD");
                for (int i3 = 0; i3 < xmlNodeList2.Count; i3++)
                {
                    xmlElement.RemoveChild(xmlNodeList2[i3]);
                }
                StringBuilder stringBuilder2 = new StringBuilder();
                string s2 = xmlDocument2.OuterXml;
                int i4 = s2.LastIndexOf("</service>\uFFFD");
                string s3 = s2.Substring(0, i4);
                string s4 = s2.Substring(i4, s2.Length - i4);
                stringBuilder2.Append(s3);
                stringBuilder2.Append(xmlDocument1.FirstChild.NextSibling.OuterXml);
                stringBuilder2.Append(s4);
                string s5 = stringBuilder2.ToString();
                try
                {
                    xmlDocument2.LoadXml(s5);
                }
                catch (XmlException e)
                {
                    e.Message;
                    e.InnerException.Message;
                }
                xmlDocument2.Save(OutputFilePath);
                return;
            }
            xmlDocument1.Save(OutputFilePath);
        }

    } // class WSQoSOfferManager

}

