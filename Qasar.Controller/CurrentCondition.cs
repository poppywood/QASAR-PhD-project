using System;
using System.Collections.Generic;
using System.Text;
using Qasar.DataLayer;
using Qasar.ObjectLayer;
using Qasar.Mva;
using Qasar.GA;

namespace Qasar.Controller
{
    internal class MVABuilder
    {

        private Metrics metrics = new Metrics();
        private ApproxMva mva = new ApproxMva();
         

        public MVABuilder() 
        { 
        }

        public MVAResponse GetNewConfig(MVARequest request, double[] rtt, double[] tt, double wr, double wt)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm(request, rtt, tt, wr, wt);
            return ga.GetSolution();
        }

        public MVAResponse CheckWithModel(MVARequest request)
        {
            return mva.PerformCalc(request);
        }

        /// <summary>
        /// Get latest statistics and build a request
        /// </summary>
        public MVARequest GetCurrentCondition()
        {
            MVARequest request = new MVARequest();
            WorkloadClassCollection wcc = metrics.GetWorkloadClass();
            MVARequestClass[] mrc = new MVARequestClass[wcc.Count];
            int classes = wcc.Count;
            ResourceCollection res = metrics.GetResource();
            int resources = res.Count;
            request.NoResources = resources - 1;
            request.NoClasses = classes;
            request.NoClients = 1;
            double[,] equiv = new double[resources, classes];
            double[,] D = new double[resources, classes];
            double[,] a = new double[resources, classes];
            
            for (int i = 0; i <= classes - 1; i += 1)
            {
                mrc[i] = new MVARequestClass();
                mrc[i].ClassID = wcc[i].getClassid();
                ClassResourceCollection rc = metrics.GetClassResource(mrc[i].ClassID);
                MVARequestClassResource[] cr = new MVARequestClassResource[rc.Count];
                mrc[i].Resources = new MVARequestClassResource[rc.Count];
                double run = 0;
                for (int j = 0; j <= rc.Count - 1; j += 1)
                {
                    cr[j] = new MVARequestClassResource();
                    cr[j].ResourceID = rc[j].getResourceid();

                    if (cr[j].ResourceID != 0)
                    { 
                        ResourceMeasurementCollection rmc = metrics.GetResourceMeasurement(rc[j].getCrid());
                        int l = rmc.Count;

                        //first get the current jobs/sec for each class/res and 
                        //use these to set the initial spot loads in the cr table
                        //the most recent rm is the current condition
                        if (l > 0)
                        {
                            equiv[j, i] = rmc[l - 1].getLoad();
                            cr[j].EquivLoad = equiv[j, i];
                            run = run + equiv[j, i];

                            //restrict data to the last 100 points
                            int start = Math.Max(0, l - 100);
                            double[] x = new double[l];
                            double[] y = new double[l];
                            for (int k = start; k <= l - 1; k += 1)
                            {
                                ResourceMeasurement rm = rmc[k];
                                x[k] = Convert.ToDouble(rm.getLoad());
                                y[k] = Convert.ToDouble(rm.getAverage());

                            }
                            double currentAv = y[l - 1];
                            if (l == 1)
                            {
                                //its a single point so assume non load dependent
                                D[j, i] = y[0];
                                a[j, i] = 1;
                            }
                            else if (l == 0)
                            {
                                D[j, i] = 0;
                                a[j, i] = 1;
                            }
                            else
                            {
                                SlopeInterceptPairD s = new SlopeInterceptPairD();
                                s = Statistics.LinearFit(y, x);
                                D[j, i] = s.Slope * equiv[j, i] + s.Intercept;
                                a[j, i] = (s.Slope * equiv[j, i]) / currentAv; //might be inverse of this!
                            }
                            cr[j].ServiceTime = D[j, i];
                            cr[j].LoadRatio = a[j, i];
                            if (cr[j].LoadRatio < 0) cr[j].LoadRatio = 1;
                            mrc[i].Resources[j] = cr[j];
                        }
                        else
                        {
                            cr[j].EquivLoad = 0;
                            cr[j].ServiceTime = 0;
                            cr[j].LoadRatio = 1;
                            mrc[i].Resources[j] = cr[j];
                        }
                    }
                    else
                    {
                        if (j == 0 && i == 0) cr[j].EquivLoad = rc[j].SpotLoad; else cr[j].EquivLoad = 0;
                        cr[j].ServiceTime = 1; //av think time is the av of the interarrival times
                        cr[j].LoadRatio = 1;
                        mrc[i].Resources[j] = cr[j];
                    }
                }
                mrc[i].TotalPopulation = Convert.ToInt32(Math.Ceiling(run));//sum of equiv loads on each res for this class, not counting terminal                
            }
            request.Classes = mrc;
            return request;
        }

        public void SendLoadBalancedUpdatetoBus(double[,] jobs, int res, int cls)
        {
            DateTime snap = DateTime.Now;
            //parse the inbound jobs array and update the table in the db
            for (int j = 1; j <= cls; j += 1)
            {
                for (int k = 1; k <= res; k += 1)
                {
                    //TODO change this to to get the right resourceid
                    int resd = k; if (k == 2) resd = 3;
                    metrics.SetSpotLoad(resd, j, jobs[k, j], snap);
                }
            }
        }
    }
}
