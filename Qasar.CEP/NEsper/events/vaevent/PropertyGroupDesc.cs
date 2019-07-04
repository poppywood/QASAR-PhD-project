///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// For use with building groups of event properties to reduce overhead in maintaining versions.
    /// </summary>
    public class PropertyGroupDesc {
    
        private readonly int groupNum;
        private readonly Map<EventType, String> types;
        private readonly String[] properties;
    
        /// <summary>Ctor. </summary>
        /// <param name="groupNum">the group number</param>
        /// <param name="aliasTypeSet">the event types and their aliases whose totality of properties fully falls within this group.</param>
        /// <param name="properties">is the properties in the group</param>
        public PropertyGroupDesc(int groupNum, Map<EventType, String> aliasTypeSet, String[] properties) {
            this.groupNum = groupNum;
            this.types = aliasTypeSet;
            this.properties = properties;
        }

        /// <summary>Returns the group number. </summary>
        /// <returns>group number</returns>
        public int GroupNum
        {
            get { return groupNum; }
        }

        /// <summary>Returns the types. </summary>
        /// <returns>types</returns>
        public Map<EventType, string> Types
        {
            get { return types; }
        }

        /// <summary>Returns the properties. </summary>
        /// <returns>properties</returns>
        public ICollection<string> Properties
        {
            get { return properties; }
        }

        public override String ToString()
        {
            return "groupNum=" + groupNum +
                   " properties=" + CollectionHelper.Render(properties) +
                   " aliasTypes=" + types;
        }
    }
}
