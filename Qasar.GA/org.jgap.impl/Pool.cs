/*
* Copyright 2001-2003 Neil Rotstan
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
namespace org.jgap.impl
{
	
	
	/// <summary> A simple, generic pool class that can be used to pool any kind of object.
	/// Objects can be released to this pool, either individually or as a
	/// Collection, and then later acquired again. It is not necessary for an
	/// object to have been originally acquired from the pool in order for it to
	/// be released to the pool. If there are no objects present in the pool,
	/// an attempt to acquire one will return null. The number of objects
	/// available in the pool can be determined with the size() method. Finally,
	/// it should be noted that the pool does not attempt to perform any kind
	/// of cleanup or re-initialization on the objects to restore them to some
	/// clean state when they are released to the pool; it's up to the user to
	/// reset any necessary state in the object prior to the release call (or
	/// just after the acquire call).
	/// 
	/// </summary>
	/// <author>  Neil Rotstan
	/// @since 1.0
	/// </author>
	public class Pool
	{
		/// <summary> The List of Objects currently in the pool.</summary>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.List'' and ''SupportClass.ListCollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		private SupportClass.ListCollectionSupport m_pooledObjects;
		
		
		/// <summary> Constructor.</summary>
		public Pool()
		{
			//UPGRADE_ISSUE: Class hierarchy differences between 'java.util.ArrayList' and 'System.Collections.ArrayList' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
			m_pooledObjects = new SupportClass.ListCollectionSupport();
		}
		
		
		/// <summary> Attempts to acquire an Object instance from the pool. It should
		/// be noted that no cleanup or re-initialization occurs on these
		/// objects, so it's up to the caller to reset the state of the
		/// returned Object if that's desirable.
		/// 
		/// </summary>
		/// <returns> An Object instance from the pool or null if no 
		/// Object instances are available in the pool.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'acquirePooledObject'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual System.Object acquirePooledObject()
		{
			lock (this)
			{
				if ((m_pooledObjects.Count == 0))
				{
					return null;
				}
				else
				{
					// Remove the last Object in the pool and return it.
					// Note that removing the last Object (as opposed to the first
					// one) is an optimization because it prevents the ArrayList
					// from resizing itself.
					// -----------------------------------------------------------
					//UPGRADE_TODO: Method 'java.util.List.remove' was converted to 'System.Collections.IList.Remove' which has a different behavior. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1073_javautilListremove_int"'
					return m_pooledObjects.RemoveElement(m_pooledObjects.Count - 1);
				}
			}
		}
		
		
		/// <summary> Releases an Object to the pool. It's not required that the Object
		/// originated from the pool--any Object can be released to it.
		/// 
		/// </summary>
		/// <param name="a_objectToPool">The Object instance to be released into
		/// the pool.
		/// </param>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'releaseObject'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual void  releaseObject(System.Object a_objectToPool)
		{
			lock (this)
			{
				//UPGRADE_TODO: The equivalent in .NET for method 'java.util.List.add' may return a different value. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1043"'
				m_pooledObjects.Add(a_objectToPool);
			}
		}
		
		
		/// <summary> Releases a Collection of objects to the pool. It's not required that
		/// the objects in the Collection originated from the pool--any objects
		/// can be released to it.
		/// 
		/// </summary>
		/// <param name="a_objectsToPool">The Collection of objects to release into
		/// the pool.
		/// </param>
		//UPGRADE_ISSUE: Class hierarchy differences between ''java.util.Collection'' and ''SupportClass.CollectionSupport'' may cause compilation errors. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1186"'
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'releaseAllObjects'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual void  releaseAllObjects(System.Collections.ICollection a_objectsToPool)
		{
			lock (this)
			{
				if (a_objectsToPool != null)
				{
					m_pooledObjects.AddRange(a_objectsToPool);
				}
			}
		}
		
		
		/// <summary> Retrieves the number of objects currently available in this pool.
		/// 
		/// </summary>
		/// <returns> the number of objects in this pool.
		/// </returns>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'size'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual int size()
		{
			lock (this)
			{
				return m_pooledObjects.Count;
			}
		}
		
		
		/// <summary> Empties out this pool of all objects.</summary>
		//UPGRADE_NOTE: Synchronized keyword was removed from method 'clear'. Lock expression was added. 'ms-help://MS.VSCC.2003/commoner/redir/redirect.htm?keyword="jlca1027"'
		public virtual void  clear()
		{
			lock (this)
			{
				m_pooledObjects.Clear();
			}
		}
	}
}