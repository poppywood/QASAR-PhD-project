using System;
using System.Collections.Generic;
using System.Text;
using Qasar.Mva;
using org.jgap;


namespace Qasar.GA
{
    class Decoder
    {
        public double[,] DecodeChromosome(Chromosome subject, Configuration config, int resources, int classes)
        {
            ChromosomeIndexCollection cic = config.ChromosomeIndexes;
            double[,] jobs = new double[resources + 1, classes + 1];
            int rescounter = 1;
            //each gene value is looked up in the cic to establish which job it is.
            for (int i = 0; i < subject.size(); i += 1)
            {
                int allele = (int)subject.Genes[i].getAllele();
                int classType = cic[allele].getClassType();
                //if classType = 0 then this is the resource divider, otherwise increment job count for this
                //classType on this resource by 1
                if (classType == 0)
                {
                    rescounter += 1;
                }
                else
                {
                    jobs[rescounter, classType] += 1;
                }
            }
            return jobs;
        }
    }
}
