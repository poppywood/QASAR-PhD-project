using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using Qasar.ObjectLayer;

namespace Qasar.DataLayer
{
    public class Metrics
    {
        static string connectionInfo = ConfigurationSettings.AppSettings["ConnectionInfo"];
        static SqlConnection objConnect = new SqlConnection(connectionInfo);

        public Metrics()
        {
            if (objConnect.State == ConnectionState.Closed) objConnect.Open();
        }

        public ResourceCollection GetResource(int ClassId)
        {
            ResourceCollection rc = new ResourceCollection();
            String sql = "SELECT Resource.ResourceID, Resource.Resource ";
                sql = sql + "FROM Class INNER JOIN ";
                sql = sql + "ClassResource ON Class.ClassId = ClassResource.ClassId INNER JOIN ";
                sql = sql + "Resource ON ClassResource.ResourceId = Resource.ResourceID ";
                sql = sql + "WHERE (Class.ClassId = " + ClassId + ") ORDER BY Resource.ResourceID"; 
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.Text, sql);
            while (reader.Read())
            {
                Resource r = new Resource(
                    reader.GetInt32(0),
                    reader.GetString(1));
                rc.Add(r);
            }
            reader.Close();
            return rc;
        }

        public TaskCollection GetTaskData(int interval, DateTime snap)
        {
            TaskCollection rc = new TaskCollection();
            SqlParameter[] parms = new SqlParameter[2];
            parms[0] = new SqlParameter();
            parms[0].ParameterName = "@interval";
            parms[0].SqlDbType = SqlDbType.Int;
            parms[0].Value = interval;
            parms[1] = new SqlParameter();
            parms[1].ParameterName = "@snap";
            parms[1].SqlDbType = SqlDbType.DateTime;
            parms[1].Value = snap;
            
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.StoredProcedure, "uspGetTasks", parms);
            while (reader.Read())
            {
                Task r = new Task(reader.GetInt32(0), reader.GetInt32(1), reader.GetDouble(2), reader.GetInt32(3));
                rc.Add(r);
            }
            reader.Close();
            return rc;
        }

        public int GetPipesforInterval(int interval, DateTime snap)
        {
            //TODO Get total pipes in interval. This will be NBar and we put it in
            //the spot load for the resource 0, class 1. 
            SqlParameter[] parms = new SqlParameter[2];
            parms[0] = new SqlParameter();
            parms[0].ParameterName="@interval";
            parms[0].SqlDbType=SqlDbType.Int;
            parms[0].Value=interval;
            parms[1] = new SqlParameter();
            parms[1].ParameterName="@snap";
            parms[1].SqlDbType=SqlDbType.DateTime;
            parms[1].Value=snap;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.StoredProcedure, "uspGetPipelines", parms));
        }

        public ResourceCollection GetResource()
        {
            ResourceCollection crc = new ResourceCollection();
            String sql = "SELECT DISTINCT Resource.ResourceID, Resource.Resource ";
            sql = sql + "FROM Class INNER JOIN ";
            sql = sql + "ClassResource ON Class.ClassId = ClassResource.ClassId INNER JOIN ";
            sql = sql + "Resource ON ClassResource.ResourceId = Resource.ResourceID ";
            sql = sql + "WHERE (Class.ClassId > 0) ORDER BY Resource.ResourceID"; 
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.Text, sql);
            while (reader.Read())
            {
                Resource r = new Resource(
                    reader.GetInt32(0),
                    reader.GetString(1));
                crc.Add(r);
            }
            reader.Close();
            return crc;
        }

        public WorkloadClassCollection GetWorkloadClass()
        {
            WorkloadClassCollection wc = new WorkloadClassCollection();
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.Text, "SELECT * FROM Class");
            while (reader.Read())
            {
                WorkloadClass c = new WorkloadClass(
                    reader.GetInt32(0),
                    reader.GetString(1),
                    reader.GetString(2));
                wc.Add(c);
            }
            reader.Close();
            return wc;
        }

        public ClassResourceCollection GetClassResource(int ClassId)
        {
            ClassResourceCollection crc = new ClassResourceCollection();
            String sql = "SELECT * FROM ClassResource WHERE ClassId = " + ClassId + "ORDER BY ResourceId";
            return PopulateClassResource(crc, sql);
        }

        private static ClassResourceCollection PopulateClassResource(ClassResourceCollection crc, String sql)
        {
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.Text, sql);
            while (reader.Read())
            {
                ClassResource cr = new ClassResource(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.GetDouble(3));
                crc.Add(cr);
            }
            reader.Close();
            return crc;
        }

        public ClassResourceCollection GetClassResource(string ClassSubActionCode)
        {
            ClassResourceCollection crc = new ClassResourceCollection(); 
            String sql = "SELECT * FROM ClassResource WHERE ";
            sql = sql + "(ClassId = (SELECT ClassId FROM Class ";
            sql = sql + "WHERE (SubActionCode = '" + ClassSubActionCode + "' ))) ORDER BY ResourceId";
            return PopulateClassResource(crc, sql);
        }

        public DateTime GetLastMetricsDate()
        {
            string sql = "SELECT LastMetrics FROM LastRuns";
            return (DateTime)SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql);

        }

        public DateTime GetLastOptoDate()
        {
            string sql = "SELECT LastOpto FROM LastRuns";
            return (DateTime)SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql);

        }

        public void SetLastOptoDate(DateTime snap)
        {
            string sql = "UPDATE LastRuns Set LastOpto = '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "'";
            SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
        }

        public void SetLastMetricsDate(DateTime snap)
        {
            string sql = "UPDATE LastRuns Set LastMetrics = '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "'";
            SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
        }

        public ResourceMeasurementCollection GetResourceMeasurement(int Crid)
        {
            ResourceMeasurementCollection rmc = new ResourceMeasurementCollection();
            //only get measurements within the last 10 mins
            DateTime snap = DateTime.Now.AddMinutes(-60);
            String sql = "SELECT * FROM ResourceMeasurement WHERE CRID = " + Crid + " AND Stamp > '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "' ORDER BY CRID DESC";
            SqlDataReader reader = SqlHelper.ExecuteReader(objConnect, CommandType.Text, sql);
            while (reader.Read())
            {
                ResourceMeasurement rm = new ResourceMeasurement(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetDouble(2),
                    reader.GetDouble(3));
                rmc.Add(rm);
            }
            reader.Close();
            return rmc;
        }


        public void SetSpotLoad(int res, int cls, double result, DateTime snap)
        {
            string sql ="(SELECT CRID FROM ClassResource WHERE ClassId = " + cls + " AND ResourceId = " + res + ")";
            int crid = Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql));
            sql = "UPDATE ClassResource SET SpotLoad=" + result + " WHERE ResourceID=" + res + " AND ClassID = " + cls;
            SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
            sql = "INSERT INTO HistoricSpotLoads (CRID, ResourceID, ClassID, SpotLoad, stamp ) VALUES (" + crid + ", " + res + ", " + cls + ", " + result + ", '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "')";
            SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
        }

        public void SetRM(int res, int cls, double av, double load, DateTime snap)
        {
            //first get crid for res/cl
            string sql ="(SELECT CRID FROM ClassResource WHERE ClassId = " + cls + " AND ResourceId = " + res + ")";
            int crid = Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql));
            
            //now insert new record in rm table
            if (crid != 0)
            {
                sql = "INSERT INTO ResourceMeasurement (CRID, [Load], Average, Stamp) VALUES (" + crid + ", " + load + ", " + av + ", '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "')";
                SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
            }
        }

        public void InsertWorkflowResult(int workflowid, double response)
        {
            DateTime snap = DateTime.Now;
            string sql = "INSERT INTO WorkflowResults (workflowid, response, stamp) VALUES (" + workflowid + ", " + response + ", '" + snap.Year + "-" + snap.Month + "-" + snap.Day + " " + snap.Hour + ":" + snap.Minute + ":" + snap.Second + "')";
            SqlHelper.ExecuteNonQuery(objConnect, CommandType.Text, sql);
        }

        public int GetWorkflowCount()
        {
            string sql = "select count(distinct workflowid) from workflow";
            return Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql));
        }

        public int GetClassCountforWorkflow(int workflow)
        {
            string sql = "select count(*) from workflow where workflowid = " + workflow;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql));
        }

        public int GetClassfromWorkflowSequence(int workflowid, int seqno)
        {
            string sql = "select classid from workflow where workflowid = " + workflowid + " and sequencenumber = " + seqno;
            return Convert.ToInt32(SqlHelper.ExecuteScalar(objConnect, CommandType.Text, sql));
        }
    }
}
