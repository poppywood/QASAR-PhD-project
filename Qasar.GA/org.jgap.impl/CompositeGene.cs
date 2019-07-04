/*
* Copyright 2003 Klaus Meffert
*
* This file is part of JGAP.
*
* JGAP is free software; you can redistribute it and/or modify
* it under the terms of the GNU Lesser Public License as published by
* the Free Software Foundation; either version 2.1 of the License, or
* (at your option) any later version.
*
* JGAP is distributed in the hope that it will be useful,
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
* GNU Lesser Public License for more details.
*
* You should have received a copy of the GNU Lesser Public License
* along with JGAP; if not, write to the Free Software Foundation, Inc.,
* 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
*/
using System;
using Configuration = org.jgap.Configuration;
using Gene = org.jgap.Gene;
using RandomGenerator = org.jgap.RandomGenerator;
using UnsupportedRepresentationException = org.jgap.UnsupportedRepresentationException;
namespace org.jgap.impl
{
	
	/// <summary> Ordered container for multiple genes
	/// Has the same interface as a single gene and could be used accordingly.
	/// Use the addGene(Gene) method to add single genes (not CompositeGenes!) after
	/// construction, an empty CompositeGene without genes makes no sense.
	/// Beware that there are two equalities defined for a CompsoiteGene in respect
	/// to its contained genes:
	/// a) Two genes are (only) equal if they are identical
	/// b) Two genes are (seen as) equal if their equals method returns true
	/// 
	/// This influences several methods such as addGene. Notice that it is "better"
	/// to use addGene(a_gene, false) than addGene(a_gene, true) because the second
	/// variant only allows to add genes not seen as equal to already added genes in
	/// respect to their equals function. But: the equals function returns true for
	/// two different DoubleGenes (e.g.) just after their creation. If no specific
	/// (and hopefully different) allele is set for these DoubleGenes they are seen
	/// as equal!
	/// 
	/// </summary>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	[Serializable]
	public class CompositeGene : Gene
	{
		/// <returns> true: no genes contained, false otherwise
		/// 
		/// @since 1.1
		/// </returns>
		virtual public bool Empty
		{
			get
			{
				return (genes.Count == 0)?true:false;
			}
			
		}
		
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		/// <summary> Represents the delimiter that is used to separate genes in the
		/// persistent representation of CompositeGene instances.
		/// </summary>
		public const System.String GENE_DELIMITER = "*";
		
		private System.Collections.ArrayList genes;
		
		public CompositeGene()
		{
			genes = new System.Collections.ArrayList(10);
		}
		
		public virtual void  addGene(Gene a_gene)
		{
			addGene(a_gene, false);
		}
		
		/// <summary> Adds a gene to the CompositeGene's container. See comments in class
		/// header for additional details about equality (concerning "strict" param.)
		/// </summary>
		/// <param name="a_gene">the gene to be added
		/// </param>
		/// <param name="strict">false: add the given gene except the gene itself already is
		/// contained within the CompositeGene's container.
		/// true: add the gene if there is no other gene being equal to the
		/// given gene in request to the Gene's equals method
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  addGene(Gene a_gene, bool strict)
		{
			if (a_gene is CompositeGene)
			{
				throw new System.ArgumentException("It is not allowed to add a" + " CompositeGene to a CompositeGene!");
			}
			//check if gene already exists
			//----------------------------
			bool containsGene;
			if (!strict)
			{
				containsGene = containsGeneByIdentity(a_gene);
			}
			else
			{
				containsGene = genes.Contains(a_gene);
			}
			if (containsGene)
			{
				throw new System.ArgumentException("The gene is already contained" + " in the CompositeGene!");
			}
			genes.Add(a_gene);
		}
		
		/// <summary> Removes the given gene from the collection of genes. The gene is only
		/// removed if an object of the same identity is contained. The equals
		/// method will not be used here intentionally
		/// </summary>
		/// <param name="gene">the gene to be removed
		/// </param>
		/// <returns> true: given gene found and removed
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual bool removeGeneByIdentity(Gene gene)
		{
			bool result;
			int mySize = size();
			if (mySize < 1)
			{
				result = false;
			}
			else
			{
				result = false;
				for (int i = 0; i < mySize; i++)
				{
					if (geneAt(i) == gene)
					{
						System.Object temp_object;
						temp_object = genes[i];
						genes.RemoveAt(i);
						System.Object generatedAux = temp_object;
						result = true;
						break;
					}
				}
			}
			return result;
		}
		
