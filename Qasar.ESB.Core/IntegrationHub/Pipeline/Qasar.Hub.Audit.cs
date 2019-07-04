using System;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using Qasar.ESB.Filter;

namespace Qasar.ESB.Pipeline
{
    /// <summary>
    /// Provides an audit of each pipeline transaction
    /// </summary>
    public class Audit
    {
        int _id;
        PipeData _data;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data"></param>
        public Audit(PipeData data)
        {
            _id = PipelineCreate(data);
            _data = data;
            
        }

        /// <summary>
        /// This method creates the initial audit entry for the pipeline audit
        /// </summary>
        /// <param name="data"></param>
        private int PipelineCreate(PipeData data)
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspCreatePipelineAudit");
            db.AddInParameter(dbCommand, "Request", DbType.String, data.Request);
            db.AddInParameter(dbCommand, "Action", DbType.String, Convert.ToString(data.RequestType));
            db.AddInParameter(dbCommand, "SubAction", DbType.String, Convert.ToString(data.SubAction));
            db.AddInParameter(dbCommand, "ProductCode", DbType.String, data.ProductCode);
            db.AddInParameter(dbCommand, "Source", DbType.String, Convert.ToString(data.Source));
            db.AddInParameter(dbCommand, "UserID", DbType.String, data.UserId);
            db.AddInParameter(dbCommand, "StartDateTime", DbType.DateTime, DateTime.Now);
            db.AddOutParameter(dbCommand, "ID", DbType.Int32, 32);
            db.ExecuteNonQuery(dbCommand);
            return Convert.ToInt32(db.GetParameterValue(dbCommand, "@ID"));
        }

        /// <summary>
        /// This methods Logs the result for the pipeline
        /// </summary>
        /// <param name="pipelineId">id of the pipeline</param>
        public void PipelineLog(int pipelineId)
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand = db.GetStoredProcCommand("uspUpdatePipelineAudit");
            db.AddInParameter(dbCommand, "ID", DbType.Int32, _id);
            db.AddInParameter(dbCommand, "PipelineId", DbType.Int32, pipelineId);
            db.AddInParameter(dbCommand, "Response", DbType.String, _data.Request);
            db.AddInParameter(dbCommand, "Result", DbType.Boolean, _data.Success);
            db.AddInParameter(dbCommand, "Notification", DbType.Boolean, _data.Notification);
            db.AddInParameter(dbCommand, "EndDateTime", DbType.DateTime, DateTime.Now);
            db.ExecuteNonQuery(dbCommand);
        }

        /// <summary>
        /// FilterCreate
        /// </summary>
        /// <param name="data"></param>
        /// <param name="filterName"></param>
        /// <returns></returns>
        public FilterAudit FilterCreate(string filterName)
        {
            return new FilterAudit(_data, _id, filterName);
        }

        /// <summary>
        /// The audit Id
        /// </summary>
        public int Id
        {
            get { return _id; }
        }
    }
}
