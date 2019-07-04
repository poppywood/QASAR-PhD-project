///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.view;
using com.espertech.esper.view.internals;
using com.espertech.esper.view.std;

namespace com.espertech.esper.epl.named
{
    /// <summary>
    /// View capability requirement that asks views to handle the remove stream posted by parent views, for use with
    /// named windows since these allow on-delete removal of events from a window.
    /// <para/>
    /// Based on being asked to provide the capability, a view factory may need to use a view with a
    /// different internal collection to provide a remove stream capability that
    /// has good performance, but may come at the cost of lower insert performance as a view
    /// may need to build reverse indexes to effeciently remove an event.
    /// </summary>
	public class RemoveStreamViewCapability : ViewCapability
	{
	    public bool Inspect(int streamNumber, IList<ViewFactory> viewFactories, StatementContext statementContext)
	    {
	        foreach (ViewFactory viewFactory in viewFactories)
	        {
	            if ((viewFactory is GroupByViewFactory) || ((viewFactory is MergeViewFactory)))
	            {
	                continue;
	            }
	            if (!(viewFactory.CanProvideCapability(this)))
	            {
	                return false;
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