		/// <summary> Removes the given gene from the collection of genes. The gene is
		/// removed if another gene exists that is equal to the given gene in respect
		/// to the equals method of the gene
		/// </summary>
		/// <param name="gene">the gene to be removed
		/// </param>
		/// <returns> true: given gene found and removed
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual bool removeGene(Gene gene)
		{
			System.Object temp_object;
			System.Boolean temp_boolean;
			temp_object = gene;
			temp_boolean = genes.Contains(temp_object);
			genes.Remove(temp_object);
			return temp_boolean;
		}
		
		/// <summary> Executed by the genetic engine when this Gene instance is no
		/// longer needed and should perform any necessary resource cleanup.
		/// 
		/// </summary>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  cleanup()
		{
			Gene gene;
			for (int i = 0; i < genes.Count; i++)
			{
				gene = (Gene) genes[i];
				gene.cleanup();
			}
		}
		
		/// <summary> See interface Gene for description</summary>
		/// <param name="a_numberGenerator">The random number generator that should be
		/// used to create any random values. It's important to use this
		/// generator to maintain the user's flexibility to configure the genetic
		/// engine to use the random number generator of their choice.
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  setToRandomValue(RandomGenerator a_numberGenerator)
		{
			Gene gene;
			for (int i = 0; i < genes.Count; i++)
			{
				gene = (Gene) genes[i];
				gene.setToRandomValue(a_numberGenerator);
			}
		}
		
		/// <summary> See interface Gene for description</summary>
		/// <param name="a_representation">the string representation retrieved from a
		/// prior call to the getPersistentRepresentation() method.
		/// 
		/// @throws UnsupportedRepresentationException
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  setValueFromPersistentRepresentation(System.String a_representation)
		{
			if ((System.Object) a_representation != null)
			{
				SupportClass.Tokenizer tokenizer = new SupportClass.Tokenizer(a_representation, GENE_DELIMITER);
				
				int numberGenes = tokenizer.Count;
				System.String singleGene;
				System.String geneTypeClass;
				try
				{
					for (int i = 0; i < numberGenes; i++)
					{
						singleGene = tokenizer.NextToken();
						SupportClass.Tokenizer geneTypeTokenizer = new SupportClass.Tokenizer(singleGene, org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER);
						
						//read type for every gene and then newly construct it
						//----------------------------------------------------
						geneTypeClass = geneTypeTokenizer.NextToken();
						//UPGRADE_TODO: Format of parameters of method 'java.lang.Class.forName' are different in the equivalent in .NET. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1092"'
						System.Type clazz = System.Type.GetType(geneTypeClass);
						//UPGRADE_TODO: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073"'
						//UPGRADE_WARNING: Method 'java.lang.Class.newInstance' was converted to 'SupportClass.CreateNewInstance' which may throw an exception. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1101"'
						Gene gene = (Gene) SupportClass.CreateNewInstance(clazz);
						
						//now work with the freshly constructed genes
						// ------------------------------------------
						System.String rep = "";
						while (geneTypeTokenizer.HasMoreTokens())
						{
							if (rep.Length > 0)
							{
								rep += org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER;
							}
							rep += geneTypeTokenizer.NextToken();
						}
						gene.setValueFromPersistentRepresentation(rep);
						addGene(gene);
					}
				}
				catch (System.Exception ex)
				{
					throw new UnsupportedRepresentationException(ex.GetBaseException().Message);
				}
			}
		}
		
		/// <summary> See interface Gene for description</summary>
		/// <returns> A string representation of this Gene's current state.
		/// @throws UnsupportedOperationException
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual System.String getPersistentRepresentation()
		{
			System.String result = "";
			Gene gene;
			for (int i = 0; i < genes.Count; i++)
			{
				gene = (Gene) genes[i];
				
				//save type with every gene to make the process reversible
				//--------------------------------------------------------
				result += gene.GetType().FullName;
				result += org.jgap.Gene_Fields.PERSISTENT_FIELD_DELIMITER;
				
				//get persistent representation from each gene itself
				result += gene.getPersistentRepresentation();
				
				if (i < genes.Count - 1)
				{
					result += GENE_DELIMITER;
					/// <summary>@todo if GENE_DELIMITER occurs in a gene (e.g. StringGene)
					/// undertake actions to maintain consistency
					/// </summary>
				}
			}
			return result;
		}
		
