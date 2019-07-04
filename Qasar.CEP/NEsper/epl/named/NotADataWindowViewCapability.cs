///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.view;
using com.espertech.esper.view.std;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// Expresses the requirement that all views are derived-value views and now data window views, with the exception of
    /// group-by and merge views.
    /// </summary>
	public class NotADataWindowViewCapability : ViewCapability
	{
	    public bool Inspect(int streamNumber, IList<ViewFactory> viewFactories, StatementContext statementContext)
	    {
	        foreach (ViewFactory viewFactory in viewFactories)
	        {
	            if ((viewFactory is GroupByViewFactory) || ((viewFactory is MergeViewFactory)))
	            {
	                continue;
	            }
                if (viewFactory is DataWindowViewFactory)
	            {
	                throw new ExprValidationException(NamedWindowServiceConstants.ERROR_MSG_NO_DATAWINDOW_ALLOWED);
	            }
	        }

	        return true;
	    }

	    public bool RequiresChildViews()
	    {
	        return false;
	    }
	}
} // End of namespace
