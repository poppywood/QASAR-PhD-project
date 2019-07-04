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
using com.espertech.esper.client.soda;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Return result for unmap operators unmapping an intermal statement representation to the SODA object model.
	/// </summary>
	public class StatementSpecUnMapResult
	{
	    private readonly EPStatementObjectModel objectModel;
	    private readonly Map<int, SubstitutionParameterExpression> indexedParams;

	    /// <summary>Ctor.</summary>
	    /// <param name="objectModel">of the statement</param>
	    /// <param name="indexedParams">a map of parameter index and parameter</param>
	    public StatementSpecUnMapResult(EPStatementObjectModel objectModel, Map<int, SubstitutionParameterExpression> indexedParams)
	    {
	        this.objectModel = objectModel;
	        this.indexedParams = indexedParams;
	    }

	    /// <summary>Returns the object model.</summary>
	    /// <returns>object model</returns>
	    public EPStatementObjectModel ObjectModel
	    {
	    	get { return objectModel; }
	    }

	    /// <summary>
	    /// Returns the substitution paremeters keyed by the parameter's index.
	    /// </summary>
	    /// <returns>map of index and parameter</returns>
	    public Map<int, SubstitutionParameterExpression> IndexedParams
	    {
	    	get { return indexedParams; }
	    }
	}
} // End of namespace
