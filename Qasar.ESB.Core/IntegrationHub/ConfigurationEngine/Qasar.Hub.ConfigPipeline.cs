using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;

namespace Qasar.ESB.ConfigEngine
{

    /// <summary>
    /// Provides Pipeline Rules
    /// </summary>
    public class PipelineRule
    {
        public PipelineRule()
        {
            //
            // TODO: Add constructor logic here
            //
        }
        /// <summary>
        /// Pipeline sequence LookUp
        /// </summary>
        /// <param name="FilterCode"></param>
        /// <param name="RuleCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="ActionCode"></param>
        /// <param name="SubActionCode"></param>
        /// <param name="SourceCode"></param>
        /// <param name="UserCode"></param>
        /// <returns>A string rule value</returns>
        public string PipelineValue(
            ref int Id,
            string ProductCode,
            string ActionCode,
            string SubActionCode,
            string SourceCode,
            string UserCode)
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspGetPipelineValue");
            db.AddInParameter(dbCommand, "ProductCode", DbType.String, ProductCode);
            db.AddInParameter(dbCommand, "ActionCode", DbType.String, ActionCode);
            db.AddInParameter(dbCommand, "SubActionCode", DbType.String, SubActionCode);
            db.AddInParameter(dbCommand, "SourceCode", DbType.String, SourceCode);
            db.AddInParameter(dbCommand, "UserCode", DbType.String, UserCode);
            db.AddInParameter(dbCommand, "EffectiveDate", DbType.DateTime, DateTime.Now);
            db.AddOutParameter(dbCommand, "Value", DbType.String, 255);
            db.AddOutParameter(dbCommand, "ID", DbType.Int32, 4);
            db.ExecuteNonQuery(dbCommand);
            Id = Convert.ToInt32(db.GetParameterValue(dbCommand, "@ID"));
            return Convert.ToString(db.GetParameterValue(dbCommand, "@Value"));
        }
    }
}
