using System;

using ChainOfSelectors = org.jgap.impl.ChainOfSelectors;
using ChromosomePool = org.jgap.impl.ChromosomePool;
using Qasar.GA;
using Qasar.Mva;

namespace org.jgap
{
	public class Configuration
	{
		private void  InitBlock()
		{
			m_geneticOperators = new SupportClass.ListCollectionSupport(new System.Collections.ArrayList());
		}

        private ChromosomeIndexCollection m_chromosomeIndexes;
        private MVARequest m_request;
        private double[] responseTimeTargets = new double[1];
        private double[] throughputTargets = new double[1];
        private double weightResponse = 0;
        private double weightThroughput = 0;
        private FitnessFunction m_objectiveFunction;
        private Chromosome m_sampleChromosome;
        private RandomGenerator m_randomGenerator;
        private ChromosomePool m_chromosomePool;
        private SupportClass.ListCollectionSupport m_geneticOperators;
        private int m_chromosomeSize;
        private int m_populationSize;
        private bool m_settingsLocked;
        private ChainOfSelectors m_preSelectors;
        private ChainOfSelectors m_postSelectors;

        public virtual void setresponseTimeTargets(double[] responseTimeTargets)
        {
            this.responseTimeTargets = (double[])ResizeArray(this.responseTimeTargets, responseTimeTargets.Length);
            this.responseTimeTargets = responseTimeTargets;
        }

        public virtual double[] getthroughputTargets()
        {
            return this.throughputTargets;
        }

        public virtual void setthroughputTargets(double[] throughputTargets)
        {
            this.throughputTargets = (double[])ResizeArray(this.throughputTargets, throughputTargets.Length);
            this.throughputTargets = throughputTargets;
        }

        public virtual double[] getresponseTimeTargets()
        {
            return this.responseTimeTargets;
        }

        public virtual void setweightResponse(double weightResponse)
        {
            this.weightResponse = weightResponse;
        }

        public virtual double getweightResponse()
        {
            return this.weightResponse;
        }

        public virtual void setweightThroughput(double weightThroughput)
        {
            this.weightThroughput = weightThroughput;
        }

        public virtual double getweightThroughput()
        {
            return this.weightThroughput;
        }

        private System.Array ResizeArray(System.Array oldArray, int newSize)
        {
            System.Type elementType = oldArray.GetType().GetElementType();
            System.Array newArray = System.Array.CreateInstance(elementType, newSize);
            return newArray;
        }

        virtual public Chromosome SampleChromosome
		{
			get
			{
				return m_sampleChromosome;
			}
			
			set
			{
				verifyChangesAllowed();
				
				m_sampleChromosome = value;
				m_chromosomeSize = m_sampleChromosome.size();
			}
			
		}
		
        virtual public int ChromosomeSize
		{
			get
			{
				return m_chromosomeSize;
			}
			
		}
		
        virtual public SupportClass.ListCollectionSupport GeneticOperators
		{
			get
			{
				return m_geneticOperators;
			}
			
		}
		
        

        
        virtual public ChromosomePool ChromosomePool
		{
			get
			{
				return m_chromosomePool;
			}
			
			set
			{
				verifyChangesAllowed();
				
				m_chromosomePool = value;
			}
			
		}
		
        virtual public bool Locked
		{
			get
			{
				return m_settingsLocked;
			}
			
		}

        virtual public ChromosomeIndexCollection ChromosomeIndexes
        {
            get
            {
                return m_chromosomeIndexes;
            }

            set
            {
                m_chromosomeIndexes = value;
            }

        }

        virtual public MVARequest Request
        {
            get
            {
                return m_request;
            }

            set
            {
                m_request = value;
            }

        }
		
        public Configuration()
		{
			InitBlock();
			m_preSelectors = new ChainOfSelectors();
			m_postSelectors = new ChainOfSelectors();
		}
		
        public virtual void  setFitnessFunction(FitnessFunction a_functionToSet)
		{
			lock (this)
			{
				verifyChangesAllowed();		
				m_objectiveFunction = a_functionToSet;
			}
		}
		
        public virtual FitnessFunction getFitnessFunction()
		{
			return m_objectiveFunction;
		}
		
        public virtual void  setNaturalSelector(NaturalSelector a_selectorToSet)
		{
			lock (this)
			{
				addNaturalSelector(a_selectorToSet, false);
			}
		}
		
        public virtual NaturalSelector getNaturalSelector()
		{
			return getNaturalSelectors(false).get_Renamed(0);
		}
		
