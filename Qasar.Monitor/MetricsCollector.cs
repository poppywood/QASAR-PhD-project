using System;
using System.Collections.Generic;
using System.Text;
using Qasar.DataLayer;
using Qasar.ObjectLayer;
using log4net;
namespace Qasar.Monitor
{
    public class MetricsCollector
    {

        private static ILog GetLog()
        {
            //log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("Logger");
            return log;
        }
        //gets metrics from raw esb data
        //interval is the time period to sample in secs
        public void GatherMetrics(int interval, DateTime snap)
        {
            
            Metrics metrics = new Metrics();

            //we put nbar in resource0 class 1 and then zero in each other class 
            //for resource 0
            int result = metrics.GetPipesforInterval(interval, snap);
            metrics.SetSpotLoad(0, 1, Convert.ToDouble(result)/Convert.ToDouble(interval), snap);
            TaskCollection tasks = metrics.GetTaskData(interval, snap);
            //get indicative service time average
            ILog log = GetLog();
            log.Info("Starting metrics collation at: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());
            double ti = 0;
            if (tasks.Count != 0)
            {
                foreach (Task t in tasks)
                {
                    ti = ti + t.getExecutionTime();
                }
                //turn this into an average 

                ti = ti / tasks.Count;
                foreach (Task t in tasks)
                {

                    //only use if the load exceeds 1/service time 
                    //this is to remove spurious results caused by throughput plunging when load becomes to great.
                    //if (t.getResourceid() != 0 && t.getclassId() != 0 && ((t.getJobCount() / interval) > (1000 / ti)))
                    if (t.getResourceid() != 0 && t.getclassId() != 0 )
                    
                    {
                        log.Info("Resource: " + t.getResourceid() + ", Class: " + t.getclassId() + ", Execution Time: " + t.getExecutionTime() + ", Job rate: " + t.getJobCount() / interval); 
                        metrics.SetRM(t.getResourceid(), t.getclassId(), t.getExecutionTime(), t.getJobCount() / interval, snap);
                    }
                }
            }
        }

    }
}
