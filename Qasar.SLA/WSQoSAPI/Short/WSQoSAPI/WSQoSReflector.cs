using System.Reflection;
using System.Web.Services;
using System.Web.Services.Description;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSReflector : SoapExtensionReflector
    {

        private static int allcallcount;
        private static int callcount;

        static WSQoSReflector()
        {
            WSQoSReflector.allcallcount = -1;
            WSQoSReflector.callcount = 0;
        }

        public WSQoSReflector()
        {
        }

        public override void ReflectMethod()
        {
            if (WSQoSReflector.allcallcount == -1)
            {
                WSQoSReflector.allcallcount = 0;
                MethodInfo[] methodInfoArr1 = ReflectionContext.ServiceType.GetMethods();
                MethodInfo[] methodInfoArr2 = methodInfoArr1;
                for (int i2 = 0; i2 < methodInfoArr2.Length; i2++)
                {
                    MethodInfo methodInfo = methodInfoArr2[i2];
                    if (methodInfo.GetCustomAttributes(typeof(WebMethodAttribute), true).Length > 0)
                        WSQoSReflector.allcallcount++;
                }
            }
            WSQoSReflector.callcount++;
            if (WSQoSReflector.callcount == (WSQoSReflector.allcallcount * 2))
            {
                ImportOffersAttribute[] importOffersAttributeArr = (ImportOffersAttribute[])ReflectionContext.Method.DeclaringType.GetCustomAttributes(typeof(ImportOffersAttribute), true);
                for (int i1 = 0; i1 < importOffersAttributeArr.Length; i1++)
                {
                    WSQoSExtension wsqoSExtension = new WSQoSExtension();
                    XmlDocument xmlDocument = new XmlDocument();
                    try
                    {
                        xmlDocument.Load(importOffersAttributeArr[i1].Path);
                        XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("offers\uFFFD");
                        if (xmlNodeList.Count > 0)
                            wsqoSExtension.offers = (XmlElement)xmlNodeList[0].FirstChild;
                    }
                    catch (XmlException e)
                    {
                        e.Message;
                    }
                    ReflectionContext.Service.Ports[0].Extensions.Add(wsqoSExtension);
                }
            }
        }

    } // class WSQoSReflector

}

