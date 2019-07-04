using System.Xml;

namespace WSQoSAPI
{

    public class QoSInfo : WSQoSAttribute
    {

        protected NamedItemCollection _extensions;
        protected NamedItemCollection _securityAndTransactions;
        protected ServerQoSMetricsAttribute _serverQoSMetrics;
        protected TransportQoSPrioritiesAttribute _transportQoSPriorities;

        public NamedItemCollection Extensions
        {
            get
            {
                return _extensions;
            }
            set
            {
                _extensions = value;
            }
        }

        public NamedItemCollection SecurityAndTransactions
        {
            get
            {
                return _securityAndTransactions;
            }
            set
            {
                _securityAndTransactions = value;
            }
        }

        public ServerQoSMetricsAttribute ServerQoSMetrics
        {
            get
            {
                return _serverQoSMetrics;
            }
            set
            {
                _serverQoSMetrics = value;
            }
        }

        public TransportQoSPrioritiesAttribute TransportQoSPriorities
        {
            get
            {
                return _transportQoSPriorities;
            }
            set
            {
                _transportQoSPriorities = value;
            }
        }

        public QoSInfo()
        {
            _serverQoSMetrics = new ServerQoSMetricsAttribute();
            _transportQoSPriorities = new TransportQoSPrioritiesAttribute();
            _securityAndTransactions = new NamedItemCollection();
            _extensions = new NamedItemCollection();
        }

        public QoSInfo(XmlNode src)
        {
            _serverQoSMetrics = new ServerQoSMetricsAttribute();
            _transportQoSPriorities = new TransportQoSPrioritiesAttribute();
            _securityAndTransactions = new NamedItemCollection();
            _extensions = new NamedItemCollection();
            _xml = src;
            base.UpdateProperties();
        }

        public SecurityAndTransactionAttribute GetSecurityAndTransaction(string Name)
        {
            return (SecurityAndTransactionAttribute)_securityAndTransactions.GetItemByName(Name);
        }

        public SecurityAndTransactionAttribute GetSecurityAndTransaction(int index)
        {
            // trial
            return null;
        }

        public bool HasProtocol(SecAndTASupportAttribute checkProt)
        {
            // trial
            return false;
        }

        public bool Includes(QoSInfo RequireInfo)
        {
            if ((RequireInfo.ServerQoSMetrics.ProcessingTime > 0.0) && (ServerQoSMetrics.ProcessingTime > RequireInfo.ServerQoSMetrics.ProcessingTime))
                return false;
            if ((RequireInfo.ServerQoSMetrics.RequestsPerSecond > 0.0) && (ServerQoSMetrics.RequestsPerSecond < RequireInfo.ServerQoSMetrics.RequestsPerSecond))
                return false;
            if ((RequireInfo.ServerQoSMetrics.Availability > 0.0) && (ServerQoSMetrics.Availability < RequireInfo.ServerQoSMetrics.Availability))
                return false;
            if ((RequireInfo.ServerQoSMetrics.Reliability > 0.0) && (ServerQoSMetrics.Reliability < RequireInfo.ServerQoSMetrics.Reliability))
                return false;
            for (int i1 = 0; i1 < RequireInfo.ServerQoSMetrics.CustomServerMetricsCount; i1++)
            {
                CustomServerQoSMetricAttribute customServerQoSMetricAttribute1 = RequireInfo.ServerQoSMetrics.GetCustomServerMetric(i1);
                CustomServerQoSMetricAttribute customServerQoSMetricAttribute2 = ServerQoSMetrics.GetCustomServerMetric(customServerQoSMetricAttribute1.Name, customServerQoSMetricAttribute1.OntologyUrl);
                if (customServerQoSMetricAttribute2 == null)
                    return false;
                WSQoSMetricDef wsqoSMetricDef = WSQoSOntologyManager.GetMetricDef(customServerQoSMetricAttribute2.Name, customServerQoSMetricAttribute2.OntologyUrl);
                if (wsqoSMetricDef.Increasing)
                {
                    if (customServerQoSMetricAttribute2.Value >= customServerQoSMetricAttribute1.Value)
                        goto label_1;
                    return false;
                }
                if (customServerQoSMetricAttribute2.Value > customServerQoSMetricAttribute1.Value)
                    return false;
            label_1:;
            }
            if ((RequireInfo.TransportQoSPriorities.Delay > 0) && (TransportQoSPriorities.Delay > RequireInfo.TransportQoSPriorities.Delay))
                return false;
            if ((RequireInfo.TransportQoSPriorities.Jitter > 0) && (TransportQoSPriorities.Jitter > RequireInfo.TransportQoSPriorities.Jitter))
                return false;
            if ((RequireInfo.TransportQoSPriorities.Throughput > 0) && (TransportQoSPriorities.Throughput > RequireInfo.TransportQoSPriorities.Throughput))
                return false;
            if ((RequireInfo.TransportQoSPriorities.PacketLoss > 0) && (TransportQoSPriorities.PacketLoss > RequireInfo.TransportQoSPriorities.PacketLoss))
                return false;
            for (int i2 = 0; i2 < RequireInfo.TransportQoSPriorities.CustomTransportPrioritiesCount; i2++)
            {
                CustomTransportQoSPriorityAttribute customTransportQoSPriorityAttribute1 = RequireInfo.TransportQoSPriorities.GetCustomTransportPriority(i2);
                CustomTransportQoSPriorityAttribute customTransportQoSPriorityAttribute2 = TransportQoSPriorities.GetCustomTransportPriority(customTransportQoSPriorityAttribute1.Name, customTransportQoSPriorityAttribute1.OntologyUrl);
                if ((customTransportQoSPriorityAttribute2 == null) || (customTransportQoSPriorityAttribute2.Value < 0))
                    return false;
                if (customTransportQoSPriorityAttribute2.Value > customTransportQoSPriorityAttribute1.Value)
                    return false;
            }
            if (!ProvidesRequiredProtocols(RequireInfo))
                return false;
            if (!RequireInfo.ProvidesRequiredProtocols(this))
                return false;
            return true;
        }

