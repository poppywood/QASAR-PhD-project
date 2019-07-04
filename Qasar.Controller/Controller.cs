using System;
using System.Collections.Generic;
using System.Text;

using Qasar.Mva;
using Qasar.Sla;
using Qasar.DataLayer;

using log4net;


namespace Qasar.Controller
{
    public class Controller
    {
        private static ILog GetLog()
        {
            ILog log = LogManager.GetLogger("Logger");
            return log;
        }
        public Controller()
        {
            //log4net.Config.XmlConfigurator.Configure();
        }

        public void CheckCurrentCondition()
        {
            MVABuilder mvab = new MVABuilder();
            
            ILog log = GetLog();
            //do mva on current status
            MVARequest request = mvab.GetCurrentCondition();
            MVAResponse response = mvab.CheckWithModel(request);

            // next get the actual SLA targets
            Sla.Sla sla1 = new Sla.Sla();
            Wsqos wsqos1 = sla1.GetWsqos(@"..\\..\\..\\Qasar.Web\\SampleQoSRequirements");
            WsqosDefinitionOffers offers = (WsqosDefinitionOffers)wsqos1.Definition.Item;
            WsqosDefinitionOffersQosOffer[] offer = offers.QosOffer;


 

            Metrics metrics = new Metrics();
            int workflowNo = metrics.GetWorkflowCount();
            double[] targetResponse = new double[workflowNo];
            double[] targetX = new double[workflowNo];

            log.Info("Starting Optimisation Check at: " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString());

            foreach (WsqosDefinitionOffersQosOffer off in offer)
            {
                //each offer is a different class of service, e.g. Gold, 
                //Silver and Bronzer with multiple operations (job class)
                //there is also a node for the overall response 
                TQoSInfo overall = off.DefaultQoSInfo;
                double r = Convert.ToDouble(overall.ServerQoSMetrics.ProcessingTime);
                TQoSDefinitionOperationQoSInfo[] ops = off.OperationQoSInfo;

                //check the target responses against the actual responses
                bool onTarget = true;
                for (int i = 0; i < workflowNo; i += 1)
                {
                    targetResponse[i] = Convert.ToDouble(ops[i].ServerQoSMetrics.ProcessingTime);
                    int noclasses = metrics.GetClassCountforWorkflow(i + 1);
                    double total = 0;
                    for (int cl = 0; cl < noclasses; cl += 1)
                    {
                        int clid = metrics.GetClassfromWorkflowSequence(i + 1, cl + 1);
                        total = total + response.Classes[clid - 1].Metrics.ServiceDemand;
                    }
                    metrics.InsertWorkflowResult(i + 1, total);
                    if (targetResponse[i] < total) onTarget = false;
                }
                if (!onTarget)
                {
                    //call GA for optimised response
                    log.Info("Executing GA");
                    double[,] jobs = new double[request.NoResources + 1, request.NoClasses + 1];
                    MVAResponse betterResponse = mvab.GetNewConfig(request, targetResponse, targetX, 1, 0);
                    for (int br = 1; br <= request.NoClasses; br += 1)
                    {
                        for (int k = 0; k <= request.NoResources; k += 1)
                        {
                            if (k == 0)
                                jobs[k, br] = request.Classes[br - 1].Resources[k].EquivLoad;
                            else
                                jobs[k, br] = betterResponse.Classes[br - 1].Resources[k].EquivLoad;
                        }
                    }
                    //send new LB update
                    mvab.SendLoadBalancedUpdatetoBus(jobs, request.NoResources, request.NoClasses);

                    //dump results
                    DumpResults("After", jobs, request.NoClasses, request.NoResources);
                }
            }
        }

        private static void DumpResults(string when, double[,] jobs, int cls, int res)
        {
            ILog log = GetLog();
            for (int j = 1; j <= cls; j += 1)
            {
                for (int k = 1; k <= res; k += 1)
                {
                    //TODO change this to to get the right resourceid
                    int resd = k; if (k == 2) resd = 3;
                    log.Info(when + ": class " + j + " on resource " + resd + " has load = " + jobs[k, j]);
                }
            }
        }
    }
}
