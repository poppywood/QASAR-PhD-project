#region WSCF
//------------------------------------------------------------------------------
// <autogenerated code>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if 
//     the code is regenerated.
// </autogenerated code>
//------------------------------------------------------------------------------
// File time 10-06-06 11:19 AM
//
// This source code was auto-generated by WSCF, Version=0.6.6034.1
#endregion


namespace ReinstateService
{
    using System.Diagnostics;
    using System.Web.Services;
    using System.ComponentModel;
    using System.Web.Services.Protocols;
    using System;
    using System.Xml.Serialization;
    using System.Web;

    using Qasar.DataAccessLayer.SqlClient;
    using Qasar.DataAccessLayer;
    using Qasar;
    
    
    /// <summary>
    /// 
    /// </summary>
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "2.0.50727.42")]
    [System.Web.Services.WebServiceAttribute(Namespace="http://qasar.shellysaunders.co.uk")]
    [System.Web.Services.WebServiceBindingAttribute(Name = "ReinstateService", Namespace = "http://qasar.shellysaunders.co.uk")]
    public partial class ReinstateService : System.Web.Services.WebService, IReinstateService
    {
        
        public ReinstateService()
        {
        }
        
        /// <summary>
        /// 
        /// </summary>
        [System.Web.Services.WebMethodAttribute()]
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("reinstateIn", Use = System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle = System.Web.Services.Protocols.SoapParameterStyle.Bare, Binding = "ReinstateService")]
        [return: System.Xml.Serialization.XmlElementAttribute("Response", Namespace="http://tempuri.org/Response.xsd")]
        public virtual string Reinstate([System.Xml.Serialization.XmlElementAttribute(Namespace="http://tempuri.org/Request.xsd", ElementName="Request")] string request)
        {
            //load up 20000 records from the database, loop through each one with a further db lookup
            TList<ProductModel> ind = DataRepository.ProductModelProvider.GetAll();
            string demo = string.Empty;
            foreach (ProductModel i in ind)
            {
                demo = i.CatalogDescription;
            }
            return demo;
        }
    }
}
