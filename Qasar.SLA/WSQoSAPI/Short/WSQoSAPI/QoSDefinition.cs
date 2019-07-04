using System.Text;
using System.Xml;

namespace WSQoSAPI
{

    public class QoSDefinition : WSQoSAttribute, NamedItem
    {

        protected NamedItemCollection _contractAndMonitoring;
        protected DefaultQoSInfo _defaultRequirements;
        protected NamedItemCollection _extensions;
        protected NamedItemCollection _operationRequirements;
        protected PriceAttribute _price;

        public DefaultQoSInfo DefaultRequirements
        {
            get
            {
                return _defaultRequirements;
            }
            set
            {
                _defaultRequirements = value;
            }
        }

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

        public NamedItemCollection OperationRequirements
        {
            get
            {
                return _operationRequirements;
            }
            set
            {
                _operationRequirements = value;
            }
        }

        public PriceAttribute Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }

        public QoSDefinition()
        {
            _defaultRequirements = new DefaultQoSInfo();
            _operationRequirements = new NamedItemCollection();
            _contractAndMonitoring = new NamedItemCollection();
            _extensions = new NamedItemCollection();
            _price = new PriceAttribute();
        }

        public QoSDefinition(string name)
        {
            _defaultRequirements = new DefaultQoSInfo();
            _operationRequirements = new NamedItemCollection();
            _contractAndMonitoring = new NamedItemCollection();
            _extensions = new NamedItemCollection();
            _price = new PriceAttribute();
            _name = name;
        }

        public QoSDefinition(XmlNode src)
        {
            _defaultRequirements = new DefaultQoSInfo();
            _operationRequirements = new NamedItemCollection();
            _contractAndMonitoring = new NamedItemCollection();
            _extensions = new NamedItemCollection();
            _price = new PriceAttribute();
            Update(src);
        }

        public ContractAndMonitoringAttribute GetContractAndMonitoring(int index)
        {
            // trial
            return null;
        }

        public ContractAndMonitoringAttribute GetContractAndMonitoring(string Name)
        {
            return (ContractAndMonitoringAttribute)_contractAndMonitoring.GetItemByName(Name);
        }

        public OperationQoSInfo GetJointOperationQoSInfo(string name)
        {
            OperationQoSInfo operationQoSInfo = new OperationQoSInfo(_defaultRequirements.Xml);
            operationQoSInfo.JoinWith(GetOperationQoSInfo(name));
            return operationQoSInfo;
        }

        public OperationQoSInfo GetOperationQoSInfo(int index)
        {
            return (OperationQoSInfo)_operationRequirements.ItemAt(index);
        }

        public OperationQoSInfo GetOperationQoSInfo(string name)
        {
            // trial
            return null;
        }

        public bool HasProtocol(ConAndMonSupportAttribute checkProt)
        {
            // trial
            return false;
        }

        public bool HasTrustedParty(TrustedParty checkParty)
        {
            for (int i = 0; i < _contractAndMonitoring.Count; i++)
            {
                if (GetContractAndMonitoring(i).GetTrustedParty(checkParty.Name, checkParty.Url) != null)
                    return true;
            }
            return false;
        }

        public void JoinWith(QoSDefinition newDef)
        {
            // trial
        }

        public bool ProvidesRequiredProtocolsAndParties(QoSDefinition RequireDef)
        {
            // trial
            return false;
        }

        public void SetName(string Name)
        {
            _name = Name;
        }

        protected override string GetInnerXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            _defaultRequirements.BuildXml();
            if (_defaultRequirements.Xml != null)
                stringBuilder.Append(_defaultRequirements.Xml.OuterXml);
            for (int i1 = 0; i1 < _operationRequirements.Count; i1++)
            {
                OperationQoSInfo operationQoSInfo = GetOperationQoSInfo(i1);
                operationQoSInfo.BuildXml();
                if (operationQoSInfo.Xml != null)
                    stringBuilder.Append(operationQoSInfo.Xml.OuterXml);
            }
            for (int i2 = 0; i2 < _contractAndMonitoring.Count; i2++)
            {
                ContractAndMonitoringAttribute contractAndMonitoringAttribute = GetContractAndMonitoring(i2);
                contractAndMonitoringAttribute.BuildXml();
                if (contractAndMonitoringAttribute.Xml != null)
                    stringBuilder.Append(contractAndMonitoringAttribute.Xml.OuterXml);
            }
            _price.BuildXml();
            stringBuilder.Append(_price.Xml.OuterXml);
            if (_extensions != null)
            {
                for (int i3 = 0; i3 < _extensions.Count; i3++)
                {
                    stringBuilder.Append(((WSQoSExtensionAttribute)_extensions.ItemAt(i3)).Xml.OuterXml);
                }
            }
            return stringBuilder.ToString();
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        public virtual bool Includes(QoSRequirements RequireDef)
        {
            if (RequireDef.Price.Value < 0.0)
                return false;
            if (RequireDef.Price.IsCheaperThan(Price))
                return false;
            if (!DefaultRequirements.Includes(RequireDef.DefaultRequirements))
                return false;
            for (int i = 0; i < RequireDef.OperationRequirements.Count; i++)
            {
                QoSInfo qoSInfo1 = GetJointOperationQoSInfo(RequireDef.GetOperationQoSInfo(i).Name);
                QoSInfo qoSInfo2 = RequireDef.GetJointOperationQoSInfo(RequireDef.GetOperationQoSInfo(i).Name);
                if (!qoSInfo1.Includes(qoSInfo2))
                    return false;
            }
            if (!ProvidesRequiredProtocolsAndParties(RequireDef))
                return false;
            if (!RequireDef.ProvidesRequiredProtocolsAndParties(this))
                return false;
            return true;
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class QoSDefinition

}