		/// <summary> Retrieves the value represented by this Gene. All values returned
		/// by this class will be Vector instances. Each element of the Vector
		/// represents the allele of the corresponding gene in the CompositeGene's
		/// container
		/// 
		/// </summary>
		/// <returns> the Boolean value of this Gene.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual System.Object getAllele()
		{
			System.Collections.ArrayList alleles = new System.Collections.ArrayList(10);
			Gene gene;
			for (int i = 0; i < genes.Count; i++)
			{
				gene = (Gene) genes[i];
				alleles.Add(gene.getAllele());
			}
			return alleles;
		}
		
		/// <summary> Sets the value of the contained Genes to the new given value. This class
		/// expects the value to be of a Vector type. Each element of the Vector
		/// must conform with the type of the gene in the CompositeGene's container
		/// at the corresponding position.
		/// 
		/// </summary>
		/// <param name="a_newValue">the new value of this Gene instance.
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  setAllele(System.Object a_newValue)
		{
			if (!(a_newValue is System.Collections.ArrayList))
			{
				throw new System.ArgumentException("The expected type of the allele" + " is a Vector.");
			}
			System.Collections.ArrayList alleles = (System.Collections.ArrayList) a_newValue;
			Gene gene;
			for (int i = 0; i < alleles.Count; i++)
			{
				gene = (Gene) genes[i];
				gene.setAllele(alleles[i]);
			}
		}
		
		/// <summary> Provides an implementation-independent means for creating new Gene
		/// instances. The new instance that is created and returned should be
		/// setup with any implementation-dependent configuration that this Gene
		/// instance is setup with (aside from the actual value, of course). For
		/// example, if this Gene were setup with bounds on its value, then the
		/// Gene instance returned from this method should also be setup with
		/// those same bounds. This is important, as the JGAP core will invoke this
		/// method on each Gene in the sample Chromosome in order to create each
		/// new Gene in the same respective gene position for a new Chromosome.
		/// <p>
		/// It should be noted that nothing is guaranteed about the actual value
		/// of the returned Gene and it should therefore be considered to be
		/// undefined.
		/// 
		/// </summary>
		/// <param name="a_activeConfiguration">The current active configuration.
		/// </param>
		/// <returns> A new Gene instance of the same type and with the same
		/// setup as this concrete Gene.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual Gene newGene(Configuration a_activeConfiguration)
		{
			CompositeGene compositeGene = new CompositeGene();
			Gene gene;
			int geneSize = genes.Count;
			for (int i = 0; i < geneSize; i++)
			{
				gene = (Gene) genes[i];
				compositeGene.addGene(gene.newGene(a_activeConfiguration), false);
			}
			return compositeGene;
		}
		
		/// <summary> Compares this CompositeGene with the specified object for order. A
		/// false value is considered to be less than a true value. A null value
		/// is considered to be less than any non-null value.
		/// 
		/// </summary>
		/// <param name="other">the CompositeGene to be compared.
		/// </param>
		/// <returns>  a negative integer, zero, or a positive integer as this object
		/// is less than, equal to, or greater than the specified object.
		/// 
		/// @throws ClassCastException if the specified object's type prevents it
		/// from being compared to this CompositeGene.
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual int CompareTo(System.Object other)
		{
			CompositeGene otherCompositeGene = (CompositeGene) other;
			// First, if the other gene (or its value) is null, then this is
			// the greater allele. Otherwise, just use the contained genes' compareTo
			// method to perform the comparison.
			// ---------------------------------------------------------------
			if (otherCompositeGene == null)
			{
				return 1;
			}
			else if (otherCompositeGene.Empty)
			{
				// If our value is also null, then we're the same. Otherwise,
				// this is the greater gene.
				// ----------------------------------------------------------
				return Empty?0:1;
			}
			else
			{
				//compare each gene against each other
				// -----------------------------------
				int numberGenes = System.Math.Min(size(), otherCompositeGene.size());
				Gene gene1;
				Gene gene2;
				for (int i = 0; i < numberGenes; i++)
				{
					gene1 = geneAt(i);
					gene2 = otherCompositeGene.geneAt(i);
					if (gene1 == null)
					{
						if (gene2 == null)
						{
							continue;
						}
						else
						{
							return - 1;
						}
					}
					else
					{
						int result = gene1.CompareTo(gene2);
						if (result != 0)
						{
							return result;
						}
					}
				}
				//if everything is equal until now the CompositeGene with more
				//contained genes wins
				// -----------------------------------------------------------
				if (size() == otherCompositeGene.size())
				{
					return 0;
				}
				else
				{
					return size() > otherCompositeGene.size()?1:- 1;
				}
			}
		}
		
