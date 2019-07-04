///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

namespace com.espertech.esper.epl.lookup
{
    /// <summary>
    /// Holds property information for joined properties in a lookup.
    /// </summary>
	public class JoinedPropDesc : IComparable
	{
        private readonly String indexPropName;
	    private readonly Type coercionType;
        private readonly String keyPropName;
        private readonly int? keyStreamId;

	    /// <summary>Ctor.</summary>
	    /// <param name="indexPropName">is the property name of the indexed field</param>
	    /// <param name="coercionType">is the type to coerce to</param>
	    /// <param name="keyPropName">is the property name of the key field</param>
	    /// <param name="keyStreamId">is the stream number of the key field</param>
	    public JoinedPropDesc(String indexPropName, Type coercionType, String keyPropName, int? keyStreamId)
	    {
	        this.indexPropName = indexPropName;
	        this.coercionType = coercionType;
	        this.keyPropName = keyPropName;
	        this.keyStreamId = keyStreamId;
	    }

	    /// <summary>Returns the property name of the indexed field.</summary>
	    /// <returns>property name of indexed field</returns>
	    public String GetIndexPropName()
	    {
	        return indexPropName;
	    }

	    /// <summary>Returns the coercion type of key to index field.</summary>
	    /// <returns>type to coerce to</returns>
	    public Type CoercionType
	    {
	        get { return coercionType;}
	    }

	    /// <summary>Returns the property name of the key field.</summary>
	    /// <returns>property name of key field</returns>
	    public String KeyPropName
	    {
	        get {return keyPropName;}
	    }

	    /// <summary>Returns the stream id of the key field.</summary>
	    /// <returns>stream id</returns>
	    public int? KeyStreamId
	    {
	        get {return keyStreamId;}
	    }

	    /// <summary>Returns the key stream numbers.</summary>
	    /// <param name="descList">a list of descriptors</param>
	    /// <returns>key stream numbers</returns>
	    public static int[] GetKeyStreamNums(ICollection<JoinedPropDesc> descList)
	    {
	        int[] streamIds = new int[descList.Count];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            streamIds[count++] = desc.KeyStreamId.Value;
	        }
	        return streamIds;
	    }

	    /// <summary>Returns the key property names.</summary>
	    /// <param name="descList">a list of descriptors</param>
	    /// <returns>key property names</returns>
	    public static String[] GetKeyProperties(ICollection<JoinedPropDesc> descList)
	    {
	        String[] result = new String[descList.Count];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            result[count++] = desc.KeyPropName;
	        }
	        return result;
	    }

	    /// <summary>Returns the key property names.</summary>
	    /// <param name="descList">a list of descriptors</param>
	    /// <returns>key property names</returns>
	    public static String[] GetKeyProperties(JoinedPropDesc[] descList)
	    {
	        String[] result = new String[descList.Length];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            result[count++] = desc.KeyPropName;
	        }
	        return result;
	    }

	    /// <summary>Returns the index property names given an array of descriptors.</summary>
	    /// <param name="descList">descriptors of joined properties</param>
	    /// <returns>array of index property names</returns>
	    public static String[] GetIndexProperties(JoinedPropDesc[] descList)
	    {
	        String[] result = new String[descList.Length];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            result[count++] = desc.GetIndexPropName();
	        }
	        return result;
	    }

	    /// <summary>Returns the key coercion types.</summary>
	    /// <param name="descList">a list of descriptors</param>
	    /// <returns>key coercion types</returns>
	    public static Type[] GetCoercionTypes(ICollection<JoinedPropDesc> descList)
	    {
	        Type[] result = new Type[descList.Count];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            result[count++] = desc.CoercionType;
	        }
	        return result;
	    }

	    /// <summary>Returns the key coercion types.</summary>
	    /// <param name="descList">a list of descriptors</param>
	    /// <returns>key coercion types</returns>
	    public static Type[] GetCoercionTypes(JoinedPropDesc[] descList)
	    {
            Type[] result = new Type[descList.Length];
	        int count = 0;
	        foreach (JoinedPropDesc desc in descList)
	        {
	            result[count++] = desc.CoercionType;
	        }
	        return result;
	    }

	    public int CompareTo(Object o)
	    {
	        JoinedPropDesc other = (JoinedPropDesc) o;
	        return indexPropName.CompareTo(other.GetIndexPropName());
	    }

	    public override bool Equals(Object o)
	    {
	        if (this == o)
	        {
	            return true;
	        }
	        if (o == null || GetType() != o.GetType())
	        {
	            return false;
	        }

	        JoinedPropDesc that = (JoinedPropDesc) o;

	        if (!coercionType.Equals(that.coercionType))
	        {
	            return false;
	        }
	        if (!indexPropName.Equals(that.indexPropName))
	        {
	            return false;
	        }
	        if (!keyPropName.Equals(that.keyPropName))
	        {
	            return false;
	        }
	        if (!keyStreamId.Equals(that.keyStreamId))
	        {
	            return false;
	        }

	        return true;
	    }

	    public override int GetHashCode()
	    {
	        int result;
	        result = indexPropName.GetHashCode();
	        result = 31 * result + coercionType.GetHashCode();
	        result = 31 * result + keyPropName.GetHashCode();
	        result = 31 * result + keyStreamId.GetHashCode();
	        return result;
	    }
	}
} // End of namespace
