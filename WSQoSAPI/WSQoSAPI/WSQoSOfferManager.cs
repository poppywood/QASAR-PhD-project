using System;
using System.Text;
using System.Threading;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSOfferManager
    {

        private WSQoSAPI.ImportQoSDeclarationAttribute[] Imports;
        private System.DateTime NextExpire;
        public int OfferValidityTime;
        private string OutputFilePath;
        private System.Threading.Thread UpdateOffersThread;
        private bool UpdatingOffers;

        public WSQoSOfferManager(System.Type ServiceType, string OutputFilePath)
        {
            this.OutputFilePath = "./Service.wsdl\uFFFD";
            UpdatingOffers = true;
            OfferValidityTime = 120000;
            this.OutputFilePath = OutputFilePath;
            Imports = (WSQoSAPI.ImportQoSDeclarationAttribute[])System.Attribute.GetCustomAttributes(ServiceType, typeof(WSQoSAPI.ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateOffersThread = new System.Threading.Thread(new System.Threading.ThreadStart(AutoUpdateOffers));
            UpdateOffersThread.Start();
        }

        public WSQoSOfferManager(System.Type ServiceType, string OutputFilePath, int OfferValidityTime)
        {
            this.OutputFilePath = "./Service.wsdl\uFFFD";
            UpdatingOffers = true;
            this.OfferValidityTime = 120000;
            this.OfferValidityTime = OfferValidityTime;
            this.OutputFilePath = OutputFilePath;
            Imports = (WSQoSAPI.ImportQoSDeclarationAttribute[])System.Attribute.GetCustomAttributes(ServiceType, typeof(WSQoSAPI.ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateOffersThread = new System.Threading.Thread(new System.Threading.ThreadStart(AutoUpdateOffers));
            UpdateOffersThread.Start();
        }

        public void AutoUpdateOffers()
        {
            while (true)
            {
                if (UpdatingOffers)
                    UpdateOffers();
                System.Threading.Thread.Sleep(OfferValidityTime);
            }
        }

        public void Dispose()
        {
            // trial
        }

        private string GetOffers(System.Xml.XmlElement Source)
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
            System.DateTime dateTime = System.DateTime.Now;
            NextExpire = dateTime.AddMilliseconds((double)OfferValidityTime);
            System.Text.StringBuilder stringBuilder1 = new System.Text.StringBuilder();
            stringBuilder1.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><wsqos xmlns=\"http://wsqos.org/\"><definition><offers>\uFFFD");
            for (int i1 = 0; i1 < Imports.Length; i1++)
            {
                if (Imports[i1].GetXml() != null)
                    stringBuilder1.Append(GetOffers(Imports[i1].GetXml()));
            }
            stringBuilder1.Append("</offers></definition></wsqos>\uFFFD");
            System.Xml.XmlDocument xmlDocument1 = new System.Xml.XmlDocument();
            xmlDocument1.LoadXml(stringBuilder1.ToString());
            int i2 = OutputFilePath.LastIndexOf(".\uFFFD");
            string s1 = OutputFilePath.Substring(i2 + 1, OutputFilePath.Length - (i2 + 1));
            if (s1 == "wsdl\uFFFD")
            {
                System.Xml.XmlDocument xmlDocument2 = new System.Xml.XmlDocument();
                xmlDocument2.Load(OutputFilePath);
                System.Xml.XmlNodeList xmlNodeList1 = xmlDocument2.GetElementsByTagName("service\uFFFD");
                if (xmlNodeList1.Count <= 0)
                    return;
                System.Xml.XmlElement xmlElement = (System.Xml.XmlElement)xmlNodeList1[0];
                System.Xml.XmlNodeList xmlNodeList2 = xmlElement.GetElementsByTagName("wsqos\uFFFD");
                for (int i3 = 0; i3 < xmlNodeList2.Count; i3++)
                {
                    xmlElement.RemoveChild(xmlNodeList2[i3]);
                }
                System.Text.StringBuilder stringBuilder2 = new System.Text.StringBuilder();
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
                catch (System.Xml.XmlException e)
                {
                    //e.Message;
                    //e.InnerException.Message;
                }
                xmlDocument2.Save(OutputFilePath);
                return;
            }
            xmlDocument1.Save(OutputFilePath);
        }

    } // class WSQoSOfferManager

}

