using System;
using System.IO;
using System.Xml;

namespace WSQoSAPI
{

    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Module | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Event | AttributeTargets.Interface | AttributeTargets.Parameter | AttributeTargets.Delegate | AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class ImportQoSDeclarationAttribute : Attribute
    {

        private DateTime _lastCheck;
        private string _loc;
        private XmlElement _xml;

        public string Location
        {
            get
            {
                return _loc;
            }
            set
            {
                _loc = value;
            }
        }

        public ImportQoSDeclarationAttribute()
        {
            _loc = "\uFFFD";
            _lastCheck = DateTime.Now;
        }

        public XmlElement GetXml()
        {
            XmlElement xmlElement;

            DateTime dateTime = new DateTime(1, 1, 1);
            try
            {
                dateTime = File.GetLastWriteTime(_loc);
            }
            catch (SystemException e)
            {
                e.Message;
                return null;
            }
            if (dateTime > _lastCheck)
                LoadFile();
            _lastCheck = DateTime.Now;
            return _xml;
        }

        public void LoadFile()
        {
            // trial
        }

    } // class ImportQoSDeclarationAttribute

}

