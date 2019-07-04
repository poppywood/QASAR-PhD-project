using System.Xml;

namespace WSQoSAPI
{

    public class WSQoSPriorityDef : WSQoSAPI.WSQoSAttribute
    {

        private string _desc;

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

        public WSQoSPriorityDef()
        {
            _desc = "\uFFFD";
        }

        public WSQoSPriorityDef(System.Xml.XmlNode src)
        {
            _desc = "\uFFFD";
            Update(src);
        }

        protected override string GetRootLabel()
        {
            return "priorityDefinition\uFFFD";
        }

        protected override void UpdateProperties()
        {
            // trial
        }

    } // class WSQoSPriorityDef

}

