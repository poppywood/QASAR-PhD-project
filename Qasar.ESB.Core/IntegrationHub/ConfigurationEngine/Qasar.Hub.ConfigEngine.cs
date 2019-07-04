using System;
using System.Collections;
using System.Data;
using System.Data.Common;
using System.Text;

using Microsoft.Practices.EnterpriseLibrary.Data;
using Qasar.ESB.EndPoints;
using Qasar.ESB.Filter;

namespace Qasar.ESB.ConfigEngine
{

    /// <summary>
    /// A mini rule engine
    /// </summary>
    public class Rule
    {
        private Hashtable _hash;
        private Hashtable _vars;
        private Database _db;
        private int _pipe;
        /// <summary>
        /// Tracks the number of calls to a particular rule value
        /// This allows the same rule with the same parameters
        /// to be called mutliple times in a pipeline, returning
        /// different values each time.
        /// </summary>
        public Rule(int pipelineId)
        {
            _db = DatabaseFactory.CreateDatabase("Hub");
            _hash = new Hashtable();
            _pipe = pipelineId;
            _vars = new Hashtable();
            //cache the contents of the variables table to avoid repeated round-trips
            DbCommand dbCommand = _db.GetStoredProcCommand("uspGetVariables");
            DataSet ds = _db.ExecuteDataSet(dbCommand);
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                _vars[Convert.ToString(row["Variable"])] = Convert.ToString(row["Value"]); 
            }
            dbCommand.Dispose();
        }

        /// <summary>
        /// Rule LookUp
        /// </summary>
        /// <param name="FilterCode"></param>
        /// <param name="RuleCode"></param>
        /// <param name="ProductCode"></param>
        /// <param name="ActionCode"></param>
        /// <param name="SubActionCode"></param>
        /// <param name="SourceCode"></param>
        /// <param name="UserCode"></param>
        /// <returns>A string rule value</returns>
        public string Value(
            string FilterCode,
            string RuleCode,
            string ProductCode,
            string ActionCode,
            string SubActionCode,
            string SourceCode,
            string UserCode,
            PipeData pipe)
        {
            return value(FilterCode, RuleCode, ProductCode, ActionCode, SubActionCode, SourceCode, UserCode, pipe, "");
        }

        private string value(string FilterCode, string RuleCode, string ProductCode, string ActionCode, string SubActionCode, string SourceCode, string UserCode, PipeData pipe, string action)
        {
            //Possibly a rather abstruse way of building a unique key for the hash table
            StringBuilder sb = new StringBuilder();
            sb.Append(RuleCode);
            sb.Append(FilterCode);
            sb.Append(ProductCode);
            sb.Append(ActionCode);
            sb.Append(SubActionCode);
            sb.Append(SourceCode);
            sb.Append(UserCode);

            string val = string.Empty;
            IncCounter(sb.ToString());
            DbCommand dbCommand = _db.GetStoredProcCommand("uspGetRuleValue");
            _db.AddInParameter(dbCommand, "PipelineId", DbType.Int32, _pipe);
            _db.AddInParameter(dbCommand, "RulePrefix", DbType.String, FilterCode);
            _db.AddInParameter(dbCommand, "RuleSuffix", DbType.String, RuleCode);
            _db.AddInParameter(dbCommand, "ProductCode", DbType.String, ProductCode);
            _db.AddInParameter(dbCommand, "ActionCode", DbType.String, ActionCode);
            _db.AddInParameter(dbCommand, "SubActionCode", DbType.String, SubActionCode);
            _db.AddInParameter(dbCommand, "SourceCode", DbType.String, SourceCode);
            _db.AddInParameter(dbCommand, "UserCode", DbType.String, UserCode);
            _db.AddInParameter(dbCommand, "EffectiveDate", DbType.DateTime, DateTime.Now);
            DataSet ds = _db.ExecuteDataSet(dbCommand);
            dbCommand.Dispose();
            int id = Convert.ToInt32(ds.Tables[0].Rows[GetCounter(sb.ToString())]["ID"]);
            val = Convert.ToString(ds.Tables[0].Rows[GetCounter(sb.ToString())]["Value"]);
            if (val.StartsWith("{") && val.EndsWith("}")) //a variable - find real value
            {
                val = Convert.ToString(_vars[val]);
            }
            if (val == "[ENDPOINT]")
            {
                //Get the URL from the EndPoints table as directed by 
                //load-balancing algorithm
                Selector s = new Selector();
                val = s.BestEndPoint(id, pipe, action);
            }
            return val;
        }

        public string Value(
           string FilterCode,
           string RuleCode,
           string ProductCode,
           string ActionCode,
           string SubActionCode,
           string SourceCode,
           string UserCode,
           PipeData pipe,
           string action)
        {
            return value(FilterCode, RuleCode, ProductCode, ActionCode, SubActionCode, SourceCode, UserCode, pipe, action);
        }

        private void IncCounter(string code)
        {
            if (_hash[code] == null) _hash[code] = 0;
            else
                _hash[code] = Convert.ToString(Convert.ToInt32(_hash[code]) + 1);
        }

        private int GetCounter(string code)
        {
            return Convert.ToInt32(_hash[code]);
        }
    }
}
