using System;
using System.Collections.Generic;
using System.Text;
using org.jgap;
using Qasar.DataLayer;
using Qasar.Mva;

namespace Qasar.GA
{
    class MvaFitnessFunction : FitnessFunction
    {
        
        public MvaFitnessFunction()
        {
        }

        protected internal override int evaluate(Chromosome subject, Configuration config)
        {
            ChromosomeIndexCollection cic = config.ChromosomeIndexes;
            MVARequest request = config.Request;
            MVAResponse response = new MVAResponse();

            Decoder dec = new Decoder();
            double[,] jobs = dec.DecodeChromosome(subject, config, request.NoResources, request.NoClasses);
            
            //update the MVARequest with new Equiv Loads
            for (int r = 1; r <= request.NoClasses; r += 1)
            {
                for (int k = 1; k <= request.NoResources; k += 1)
                {
                    request.Classes[r - 1].Resources[k].EquivLoad = jobs[k, r];
                }
            }
            ApproxMva mva = new ApproxMva();
            response = mva.PerformCalc(request);

            Metrics metrics = new Metrics();
            int workflowNo = metrics.GetWorkflowCount();
            double sum = 0;
            for (int i = 0; i < workflowNo; i += 1)
            {
                int noclasses = metrics.GetClassCountforWorkflow(i + 1);
                double total = 0;
                for (int cl = 0; cl < noclasses; cl += 1)
                {
                    int clid = metrics.GetClassfromWorkflowSequence(i + 1, cl + 1);
                    total = total + response.Classes[clid - 1].Metrics.ServiceDemand;
                    //heavily penalise the result if the average utilisation exceeds 1
                    if (response.Classes[clid - 1].Metrics.Utilization > (decimal)0.99) total = total + 1000000;
                }
                double sLA = config.getresponseTimeTargets()[i];
                sum = sum + 100 * (config.getweightResponse() * (sLA - total) / Math.Max(sLA, total)
                    + config.getweightResponse() * (sLA - total) / Math.Max(sLA, total));
            }
            if (sum < 1) sum = 1; else sum = sum * 100;
            return Convert.ToInt32(sum) ;
        }

    }
}
