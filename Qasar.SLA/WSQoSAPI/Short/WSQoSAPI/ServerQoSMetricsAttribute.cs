using System;
using System.Text;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = false)]
    public class ServerQoSMetricsAttribute : WSQoSAttribute
    {

        private double _availability;
        private NamedItemCollection _CustomMetics;
        private double _processingTime;
        private double _reliability;
        private double _requestsPerSecond;

        public double Availability
        {
            get
            {
                return _availability;
            }
            set
            {
                _availability = value;
                BuildXml();
            }
        }

        public int CustomServerMetricsCount
        {
            get
            {
                return _CustomMetics.Count;
            }
        }

        public double ProcessingTime
        {
            get
            {
                return _processingTime;
            }
            set
            {
                _processingTime = value;
                BuildXml();
            }
        }

        public double Reliability
        {
            get
            {
                return _reliability;
            }
            set
            {
                _reliability = value;
                BuildXml();
            }
        }

        public double RequestsPerSecond
        {
            get
            {
                return _requestsPerSecond;
            }
            set
            {
                _requestsPerSecond = value;
                BuildXml();
            }
        }

        public ServerQoSMetricsAttribute()
        {
            _processingTime = -1.0;
            _requestsPerSecond = -1.0;
            _availability = -1.0;
            _reliability = -1.0;
            _CustomMetics = new NamedItemCollection();
        }

        public ServerQoSMetricsAttribute(XmlNode src)
        {
            _processingTime = -1.0;
            _requestsPerSecond = -1.0;
            _availability = -1.0;
            _reliability = -1.0;
            _CustomMetics = new NamedItemCollection();
            Update(src);
        }

        public void AddCustomServerMetric(CustomServerQoSMetricAttribute newMetric)
        {
            _CustomMetics.Add(newMetric);
        }

        public CustomServerQoSMetricAttribute GetCustomServerMetric(string Name, string Ontology)
        {
            // trial
            return null;
        }

        public CustomServerQoSMetricAttribute GetCustomServerMetric(int index)
        {
            return (CustomServerQoSMetricAttribute)_CustomMetics.ItemAt(index);
        }

        public void RemoveCustomServerMetric(CustomServerQoSMetricAttribute toBeRemoved)
        {
            // trial
        }

        protected override string GetInnerXml()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.ToString();
            if (_processingTime > 0.0)
                stringBuilder.AppendFormat("<processingTime>{0}</processingTime> \uFFFD", _processingTime);
            if (_requestsPerSecond > 0.0)
                stringBuilder.AppendFormat("<requestsPerSecond>{0}</requestsPerSecond> \uFFFD", _requestsPerSecond);
            if (_availability > 0.0)
                stringBuilder.AppendFormat("<availability>{0}</availability> \uFFFD", _availability);
            if (_reliability > 0.0)
                stringBuilder.AppendFormat("<reliability>{0}</reliability> \uFFFD", _reliability);
            for (int i = 0; i < _CustomMetics.Count; i++)
            {
                GetCustomServerMetric(i).BuildXml();
                if (GetCustomServerMetric(i).Value > 0.0)
                    stringBuilder.AppendFormat(GetCustomServerMetric(i).Xml.OuterXml, new object[0]);
            }
            return stringBuilder.ToString();
        }

        protected override string GetRootLabel()
        {
            // trial
            return null;
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class ServerQoSMetricsAttribute

}

