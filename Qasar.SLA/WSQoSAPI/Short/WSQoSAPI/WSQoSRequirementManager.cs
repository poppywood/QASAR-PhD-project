using System;
using System.Reflection;
using System.Threading;
using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSRequirementManager
    {

        private QoSRequirements _currentrequirements;
        private QoSRequirements _staticRequirements;
        private ImportQoSDeclarationAttribute[] Imports;
        private string LogFilePath;
        private bool Logging;
        public int UpdateInterval;
        private Thread UpdateRequirementsThread;

        public QoSRequirements CurrentRequirements
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
            _staticRequirements = WSQoSRequirementManager.loadStaticRequirements(ServiceObject);
            Imports = (ImportQoSDeclarationAttribute[])Attribute.GetCustomAttributes(ServiceObject.GetType(), typeof(ImportQoSDeclarationAttribute));
            for (int i = 0; i < Imports.Length; i++)
            {
                Imports[i].LoadFile();
            }
            UpdateRequirementsThread = new Thread(new ThreadStart(UpdateRequirements));
            UpdateRequirementsThread.Name = "Update Requirements for Service Proxy \uFFFD" + ServiceObject.GetType().Name;
            UpdateRequirementsThread.Priority = ThreadPriority.Lowest;
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
                QoSDefinition qoSDefinition = new QoSDefinition();
                for (int i = 0; i < Imports.Length; i++)
                {
                    qoSDefinition.JoinWith(new QoSDefinition(Imports[i].GetXml()));
                }
                QoSRequirements qoSRequirements = new QoSRequirements(_staticRequirements.Xml);
                qoSRequirements.JoinWith(qoSDefinition);
                _currentrequirements = qoSRequirements;
                if (Logging)
                {
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml("<?xml version=\"1.0\" encoding=\"utf-8\" ?><wsqos xmlsn=\"http://wsqos.org\"><definition>\uFFFD" + _currentrequirements.Xml.OuterXml + "</definition></wsqos>\uFFFD");
                    xmlDocument.Save(LogFilePath);
                }
                Thread.Sleep(UpdateInterval);
            }
        }

        private static QoSInfo loadStaticOperationRequirements(MethodInfo m)
        {
            // trial
            return null;
        }

        public static QoSRequirements loadStaticRequirements(object src)
        {
            QoSRequirements qoSRequirements = new QoSRequirements(src.GetType().Name + "-Requirements\uFFFD");
            qoSRequirements.DefaultRequirements = new DefaultQoSInfo();
            ServerQoSMetricsAttribute serverQoSMetricsAttribute = (ServerQoSMetricsAttribute)Attribute.GetCustomAttribute(src.GetType(), typeof(ServerQoSMetricsAttribute));
            CustomServerQoSMetricAttribute[] customServerQoSMetricAttributeArr = (CustomServerQoSMetricAttribute[])Attribute.GetCustomAttributes(src.GetType(), typeof(CustomServerQoSMetricAttribute));
            if ((serverQoSMetricsAttribute == null) && (customServerQoSMetricAttributeArr.Length > 0))
                serverQoSMetricsAttribute = new ServerQoSMetricsAttribute();
            for (int i1 = 0; i1 < customServerQoSMetricAttributeArr.Length; i1++)
            {
                serverQoSMetricsAttribute.AddCustomServerMetric(customServerQoSMetricAttributeArr[i1]);
            }
            if (serverQoSMetricsAttribute != null)
                qoSRequirements.DefaultRequirements.ServerQoSMetrics = serverQoSMetricsAttribute;
            TransportQoSPrioritiesAttribute transportQoSPrioritiesAttribute = (TransportQoSPrioritiesAttribute)Attribute.GetCustomAttribute(src.GetType(), typeof(TransportQoSPrioritiesAttribute));
            CustomTransportQoSPriorityAttribute[] customTransportQoSPriorityAttributeArr = (CustomTransportQoSPriorityAttribute[])Attribute.GetCustomAttributes(src.GetType(), typeof(CustomTransportQoSPriorityAttribute));
            if ((transportQoSPrioritiesAttribute == null) && (customTransportQoSPriorityAttributeArr.Length > 0))
                transportQoSPrioritiesAttribute = new TransportQoSPrioritiesAttribute();
            for (int i2 = 0; i2 < customTransportQoSPriorityAttributeArr.Length; i2++)
            {
                transportQoSPrioritiesAttribute.AddCustomTransportPriority(customTransportQoSPriorityAttributeArr[i2]);
            }
            if (transportQoSPrioritiesAttribute != null)
                qoSRequirements.DefaultRequirements.TransportQoSPriorities = transportQoSPrioritiesAttribute;
            SecAndTASupportAttribute[] secAndTASupportAttributeArr = (SecAndTASupportAttribute[])Attribute.GetCustomAttributes(src.GetType(), typeof(SecAndTASupportAttribute));
            if (secAndTASupportAttributeArr.Length > 0)
            {
                SecurityAndTransactionAttribute securityAndTransactionAttribute = new SecurityAndTransactionAttribute("general\uFFFD");
                for (int i3 = 0; i3 < secAndTASupportAttributeArr.Length; i3++)
                {
                    securityAndTransactionAttribute.AddProtocol(secAndTASupportAttributeArr[i3]);
                }
                qoSRequirements.DefaultRequirements.SecurityAndTransactions.Add(securityAndTransactionAttribute);
            }
            MethodInfo[] methodInfoArr = src.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
            for (int i4 = 0; i4 < methodInfoArr.Length; i4++)
            {
                if (methodInfoArr[i4].GetCustomAttributes(typeof(WSQoSAttribute), false).Length > 0)
                    qoSRequirements.OperationRequirements.Add(WSQoSRequirementManager.loadStaticOperationRequirements(methodInfoArr[i4]));
            }
            PriceAttribute priceAttribute = (PriceAttribute)Attribute.GetCustomAttribute(src.GetType(), typeof(PriceAttribute));
            if (priceAttribute == null)
                priceAttribute = new PriceAttribute();
            qoSRequirements.Price = priceAttribute;
            return qoSRequirements;
        }

    } // class WSQoSRequirementManager

}

