///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Specification for the on-delete statement.</summary>
    public class OnTriggerWindowDesc : OnTriggerDesc
    {
        private readonly String windowName;
        private readonly String optionalAsName;

        /// <summary>Ctor.</summary>
        /// <param name="windowName">the window name</param>
        /// <param name="optionalAsName">the optional alias</param>
        /// <param name="isOnDelete">true for on-delete and false for on-select</param>
        public OnTriggerWindowDesc(String windowName, String optionalAsName, bool isOnDelete)
            : base(isOnDelete ? OnTriggerType.ON_DELETE : OnTriggerType.ON_SELECT)
        {
            this.windowName = windowName;
            this.optionalAsName = optionalAsName;
        }

        /// <summary>Returns the window name.</summary>
        /// <returns>window name</returns>
        public String WindowName
        {
            get { return windowName; }
        }

        /// <summary>Returns the alias, or null if none defined.</summary>
        /// <returns>alias</returns>
        public String OptionalAsName
        {
            get { return optionalAsName; }
        }
    }
} // End of namespace
