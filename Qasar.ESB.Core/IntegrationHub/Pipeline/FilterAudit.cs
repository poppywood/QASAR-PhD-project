using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using Qasar.ESB.Filter;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// FilterAudit encapsulates auditing for a filter.
    /// </summary>
    public class FilterAudit
    {
        int _id;
        PipeData _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public FilterAudit(PipeData data, int pipelineAuditId, string filterName)
        {
            _data = data;
            _id = FilterCreate(pipelineAuditId, filterName);
        }

        /// <summary>
        /// This method creates the initial audit entry for each filter audit
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        private int FilterCreate(int pipelineAuditId, string filterName)
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspCreateFilterAudit");
            db.AddInParameter(dbCommand, "PipelineAuditID", DbType.Int32, pipelineAuditId);
            db.AddInParameter(dbCommand, "Request", DbType.String, _data.Request);
            db.AddInParameter(dbCommand, "FilterName", DbType.String, filterName);
            db.AddInParameter(dbCommand, "StartDateTime", DbType.DateTime, DateTime.Now);
            db.AddOutParameter(dbCommand, "ID", DbType.Int32, 32);
            db.ExecuteNonQuery(dbCommand);
            return Convert.ToInt32(db.GetParameterValue(dbCommand, "@ID"));
        }

        /// <summary>
        /// This methods Logs the result for the filter
        /// </summary>
        /// <param name="r">pipeResult</param>
        /// <param name="data"></param>
        /// <param name="id"></param>
        public void Log()
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspUpdateFilterAudit");
            db.AddInParameter(dbCommand, "ID", DbType.Int32, _id);
            db.AddInParameter(dbCommand, "Response", DbType.String, _data.Request);
            db.AddInParameter(dbCommand, "EndDateTime", DbType.DateTime, DateTime.Now);
            db.AddInParameter(dbCommand, "Result", DbType.Boolean, _data.Success);
            db.AddInParameter(dbCommand, "Notification", DbType.Boolean, _data.Notification);
            db.AddInParameter(dbCommand, "ResourceID", DbType.Int32, _data.ResourceID);
            db.AddInParameter(dbCommand, "ClassID", DbType.Int32, _data.ClassID);
            db.ExecuteNonQuery(dbCommand);
        }
    }
}