		/// <summary> Compares this CompositeGene with the given object and returns true if
		/// the other object is a IntegerGene and has the same value (allele) as
		/// this IntegerGene. Otherwise it returns false.
		/// 
		/// </summary>
		/// <param name="other">the object to compare to this IntegerGene for equality.
		/// </param>
		/// <returns> true if this IntegerGene is equal to the given object,
		/// false otherwise.
		/// 
		/// @since 1.1
		/// </returns>
		public  override bool Equals(System.Object other)
		{
			try
			{
				return CompareTo(other) == 0;
			}
			catch (System.InvalidCastException )
			{
				// If the other object isn't an IntegerGene, then we're not
				// equal.
				// ----------------------------------------------------------
				return false;
			}
		}
		
		/// <summary> Retrieves a string representation of this CompositeGene's value that
		/// may be useful for display purposes.
		/// </summary>
		/// <returns> a string representation of this CompositeGene's value. Every
		/// contained gene's string representation is delimited by the given
		/// delimiter
		/// 
		/// @since 1.1
		/// </returns>
		public override System.String ToString()
		{
			if ((genes.Count == 0))
			{
				return "null";
			}
			else
			{
				System.String result = "";
				Gene gene;
				for (int i = 0; i < genes.Count; i++)
				{
					gene = (Gene) genes[i];
					//UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Object.toString' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
					result += gene;
					if (i < genes.Count - 1)
					{
						result += GENE_DELIMITER;
					}
				}
				return result;
			}
		}
		
		/// <summary> Returns the gene at the given index</summary>
		/// <param name="index">sic
		/// </param>
		/// <returns> the gene at the given index
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual Gene geneAt(int index)
		{
			return (Gene) genes[index];
		}
		
		/// <returns> the number of genes contained
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual int size()
		{
			return genes.Count;
		}
		
		/// <summary> Checks whether a specific gene is already contained. The determination
		/// will be done by checking for identity and not using the equal method!
		/// </summary>
		/// <param name="gene">the gene under test
		/// </param>
		/// <returns> true: the given gene object is contained
		/// 
		/// </returns>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual bool containsGeneByIdentity(Gene gene)
		{
			bool result;
			int mySize = size();
			if (mySize < 1)
			{
				result = false;
			}
			else
			{
				result = false;
				for (int i = 0; i < mySize; i++)
				{
					
					//check for identity
					//------------------
					if (geneAt(i) == gene)
					{
						result = true;
						break;
					}
				}
			}
			return result;
		}
		
		/// <summary> Don't use this method, is makes no sense here. It is just there to
		/// satisfy the Gene interface.
		/// Instead, loop over all cotnained genes and call their applyMutation
		/// method.
		/// </summary>
		/// <param name="index">does not matter here
		/// </param>
		/// <param name="a_percentage">does not matter here
		/// 
		/// </param>
		/// <author>  Klaus Meffert
		/// @since 1.1
		/// </author>
		public virtual void  applyMutation(int index, double a_percentage)
		{
			for (int i = 0; i < size(); i++)
			{
				// problem here: size() of CompositeGene not equal to (different)
				// sizes of contained genes.
				// Solution: Don't use CompositeGene.applyMutation, instead loop
				//           over all contained genes and call their method
				// -------------------------------------------------------------
				throw new System.SystemException("applyMutation may not be called for" + " a CompositeGene. Call this method for each gene contained" + " in the CompositeGene.");
			}
		}
	}
}