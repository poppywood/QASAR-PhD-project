///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>
    /// For use in select clauses for specifying a selected stream: select a.* from MyEvent as a, MyOther as b
    /// </summary>
	public class SelectExprElementStreamRawSpec : MetaDefItem
	{
	    private readonly String streamAliasName;
        private readonly String optionalAsName;

	    /// <summary>Ctor.</summary>
	    /// <param name="streamAliasName">is the stream alias of the stream to select</param>
	    /// <param name="optionalAsName">is the column alias</param>
	    public SelectExprElementStreamRawSpec(String streamAliasName, String optionalAsName)
	    {
	        this.streamAliasName = streamAliasName;
	        this.optionalAsName = optionalAsName;
	    }

	    /// <summary>
	    /// Returns the stream alias (e.g. select streamAlias from MyEvent as streamAlias).
	    /// </summary>
	    /// <returns>alias</returns>
	    public String StreamAliasName
	    {
	        get {return streamAliasName;}
	    }

	     /// <summary>
	     /// Returns the column alias (e.g. select streamAlias as mycol from MyEvent as streamAlias).
	     /// </summary>
	     /// <returns>alias</returns>
	    public String OptionalAsName
	    {
	        get {return optionalAsName;}
	    }
	}
} // End of namespace
