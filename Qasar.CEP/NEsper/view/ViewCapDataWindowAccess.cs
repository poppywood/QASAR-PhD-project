///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using net.esper.core;
using net.esper.view.std;

namespace net.esper.view
{
	/// <summary>Describes that we need random access into a data window by index.</summary>
	public class ViewCapDataWindowAccess : ViewCapability
	{
        /**
         * Ctor.
         */
        public ViewCapDataWindowAccess()
        {
        }

        /// <summary>
        /// Inspect view factories returning false to indicate that view factories do not meet
        /// view resource requirements, or true to indicate view capability and view factories can be compatible.
        /// </summary>
        /// <param name="streamNumber">is the number of the stream</param>
        /// <param name="viewFactories">is a list of view factories that originate the final views</param>
        /// <param name="statementContext">is the statement-level services</param>
        /// <returns>
        /// true to indicate inspection success, or false to indicate inspection failure
        /// </returns>
	    public bool Inspect(int streamNumber, IList<ViewFactory> viewFactories, StatementContext statementContext)
	    {
	        // We allow the capability only if
	        //  - 1 view
	        //  - 2 views and the first view is a group-by (for window-per-group access)
	        if (viewFactories.Count == 1)
	        {
	            return true;
	        }
	        if (viewFactories.Count == 2)
	        {
	        	if (viewFactories[0] is GroupByViewFactory)
	            {
	                return true;
	            }
	            return false;
	        }
	        return true;
	    }
	}
} // End of namespace
