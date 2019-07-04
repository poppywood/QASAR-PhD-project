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
using InvalidConfigurationException = org.jgap.InvalidConfigurationException;
using NaturalSelector = org.jgap.NaturalSelector;
namespace org.jgap.impl
{
	
	/// <summary> Ordered chain of NaturalSelectors. With this container you can plugin
	/// NaturalSelector implementations which will be performed either before (pre-)
	/// or after (post-selectors) registered genetic operations have been applied.
	/// </summary>
	/// <seealso cref="Genotype.evolve">
	/// </seealso>
	/// <seealso cref="Configuration.addNaturalSelector">
	/// 
	/// </seealso>
	/// <author>  Klaus Meffert
	/// @since 1.1
	/// </author>
	public class ChainOfSelectors
	{
		virtual public bool Empty
		{
			get
			{
				return size() == 0;
			}
			
		}
		/// <summary>String containing the CVS revision. Read out via reflection!</summary>
		private const System.String CVS_REVISION = "$Revision: 1.1 $";
		
		/// <summary> Ordered list holding the NaturalSelector's.
		/// Intentionally used as a decorator and not via inheritance!
		/// </summary>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		private SupportClass.ListCollectionSupport selectors;
		
		public ChainOfSelectors()
		{
			selectors = new SupportClass.ListCollectionSupport(10);
		}
		
		/// <summary> Adds a natural selector to the chain</summary>
		/// <param name="a_selector">the selector to be added
		/// @throws InvalidConfigurationException
		/// 
		/// @since 1.1 (previously part of class Configuration)
		/// </param>
		public virtual void  addNaturalSelector(NaturalSelector a_selector)
		{
			if (a_selector == null)
			{
				throw new InvalidConfigurationException("This Configuration object is locked. Settings may not be " + "altered.");
			}
			//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
			selectors.Add(a_selector);
		}
		
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.Collection'' and ''SupportClass.CollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		public virtual void  addAll(SupportClass.CollectionSupport c)
		{
			System.Collections.IEnumerator it = c.GetEnumerator();
			//UPGRADE_TODO: Method 'java.util.Iterator.hasNext' was converted to 'System.Collections.IEnumerator.MoveNext' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratorhasNext"'
			while (it.MoveNext())
			{
				//UPGRADE_TODO: Method 'java.util.Iterator.next' was converted to 'System.Collections.IEnumerator.Current' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilIteratornext"'
				NaturalSelector selector = (NaturalSelector) it.Current;
				addNaturalSelector(selector);
			}
		}
		
		public virtual int size()
		{
			return selectors.Count;
		}
		
		public override int GetHashCode()
		{
			return selectors.GetHashCode();
		}
		
		public  override bool Equals(System.Object o)
		{
			return SupportClass.EqualsSupport(selectors, o);
		}
		
		public virtual NaturalSelector get_Renamed(int index)
		{
			return (NaturalSelector) selectors.Get(index);
		}
		
		public virtual void  clear()
		{
			selectors.Clear();
		}
	}
}