using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

using Qasar.Controller;
using Qasar.Monitor;

using log4net;

namespace Qasar.Console
{
    class Program
    {
        private static System.Timers.Timer metricsTimer;
        private static System.Timers.Timer optoTimer;
        private static ILog GetLog()
        {
            ILog log = LogManager.GetLogger("Logger");
            return log;
        }

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = GetLog();
            try
            {
                //rm metrics will be updated every 1 mins
                metricsTimer = new System.Timers.Timer(1000 * 60);
                metricsTimer.Elapsed += new ElapsedEventHandler(OnTimedEvent);

                //the controller will run ten minute
                optoTimer = new System.Timers.Timer(1000 * 600);
                optoTimer.Elapsed += new ElapsedEventHandler(OnTimedOptoEvent);

                System.Console.WriteLine("Starting up metrics collation.");
                GetMetrics(DateTime.Now);
                
                metricsTimer.Enabled = true;
                optoTimer.Enabled = true;
            }
            catch(Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                log.Info(ex.Message);
            }
            
            System.Console.WriteLine("Press the Enter key to exit the program.");
            System.Console.ReadLine();
        }

        private static void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            GetMetrics(e.SignalTime);
        }

        private static void GetMetrics(DateTime dt)
        {
            MetricsCollector c = new MetricsCollector();
            c.GatherMetrics(60, dt.AddSeconds(-60));
        }

        private static void OnTimedOptoEvent(object source, ElapsedEventArgs e)
        {
            Opto();
        }

        private static void Opto()
        {
            Controller.Controller c = new Controller.Controller();
            c.CheckCurrentCondition();
        }
    }
}