		public virtual ChainOfSelectors getNaturalSelectors(bool processBeforeGeneticOperators)
		{
			if (processBeforeGeneticOperators)
			{
				return m_preSelectors;
			}
			else
			{
				return m_postSelectors;
			}
		}
		
        public virtual void  setRandomGenerator(RandomGenerator a_generatorToSet)
		{
			lock (this)
			{
				verifyChangesAllowed();
				m_randomGenerator = a_generatorToSet;
			}
		}
		
        public virtual RandomGenerator getRandomGenerator()
		{
			return m_randomGenerator;
		}
		
		
        public virtual void  addGeneticOperator(GeneticOperator a_operatorToAdd)
		{
			lock (this)
			{
				verifyChangesAllowed();
				m_geneticOperators.Add(a_operatorToAdd);
			}
		}
		
        public virtual void  setPopulationSize(int a_sizeOfPopulation)
		{
			lock (this)
			{
				verifyChangesAllowed();
				m_populationSize = a_sizeOfPopulation;
			}
		}
		
        public virtual int getPopulationSize()
		{
			return m_populationSize;
		}
		
        public virtual void  lockSettings()
		{
			lock (this)
			{
				if (!m_settingsLocked)
				{
					verifyStateIsValid();
					
					// Make genetic operators list immutable.
					// --------------------------------------
					m_geneticOperators = new SupportClass.ListCollectionSupport(SupportClass.CollectionsManager.UnModifiableList(m_geneticOperators));
					
					m_settingsLocked = true;
				}
			}
		}
		
		
        public virtual void  verifyStateIsValid()
		{
			lock (this)
			{
				// First, make sure all of the required fields have been set to
				// appropriate values.
				// ------------------------------------------------------------
				if (m_objectiveFunction == null)
				{
					throw new InvalidConfigurationException("A desired fitness function or bulk fitness function must " + "be specified in the active configuration.");
				}
				
				if (m_sampleChromosome == null)
				{
					throw new InvalidConfigurationException("A sample instance of the desired Chromosome " + "setup must be specified in the active configuration.");
				}
				
				if (m_preSelectors.size() == 0 && m_postSelectors.size() == 0)
				{
					throw new InvalidConfigurationException("At least one desired natural selector must be specified in the" + " active configuration.");
				}
				
				if (m_randomGenerator == null)
				{
					throw new InvalidConfigurationException("A desired random number generator must be specified in the " + "active configuration.");
				}
				
				
				if ((m_geneticOperators.Count == 0))
				{
					throw new InvalidConfigurationException("At least one genetic operator must be specified in the " + "configuration.");
				}
				
				if (m_chromosomeSize <= 0)
				{
					throw new InvalidConfigurationException("A chromosome size greater than zero must be specified in " + "the active configuration.");
				}
				
				if (m_populationSize <= 0)
				{
					throw new InvalidConfigurationException("A genotype size greater than zero must be specified in " + "the active configuration.");
				}
				
				// Next, it's critical that each Gene implementation in the sample
				// Chromosome has a working equals() method, or else the genetic
				// engine will end up failing in mysterious and unpredictable ways.
				// We therefore verify right here that this method is working properly
				// in each of the Gene implementations used in the sample Chromosome.
				// -------------------------------------------------------------------
				Gene[] sampleGenes = m_sampleChromosome.Genes;
				for (int i = 0; i < sampleGenes.Length; i++)
				{
					Gene sampleCopy = sampleGenes[i].newGene(this);
					sampleCopy.setAllele(sampleGenes[i].getAllele());
					
					if (!(sampleCopy.Equals(sampleGenes[i])))
					{
						throw new InvalidConfigurationException("The sample Gene at gene position (locus) " + i + " does not appear to have a working equals() method. " + "When tested, the method returned false when comparing " + "the sample gene with a gene of the same type and " + "possessing the same value (allele).");
					}
				}
			}
		}
		
        protected internal virtual void  verifyChangesAllowed()
		{
			if (m_settingsLocked)
			{
				throw new InvalidConfigurationException("This Configuration object is locked. Settings may not be " + "altered.");
			}
		}
		
        public virtual void  addNaturalSelector(NaturalSelector a_selector, bool processBeforeGeneticOperators)
		{
			verifyChangesAllowed();
			if (processBeforeGeneticOperators)
			{
				m_preSelectors.addNaturalSelector(a_selector);
			}
			else
			{
				m_postSelectors.addNaturalSelector(a_selector);
			}
		}
	}
}