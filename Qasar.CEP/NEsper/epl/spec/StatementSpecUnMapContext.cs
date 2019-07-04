///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.compat;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Un-mapping context for mapping from an internal specifications to an SODA object model.
	/// </summary>
	public class StatementSpecUnMapContext
	{
	    private readonly Map<int, SubstitutionParameterExpression> indexedParams;

	    /// <summary>Ctor.</summary>
	    public StatementSpecUnMapContext()
	    {
	        indexedParams = new HashMap<int, SubstitutionParameterExpression>();
	    }

	    /// <summary>Adds a substitution parameters.</summary>
	    /// <param name="index">is the index of the parameter</param>
	    /// <param name="subsParam">is the parameter expression node</param>
	    public void Add(int index, SubstitutionParameterExpression subsParam)
	    {
	        if (indexedParams.ContainsKey(index))
	        {
	            throw new IllegalStateException("Index '" + index + "' already found in collection");
	        }
	        indexedParams.Put(index, subsParam);
	    }

	    /// <summary>Returns all indexed parameters.</summary>
	    /// <returns>map of parameter index and parameter expression node</returns>
	    public Map<int, SubstitutionParameterExpression> IndexedParams
	    {
	    	get { return indexedParams; }
	    }

	    /// <summary>
	    /// Adds all substitution parameters. Checks if indexes already exists
	    /// and throws an exception if they do.
	    /// </summary>
	    /// <param name="inner">to indexes and parameters to add</param>
	    public void AddAll(Map<int, SubstitutionParameterExpression> inner)
	    {
	        foreach (KeyValuePair<int, SubstitutionParameterExpression> entry in inner)
	        {
	            if (indexedParams.ContainsKey(entry.Key))
	            {
	                throw new IllegalStateException("Index '" + entry.Key + "' already found in collection");
	            }
	            indexedParams.Put(entry.Key, entry.Value);
	        }
	    }
	}
} // End of namespace
