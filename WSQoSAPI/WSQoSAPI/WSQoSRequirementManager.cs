using System;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSRequirementManager
    {

        private WSQoSAPI.QoSRequirements _currentrequirements;
        private WSQoSAPI.QoSRequirements _staticRequirements;
        private WSQoSAPI.ImportQoSDeclarationAttribute[] Imports;
        private string LogFilePath;
        private bool Logging;
        public int UpdateInterval;
        private System.Threading.Thread UpdateRequirementsThread;

        public WSQoSAPI.QoSRequirements CurrentRequirements
        {
            get
            {
                return _currentrequirements;
            }
        }

        public WSQoSRequirementManager(object ServiceObject)
        {
            LogFilePath = "./CurrentRequirementsLog.wsqos\uFFFD";
            Logging = false;
            UpdateInterval = 10000;
            LogFilePath = "Current\uFFFD" + ServiceObject.GetType().Name + "CurrentRequirements.wsqos\uFFFD";
            _staticRequirements = WSQoSAPI.WSQoSRequirementManager.loadStaticRequirements(ServiceObject);
            Imports = (WSQoSAPI.ImportQoSDeclarationAttribute[])System.Attribute.GetCustomAttributes(ServiceObject.GetType(), typeof(WSQoSAPI.ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateRequirementsThread = new System.Threading.Thread(new System.Threading.ThreadStart(UpdateRequirements));
            UpdateRequirementsThread.Name = "Update Requirements for Service Proxy \uFFFD" + ServiceObject.GetType().Name;
            UpdateRequirementsThread.Priority = System.Threading.ThreadPriority.Lowest;
            UpdateRequirementsThread.Start();
        }

        public void Dispose()
        {
            // trial
        }

        public void StartLogging()
        {
            // trial
        }

        public void StopLogging()
        {
            Logging = false;
        }

        public void UpdateRequirements()
        {
            while (true)
            {
                WSQoSAPI.QoSDefinition qoSDefinition = new WSQoSAPI.QoSDefinition();
                for (int i = 0; i < Imports.Length; i++)
                {
                    qoSDefinition.JoinWith(new WSQoSAPI.QoSDefinition(Imports[i].GetXml()));
                }
                WSQoSAPI.QoSRequirements qoSRequirements = new WSQoSAPI.QoSRequirements(_staticRequirements.Xml);
                qoSRequirements.JoinWith(qoSDefinition);
                _currentrequirements = qoSRequirements;
                if (Logging)
                {
                    System.Xml.XmlDocument xmlDocument = new System.Xml.XmlDocument();
                    xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><wsqos xmlsn=\"http://wsqos.org\"><definition>\uFFFD" + _currentrequirements.Xml.OuterXml + "</definition></wsqos>\uFFFD");
                    xmlDocument.Save(LogFilePath);
                }
                System.Threading.Thread.Sleep(UpdateInterval);
            }
        }

        private static WSQoSAPI.QoSInfo loadStaticOperationRequirements(System.Reflection.MethodInfo m)
        {
            // trial
            return null;
        }

        public static WSQoSAPI.QoSRequirements loadStaticRequirements(object src)
        {
            WSQoSAPI.QoSRequirements qoSRequirements = new WSQoSAPI.QoSRequirements(src.GetType().Name + "-Requirements\uFFFD");
            qoSRequirements.DefaultRequirements = new WSQoSAPI.DefaultQoSInfo();
            WSQoSAPI.ServerQoSMetricsAttribute serverQoSMetricsAttribute = (WSQoSAPI.ServerQoSMetricsAttribute)System.Attribute.GetCustomAttribute(src.GetType(), typeof(WSQoSAPI.ServerQoSMetricsAttribute));
            WSQoSAPI.CustomServerQoSMetricAttribute[] customServerQoSMetricAttributeArr = (WSQoSAPI.CustomServerQoSMetricAttribute[])System.Attribute.GetCustomAttributes(src.GetType(), typeof(WSQoSAPI.CustomServerQoSMetricAttribute));
            if ((serverQoSMetricsAttribute == null) && (customServerQoSMetricAttributeArr.Length > 0))
                serverQoSMetricsAttribute = new WSQoSAPI.ServerQoSMetricsAttribute();
            for (int i1 = 0; i1 < customServerQoSMetricAttributeArr.Length; i1++)
            {
                serverQoSMetricsAttribute.AddCustomServerMetric(customServerQoSMetricAttributeArr[i1]);
            }
            if (serverQoSMetricsAttribute != null)
                qoSRequirements.DefaultRequirements.ServerQoSMetrics = serverQoSMetricsAttribute;
            WSQoSAPI.TransportQoSPrioritiesAttribute transportQoSPrioritiesAttribute = (WSQoSAPI.TransportQoSPrioritiesAttribute)System.Attribute.GetCustomAttribute(src.GetType(), typeof(WSQoSAPI.TransportQoSPrioritiesAttribute));
            WSQoSAPI.CustomTransportQoSPriorityAttribute[] customTransportQoSPriorityAttributeArr = (WSQoSAPI.CustomTransportQoSPriorityAttribute[])System.Attribute.GetCustomAttributes(src.GetType(), typeof(WSQoSAPI.CustomTransportQoSPriorityAttribute));
            if ((transportQoSPrioritiesAttribute == null) && (customTransportQoSPriorityAttributeArr.Length > 0))
                transportQoSPrioritiesAttribute = new WSQoSAPI.TransportQoSPrioritiesAttribute();
            for (int i2 = 0; i2 < customTransportQoSPriorityAttributeArr.Length; i2++)
            {
                transportQoSPrioritiesAttribute.AddCustomTransportPriority(customTransportQoSPriorityAttributeArr[i2]);
            }
            if (transportQoSPrioritiesAttribute != null)
                qoSRequirements.DefaultRequirements.TransportQoSPriorities = transportQoSPrioritiesAttribute;
            WSQoSAPI.SecAndTASupportAttribute[] secAndTASupportAttributeArr = (WSQoSAPI.SecAndTASupportAttribute[])System.Attribute.GetCustomAttributes(src.GetType(), typeof(WSQoSAPI.SecAndTASupportAttribute));
            if (secAndTASupportAttributeArr.Length > 0)
            {
                WSQoSAPI.SecurityAndTransactionAttribute securityAndTransactionAttribute = new WSQoSAPI.SecurityAndTransactionAttribute("general\uFFFD");
                for (int i3 = 0; i3 < secAndTASupportAttributeArr.Length; i3++)
                {
                    securityAndTransactionAttribute.AddProtocol(secAndTASupportAttributeArr[i3]);
                }
                qoSRequirements.DefaultRequirements.SecurityAndTransactions.Add(securityAndTransactionAttribute);
            }
            System.Reflection.MethodInfo[] methodInfoArr = src.GetType().GetMethods(System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            for (int i4 = 0; i4 < methodInfoArr.Length; i4++)
            {
                if (methodInfoArr[i4].GetCustomAttributes(typeof(WSQoSAPI.WSQoSAttribute), false).Length > 0)
                    qoSRequirements.OperationRequirements.Add(WSQoSAPI.WSQoSRequirementManager.loadStaticOperationRequirements(methodInfoArr[i4]));
            }
            WSQoSAPI.PriceAttribute priceAttribute = (WSQoSAPI.PriceAttribute)System.Attribute.GetCustomAttribute(src.GetType(), typeof(WSQoSAPI.PriceAttribute));
            if (priceAttribute == null)
                priceAttribute = new WSQoSAPI.PriceAttribute();
            qoSRequirements.Price = priceAttribute;
            return qoSRequirements;
        }

    } // class WSQoSRequirementManager

}

