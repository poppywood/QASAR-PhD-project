using System;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = false)]
    public class TransportQoSPrioritiesAttribute : WSQoSAttribute
    {

        private NamedItemCollection _CustomPriorities;
        private int _delay;
        private int _jitter;
        private int _packetLoss;
        private int _throughput;

        public int CustomTransportPrioritiesCount
        {
            get
            {
                return _CustomPriorities.Count;
            }
        }

        public int Delay
        {
            get
            {
                return _delay;
            }
            set
            {
                _delay = value;
                BuildXml();
            }
        }

        public int Jitter
        {
            get
            {
                return _jitter;
            }
            set
            {
                _jitter = value;
                BuildXml();
            }
        }

        public int PacketLoss
        {
            get
            {
                return _packetLoss;
            }
            set
            {
                _packetLoss = value;
                BuildXml();
            }
        }

        public int Throughput
        {
            get
            {
                return _throughput;
            }
            set
            {
                _throughput = value;
                BuildXml();
            }
        }

        public TransportQoSPrioritiesAttribute()
        {
            _delay = 0;
            _jitter = 0;
            _throughput = 0;
            _packetLoss = 0;
            _CustomPriorities = new NamedItemCollection();
        }

        public TransportQoSPrioritiesAttribute(XmlNode src)
        {
            _delay = 0;
            _jitter = 0;
            _throughput = 0;
            _packetLoss = 0;
            _CustomPriorities = new NamedItemCollection();
            Update(src);
        }

        public void AddCustomTransportPriority(CustomTransportQoSPriorityAttribute newPriority)
        {
            _CustomPriorities.Add(newPriority);
        }

        public CustomTransportQoSPriorityAttribute GetCustomTransportPriority(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public CustomTransportQoSPriorityAttribute GetCustomTransportPriority(int index)
        {
            return (CustomTransportQoSPriorityAttribute)_CustomPriorities.ItemAt(index);
        }

        public void RemoveCustomTransportPriority(CustomTransportQoSPriorityAttribute toBeRemoved)
        {
            // trial
        }

        protected override string GetInnerXml()
        {
            // trial
            return null;
        }

        protected override string GetRootLabel()
        {
            return "transportQoSPriorities\uFFFD";
        }

        protected override void UpdateProperties()
        {
            try
            {
                _delay = Int32.Parse(_xml["delay\uFFFD"].InnerText);
            }
            catch (NullReferenceException e1)
            {
                e1.Message;
            }
            try
            {
                _jitter = Int32.Parse(_xml["jitter\uFFFD"].InnerText);
            }
            catch (NullReferenceException e2)
            {
                e2.Message;
            }
            try
            {
                _throughput = Int32.Parse(_xml["throughput\uFFFD"].InnerText);
            }
            catch (NullReferenceException e3)
            {
                e3.Message;
            }
            try
            {
                _packetLoss = Int32.Parse(_xml["packetLoss\uFFFD"].InnerText);
            }
            catch (NullReferenceException e4)
            {
                e4.Message;
            }
            _CustomPriorities.Clear();
            XmlNodeList xmlNodeList = ((XmlElement)_xml).GetElementsByTagName("customPriority\uFFFD");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                CustomTransportQoSPriorityAttribute customTransportQoSPriorityAttribute = new CustomTransportQoSPriorityAttribute(xmlNodeList[i]);
                _CustomPriorities.Add(customTransportQoSPriorityAttribute);
            }
            BuildXml();
        }

    } // class TransportQoSPrioritiesAttribute

}

