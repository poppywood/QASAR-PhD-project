using System;
using System.Collections.Generic;
using System.Text;

using org.jgap;
using org.jgap.impl;

using Qasar.Mva;

namespace Qasar.GA
{
    public class GeneticAlgorithm
    {
        private const int NO_CHROMOSOMES = 50;
        private const int MAX_ALLOWED_EVOLUTIONS = 50;
        private const int MUTATION_RATE = 10;  // => 1/MUTATION_RATE %

        private Configuration m_config;

        private static ApproxMva mva = new ApproxMva();
        private MVARequest request = new MVARequest();
        private MVAResponse response = new MVAResponse();
        private double[] responseTimeTargets = new double[1];
        private double[] throughputTargets = new double[1];
        private double weightResponse = 0;
        private double weightThroughput = 0;
        private ChromosomeIndexCollection cic = new ChromosomeIndexCollection();

        public MVAResponse getResponse()
        {
            return this.response;
        }

        public GeneticAlgorithm(MVARequest Request, 
            double[] ResponseTimeTargets, 
            double[] ThroughputTargets,
            double WeightResponse,
            double WeightThroughput)
        {
            this.responseTimeTargets = (double[])ResizeArray(this.responseTimeTargets, Request.NoClasses);
            this.throughputTargets = (double[])ResizeArray(this.throughputTargets, Request.NoClasses);
            this.request = Request;
            this.responseTimeTargets = ResponseTimeTargets;
            this.throughputTargets = ThroughputTargets;
            this.weightResponse = WeightResponse;
            this.weightThroughput = WeightThroughput;
        }

        public MVAResponse GetSolution()
        {
            int NoResources = request.NoResources;
            int NoClasses = request.NoClasses;
            int[] N = new int[NoClasses];
            int JobTotal = 0;
            int runningCount = 0;
            for (int r = 0; r < NoClasses; r += 1)
            {
                //number of jobs of each class is the sum of the equiv loads on each resource
                //(nit counting the terminal resource) which is given by the TotalPopulation
                N[r] = request.Classes[r].TotalPopulation;
                JobTotal += N[r];
                for (int j = 0; j < N[r]; j += 1)
                {
                    ChromosomeIndex ci = new ChromosomeIndex(runningCount, r + 1);
                    runningCount += 1;
                    cic.Add(ci);
                }
            }
            //set up Chromosome Indexes  by adding each job to a class and resource
            //note that this simply maps all jobs by class type onto an array

            for (int i = 0; i < NoResources - 1; i++)
            {
                ChromosomeIndex ci = new ChromosomeIndex(runningCount, 0);
                runningCount += 1;
                cic.Add(ci);
            }
            Chromosome chromo = FindOptimalChromosome(NoResources, NoClasses, JobTotal);
            Decoder dec = new Decoder();
            double[,] jobs = dec.DecodeChromosome(chromo, m_config, NoResources, NoClasses);
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
            for (int r = 1; r <= request.NoClasses; r += 1)
            {
                for (int k = 0; k <= request.NoResources; k += 1)
                {
                    if (k == 0) 
                        response.Classes[r - 1].Resources[k].EquivLoad = request.Classes[r - 1].Resources[k].EquivLoad;
                    else
                        response.Classes[r - 1].Resources[k].EquivLoad = jobs[k, r];
                }
            }
            return response;
        }

        protected Chromosome FindOptimalChromosome(int NoResources, int NoClasses, int JobTotal)
        {
            m_config = createConfiguration();
            m_config.SampleChromosome = createSampleChromosome(NoResources + JobTotal - 1);
            m_config.setPopulationSize(NO_CHROMOSOMES);
            m_config.ChromosomeIndexes = cic;
            m_config.Request = this.request;
            m_config.setweightResponse(this.weightResponse);
            m_config.setweightThroughput(this.weightThroughput);
            m_config.setresponseTimeTargets(this.responseTimeTargets);
            m_config.setthroughputTargets(this.throughputTargets);

            Chromosome[] chromosomes = new Chromosome[m_config.getPopulationSize()];
            Gene[] samplegenes = m_config.SampleChromosome.Genes;
            for (int i = 0; i < chromosomes.Length; i++)
            {
                Gene[] genes = new Gene[samplegenes.Length];
                for (int k = 0; k < genes.Length; k++)
                {
                    genes[k] = samplegenes[k].newGene(m_config);
                    genes[k].setAllele(samplegenes[k].getAllele());
                }
                shuffle(genes);
                chromosomes[i] = new Chromosome(m_config, genes);
            }
            Genotype population = new Genotype(m_config, chromosomes);
            Chromosome best = null;

            int j = 0;
            do //loop for the max number of iterations
            {
                population.evolve();
                best = (Chromosome)population.FittestChromosome;
                j++;
            }
            while (j < MAX_ALLOWED_EVOLUTIONS );
            return best;
        }

        public Configuration getConfiguration()
        {
            return m_config;
        }

        private Chromosome createSampleChromosome(int chromosomeLength)
        {
            Gene[] mygenes = new Gene[chromosomeLength];
            for (int i = 0; i < chromosomeLength; i++)
            {
                mygenes[i] = new IntegerGene(0, chromosomeLength - 1);
                mygenes[i].setAllele(i);
            }
            Chromosome sample = new Chromosome(getConfiguration(), mygenes);
            shuffle(mygenes);
            return sample;
        }

        protected void shuffle(Gene[] genes) 
        {
            Gene t;
            for (int r = 0; r < 10 * genes.Length; r++) 
            {
                for (int i = 0; i < genes.Length; i++) 
                {
                    int p = m_config.getRandomGenerator().nextInt(genes.Length);
                    t = genes[i];
                    genes[i] = genes[p];
                    genes[p] = t;
                }
            }
        }

        private Configuration createConfiguration()
        {
            //Amend default configuration to use Greedy Crossover, Swapping Mutation and custom Mva fitness function
            Configuration conf = new Configuration();
            conf.addNaturalSelector(new WeightedRouletteSelector(), false);
            conf.setRandomGenerator(new StockRandomGenerator());
            conf.addGeneticOperator(new GreedyCrossover(conf));
            conf.setPopulationSize(NO_CHROMOSOMES);
            conf.addGeneticOperator(new SwappingMutationOperator(conf, MUTATION_RATE));
            conf.setFitnessFunction(new MvaFitnessFunction());
            return conf;
        }

        private System.Array ResizeArray(System.Array oldArray, int newSize)
        {
            System.Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);
            return newArray;
        }
    }
}
