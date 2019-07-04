///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Specification for on-trigger statements.</summary>
	public abstract class OnTriggerDesc : MetaDefItem
	{
	    private readonly OnTriggerType onTriggerType;

	    /// <summary>Ctor.</summary>
	    /// <param name="onTriggerType">the type of on-trigger</param>
	    public OnTriggerDesc(OnTriggerType onTriggerType)
	    {
	        this.onTriggerType = onTriggerType;
	    }

	    /// <summary>Returns the type of the on-trigger statement.</summary>
	    /// <returns>trigger type</returns>
	    public OnTriggerType OnTriggerType
	    {
            get { return onTriggerType; }
	    }
	}
} // End of namespace
