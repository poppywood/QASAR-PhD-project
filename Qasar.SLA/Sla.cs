using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Web;

namespace Qasar.Sla
{
    public class Sla : IQasarSla
    {

        #region IQasarSla Members

        public void SetWsqos(Wsqos wsqos1, string slaFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Wsqos));
            TextWriter tw = new StreamWriter(HttpContext.Current.Server.MapPath(slaFile + ".wsqos"));
            serializer.Serialize(tw, wsqos1);
            tw.Close(); 
        }

        public Wsqos GetWsqos(string slaFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Wsqos));
            TextReader tr = null;

                slaFile = System.Configuration.ConfigurationManager.AppSettings["qos"];
                tr = new StreamReader(slaFile + ".wsqos");

            Wsqos wsqos1 = (Wsqos)serializer.Deserialize(tr);
            tr.Close();
            return wsqos1;
        }

        #endregion
    }
}
