using System.Reflection;
using System.Web.Services;
using System.Web.Services.Description;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSReflector : System.Web.Services.Description.SoapExtensionReflector
    {

        private static int allcallcount;
        private static int callcount;

        static WSQoSReflector()
        {
            WSQoSAPI.WSQoSReflector.allcallcount = -1;
            WSQoSAPI.WSQoSReflector.callcount = 0;
        }

        public WSQoSReflector()
        {
        }

        public override void ReflectMethod()
        {
            if (WSQoSAPI.WSQoSReflector.allcallcount == -1)
            {
                WSQoSAPI.WSQoSReflector.allcallcount = 0;
                System.Reflection.MethodInfo[] methodInfoArr1 = ReflectionContext.ServiceType.GetMethods();
                System.Reflection.MethodInfo[] methodInfoArr2 = methodInfoArr1;
                for (int i2 = 0; i2 < methodInfoArr2.Length; i2++)
                {
                    System.Reflection.MethodInfo methodInfo = methodInfoArr2[i2];
                    if (methodInfo.GetCustomAttributes(typeof(System.Web.Services.WebMethodAttribute), true).Length > 0)
                        WSQoSAPI.WSQoSReflector.allcallcount++;
                }
            }
            WSQoSAPI.WSQoSReflector.callcount++;
            if (WSQoSAPI.WSQoSReflector.callcount == (WSQoSAPI.WSQoSReflector.allcallcount * 2))
            {
                WSQoSAPI.ImportOffersAttribute[] importOffersAttributeArr = (WSQoSAPI.ImportOffersAttribute[])ReflectionContext.Method.DeclaringType.GetCustomAttributes(typeof(WSQoSAPI.ImportOffersAttribute), true);
                for (int i1 = 0; i1 < importOffersAttributeArr.Length; i1++)
                {
                    WSQoSAPI.WSQoSExtension wsqoSExtension = new WSQoSAPI.WSQoSExtension();
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    try
                    {
                        xmlDocument.Load(importOffersAttributeArr[i1].Path);
                        System.Xml.XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("offers\uFFFD");
                        if (xmlNodeList.Count > 0)
                            wsqoSExtension.offers = (System.Xml.XmlElement)xmlNodeList[0].FirstChild;
                    }
                    catch (System.Xml.XmlException e)
                    {
                        //e.Message;
                    }
                    ReflectionContext.Service.Ports[0].Extensions.Add(wsqoSExtension);
                }
            }
        }

    } // class WSQoSReflector

}

