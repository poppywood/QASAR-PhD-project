using System;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = false)]
    public class TransportQoSPrioritiesAttribute : WSQoSAPI.WSQoSAttribute
    {

        private WSQoSAPI.NamedItemCollection _CustomPriorities;
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
            _CustomPriorities = new WSQoSAPI.NamedItemCollection();
        }

        public TransportQoSPrioritiesAttribute(System.Xml.XmlNode src)
        {
            _delay = 0;
            _jitter = 0;
            _throughput = 0;
            _packetLoss = 0;
            _CustomPriorities = new WSQoSAPI.NamedItemCollection();
            Update(src);
        }

        public void AddCustomTransportPriority(WSQoSAPI.CustomTransportQoSPriorityAttribute newPriority)
        {
            _CustomPriorities.Add(newPriority);
        }

        public WSQoSAPI.CustomTransportQoSPriorityAttribute GetCustomTransportPriority(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public WSQoSAPI.CustomTransportQoSPriorityAttribute GetCustomTransportPriority(int index)
        {
            return (WSQoSAPI.CustomTransportQoSPriorityAttribute)_CustomPriorities.ItemAt(index);
        }

        public void RemoveCustomTransportPriority(WSQoSAPI.CustomTransportQoSPriorityAttribute toBeRemoved)
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
                _delay = System.Int32.Parse(_xml["delay\uFFFD"].InnerText);
            }
            catch (System.NullReferenceException e1)
            {
                //e1.Message;
            }
            try
            {
                _jitter = System.Int32.Parse(_xml["jitter\uFFFD"].InnerText);
            }
            catch (System.NullReferenceException e2)
            {
                ///e2.Message;
            }
            try
            {
                _throughput = System.Int32.Parse(_xml["throughput\uFFFD"].InnerText);
            }
            catch (System.NullReferenceException e3)
            {
                //e3.Message;
            }
            try
            {
                _packetLoss = System.Int32.Parse(_xml["packetLoss\uFFFD"].InnerText);
            }
            catch (System.NullReferenceException e4)
            {
                //e4.Message;
            }
            _CustomPriorities.Clear();
            System.Xml.XmlNodeList xmlNodeList = ((System.Xml.XmlElement)_xml).GetElementsByTagName("customPriority\uFFFD");
            for (int i = 0; i < xmlNodeList.Count; i++)
            {
                WSQoSAPI.CustomTransportQoSPriorityAttribute customTransportQoSPriorityAttribute = new WSQoSAPI.CustomTransportQoSPriorityAttribute(xmlNodeList[i]);
                _CustomPriorities.Add(customTransportQoSPriorityAttribute);
            }
            BuildXml();
        }

    } // class TransportQoSPrioritiesAttribute

}

