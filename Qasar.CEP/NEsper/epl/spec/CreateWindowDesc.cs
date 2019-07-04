///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Specification for creating a named window.</summary>
    public class CreateWindowDesc : MetaDefItem
    {
        private readonly String windowName;
        private readonly IList<ViewSpec> viewSpecs;

        /// <summary>Ctor.</summary>
        /// <param name="windowName">the window name</param>
        /// <param name="viewSpecs">the view definitions</param>
        public CreateWindowDesc(String windowName, IList<ViewSpec> viewSpecs)
        {
            this.windowName = windowName;
            this.viewSpecs = viewSpecs;
        }

        /// <summary>Returns the window name.</summary>
        /// <returns>window name</returns>
        public String WindowName
        {
            get { return windowName; }
        }

        /// <summary>Returns the view specifications.</summary>
        /// <returns>view specs</returns>
        public IList<ViewSpec> ViewSpecs
        {
            get { return viewSpecs; }
        }
    }
} // End of namespace
