using System;
using System.IO;
using System.Xml;

namespace WSQoSAPI
{

    [System.AttributeUsage(System.AttributeTargets.Assembly | System.AttributeTargets.Module | System.AttributeTargets.Class | System.AttributeTargets.Struct | System.AttributeTargets.Enum | System.AttributeTargets.Constructor | System.AttributeTargets.Method | System.AttributeTargets.Property | System.AttributeTargets.Field | System.AttributeTargets.Event | System.AttributeTargets.Interface | System.AttributeTargets.Parameter | System.AttributeTargets.Delegate | System.AttributeTargets.ReturnValue, Inherited = true, AllowMultiple = true)]
    public class ImportQoSDeclarationAttribute : System.Attribute
    {

        private System.DateTime _lastCheck;
        private string _loc;
        private System.Xml.XmlElement _xml;

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
            _lastCheck = System.DateTime.Now;
        }

        public System.Xml.XmlElement GetXml()
        {
            System.Xml.XmlElement xmlElement;

            System.DateTime dateTime = new System.DateTime(1, 1, 1);
            try
            {
                dateTime = System.IO.File.GetLastWriteTime(_loc);
            }
            catch (System.SystemException e)
            {
                //e.Message;
                return null;
            }
            if (dateTime > _lastCheck)
                LoadFile();
            _lastCheck = System.DateTime.Now;
            return _xml;
        }

        public void LoadFile()
        {
            // trial
        }

    } // class ImportQoSDeclarationAttribute

}

