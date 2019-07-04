using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSMetricDef : WSQoSAttribute
    {

        private string _dataOwner;
        private string _dataScope;
        private string _desc;
        private bool _inc;
        private string _measurementInterval;
        private double _percentile;
        private string _unit;

        public string DataOwner
        {
            get
            {
                return _dataOwner;
            }
            set
            {
                _dataOwner = value;
            }
        }

        public string DataScope
        {
            get
            {
                return _dataScope;
            }
            set
            {
                _dataScope = value;
            }
        }

        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                _desc = value;
            }
        }

        public bool Increasing
        {
            get
            {
                return _inc;
            }
            set
            {
                _inc = value;
            }
        }

        public string MeasurementInterval
        {
            get
            {
                return _measurementInterval;
            }
            set
            {
                _measurementInterval = value;
            }
        }

        public double Percentile
        {
            get
            {
                return _percentile;
            }
            set
            {
                _percentile = value;
            }
        }

        public string Unit
        {
            get
            {
                return _unit;
            }
            set
            {
                _unit = value;
            }
        }

        public WSQoSMetricDef()
        {
            _inc = true;
            _desc = "\uFFFD";
            _unit = "\uFFFD";
            _dataOwner = "\uFFFD";
            _dataScope = "\uFFFD";
            _measurementInterval = "\uFFFD";
            _percentile = 1.0;
        }

        public WSQoSMetricDef(XmlNode src)
        {
            _inc = true;
            _desc = "\uFFFD";
            _unit = "\uFFFD";
            _dataOwner = "\uFFFD";
            _dataScope = "\uFFFD";
            _measurementInterval = "\uFFFD";
            _percentile = 1.0;
            Update(src);
        }

        protected override string GetRootLabel()
        {
            return "metricDefinition\uFFFD";
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class WSQoSMetricDef

}