        public void JoinWith(QoSInfo newInfo)
        {
            // trial
        }

        public bool ProvidesRequiredProtocols(QoSInfo RequireInfo)
        {
            for (int i1 = 0; i1 < RequireInfo.SecurityAndTransactions.Count; i1++)
            {
                SecurityAndTransactionAttribute securityAndTransactionAttribute = RequireInfo.GetSecurityAndTransaction(i1);
                switch (securityAndTransactionAttribute.Requires)
                {
                    case "all\uFFFD":
                        for (int i2 = 0; i2 < securityAndTransactionAttribute.ProtocolCount; i2++)
                        {
                            if (!HasProtocol(securityAndTransactionAttribute.GetProtocol(i2)))
                                return false;
                        }
                        continue;

                    case "one\uFFFD":
                        bool flag = false;
                        for (int i3 = 0; i3 < securityAndTransactionAttribute.ProtocolCount; i3++)
                        {
                            if (HasProtocol(securityAndTransactionAttribute.GetProtocol(i3)))
                                flag = true;
                        }
                        if (!flag)
                            return false;
                        break;
                }
            }
            return true;
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            return "qosInfo\uFFFD";
        }

        protected override void UpdateProperties()
        {
            if (((XmlElement)_xml).HasAttribute("name\uFFFD"))
                _name = _xml.Attributes["name\uFFFD"].Value;
            XmlNodeList xmlNodeList1 = ((XmlElement)_xml).GetElementsByTagName("serverQoSMetrics\uFFFD");
            if (xmlNodeList1.Count > 0)
                _serverQoSMetrics = new ServerQoSMetricsAttribute(xmlNodeList1[0]);
            XmlNodeList xmlNodeList2 = ((XmlElement)_xml).GetElementsByTagName("transportQoSPriorities\uFFFD");
            if (xmlNodeList2.Count > 0)
                _transportQoSPriorities = new TransportQoSPrioritiesAttribute(xmlNodeList2[0]);
            _securityAndTransactions.Clear();
            XmlNodeList xmlNodeList3 = ((XmlElement)_xml).GetElementsByTagName("securityAndTransaction\uFFFD");
            for (int i1 = 0; i1 < xmlNodeList3.Count; i1++)
            {
                _securityAndTransactions.Add(new SecurityAndTransactionAttribute(xmlNodeList3[i1]));
            }
            _extensions.Clear();
            XmlNodeList xmlNodeList4 = ((XmlElement)_xml).GetElementsByTagName("extension\uFFFD");
            for (int i2 = 0; i2 < xmlNodeList4.Count; i2++)
            {
                _extensions.Add(new WSQoSExtensionAttribute(xmlNodeList4[i2]));
            }
            BuildXml();
        }

    } // class QoSInfo

}

