using System.Collections;
using System.Data;
using Microsoft.Practices.EnterpriseLibrary.Data;
using System.Data.Common;
using System.Data.SqlClient;

using Qasar.DataLayer;
using Qasar.ESB.Filter;
using Qasar.ObjectLayer;
using System;


namespace Qasar.ESB.EndPoints
{
    public class Selector
    {
        //get the endpoint address and resource id for the job
        public string BestEndPoint(int id, PipeData pipe, string action)
        {
            Database db = DatabaseFactory.CreateDatabase("Hub");
            DbCommand dbCommand =db.GetStoredProcCommand("uspGetEndpoints");
            db.AddInParameter(dbCommand, "ID", DbType.Int32, id);
            DataSet ds = db.ExecuteDataSet(dbCommand);
            
            string add = WeightedSelector(ds, pipe, action);
            dbCommand.Dispose();
            return add;
        }

        private string WeightedSelector(DataSet ds, PipeData pipe, string action)
        {
            //no slots = total number of jobs of this type
            //ie add up all equiv loads for all resources of this class
            //then pick a random number from 1 to no slots
            //and use the endpoint address for the matching resource
            Metrics metrics = new Metrics();
            ClassResourceCollection crc = metrics.GetClassResource(action);
            int totalJobs = 0;
            int cid = 0;
            foreach (ClassResource cr in crc)
            {
                cid = cr.getClassid();
                if (cr.getResourceid() != 0) totalJobs += Convert.ToInt32(cr.SpotLoad);
            }
            System.Random rd = new System.Random();
            int id = rd.Next(1, totalJobs + 1);
            int resid = 0;
            int runningTotal = 0;
            bool done = false;
            foreach (ClassResource cr in crc)
            {
                if (cr.getResourceid() != 0)
                {
                    runningTotal += Convert.ToInt32(cr.SpotLoad);
                    if (runningTotal >= id && !done)
                    {
                        resid = cr.getResourceid();
                        done = true;
                    }
                }
            }
            string address = string.Empty;
            int co = 0;
            foreach (DataRow row in ds.Tables[0].Rows)
            {
                if (resid == Convert.ToInt32(row["ResourceID"])) address = row["Address"].ToString();
            }

            pipe.ResourceID = resid;
            pipe.ClassID = cid;
            return address;
        }
    }
}
