using System.Text;
using System.Xml;

namespace WSQoSAPI
{

    public class QoSDefinition : WSQoSAPI.WSQoSAttribute, WSQoSAPI.NamedItem
    {

        protected WSQoSAPI.NamedItemCollection _contractAndMonitoring;
        protected WSQoSAPI.DefaultQoSInfo _defaultRequirements;
        protected WSQoSAPI.NamedItemCollection _extensions;
        protected WSQoSAPI.NamedItemCollection _operationRequirements;
        protected WSQoSAPI.PriceAttribute _price;

        public WSQoSAPI.DefaultQoSInfo DefaultRequirements
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

        public WSQoSAPI.NamedItemCollection Extensions
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

        public WSQoSAPI.NamedItemCollection OperationRequirements
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

        public WSQoSAPI.PriceAttribute Price
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
            _defaultRequirements = new WSQoSAPI.DefaultQoSInfo();
            _operationRequirements = new WSQoSAPI.NamedItemCollection();
            _contractAndMonitoring = new WSQoSAPI.NamedItemCollection();
            _extensions = new WSQoSAPI.NamedItemCollection();
            _price = new WSQoSAPI.PriceAttribute();
        }

        public QoSDefinition(string name)
        {
            _defaultRequirements = new WSQoSAPI.DefaultQoSInfo();
            _operationRequirements = new WSQoSAPI.NamedItemCollection();
            _contractAndMonitoring = new WSQoSAPI.NamedItemCollection();
            _extensions = new WSQoSAPI.NamedItemCollection();
            _price = new WSQoSAPI.PriceAttribute();
            _name = name;
        }

        public QoSDefinition(System.Xml.XmlNode src)
        {
            _defaultRequirements = new WSQoSAPI.DefaultQoSInfo();
            _operationRequirements = new WSQoSAPI.NamedItemCollection();
            _contractAndMonitoring = new WSQoSAPI.NamedItemCollection();
            _extensions = new WSQoSAPI.NamedItemCollection();
            _price = new WSQoSAPI.PriceAttribute();
            Update(src);
        }

        public WSQoSAPI.ContractAndMonitoringAttribute GetContractAndMonitoring(int index)
        {
            // trial
            return null;
        }

        public WSQoSAPI.ContractAndMonitoringAttribute GetContractAndMonitoring(string Name)
        {
            return (WSQoSAPI.ContractAndMonitoringAttribute)_contractAndMonitoring.GetItemByName(Name);
        }

        public WSQoSAPI.OperationQoSInfo GetJointOperationQoSInfo(string name)
        {
            WSQoSAPI.OperationQoSInfo operationQoSInfo = new WSQoSAPI.OperationQoSInfo(_defaultRequirements.Xml);
            operationQoSInfo.JoinWith(GetOperationQoSInfo(name));
            return operationQoSInfo;
        }

        public WSQoSAPI.OperationQoSInfo GetOperationQoSInfo(int index)
        {
            return (WSQoSAPI.OperationQoSInfo)_operationRequirements.ItemAt(index);
        }

        public WSQoSAPI.OperationQoSInfo GetOperationQoSInfo(string name)
        {
            // trial
            return null;
        }

        public bool HasProtocol(WSQoSAPI.ConAndMonSupportAttribute checkProt)
        {
            // trial
            return false;
        }

        public bool HasTrustedParty(WSQoSAPI.TrustedParty checkParty)
        {
            for (int i = 0; i < _contractAndMonitoring.Count; i++)
            {
                if (GetContractAndMonitoring(i).GetTrustedParty(checkParty.Name, checkParty.Url) != null)
                    return true;
            }
            return false;
        }

        public void JoinWith(WSQoSAPI.QoSDefinition newDef)
        {
            // trial
        }

        public bool ProvidesRequiredProtocolsAndParties(WSQoSAPI.QoSDefinition RequireDef)
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
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            _defaultRequirements.BuildXml();
            if (_defaultRequirements.Xml != null)
                stringBuilder.Append(_defaultRequirements.Xml.OuterXml);
            for (int i1 = 0; i1 < _operationRequirements.Count; i1++)
            {
                WSQoSAPI.OperationQoSInfo operationQoSInfo = GetOperationQoSInfo(i1);
                operationQoSInfo.BuildXml();
                if (operationQoSInfo.Xml != null)
                    stringBuilder.Append(operationQoSInfo.Xml.OuterXml);
            }
            for (int i2 = 0; i2 < _contractAndMonitoring.Count; i2++)
            {
                WSQoSAPI.ContractAndMonitoringAttribute contractAndMonitoringAttribute = GetContractAndMonitoring(i2);
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
                    stringBuilder.Append(((WSQoSAPI.WSQoSExtensionAttribute)_extensions.ItemAt(i3)).Xml.OuterXml);
                }
            }
            return stringBuilder.ToString();
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        public virtual bool Includes(WSQoSAPI.QoSRequirements RequireDef)
        {
            if (RequireDef.Price.Value < 0.0)
                return false;
            if (RequireDef.Price.IsCheaperThan(Price))
                return false;
            if (!DefaultRequirements.Includes(RequireDef.DefaultRequirements))
                return false;
            for (int i = 0; i < RequireDef.OperationRequirements.Count; i++)
            {
                WSQoSAPI.QoSInfo qoSInfo1 = GetJointOperationQoSInfo(RequireDef.GetOperationQoSInfo(i).Name);
                WSQoSAPI.QoSInfo qoSInfo2 = RequireDef.GetJointOperationQoSInfo(RequireDef.GetOperationQoSInfo(i).Name);
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

