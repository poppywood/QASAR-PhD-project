///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;

using com.espertech.esper.events;
using com.espertech.esper.util;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>Descriptor for a variant stream property. </summary>
    public class VariantPropertyDesc
    {
        private readonly Type propertyType;
        private readonly EventPropertyGetter getter;
        private readonly bool isProperty;
    
        /// <summary>Ctor. </summary>
        /// <param name="propertyType">type or null if not exists</param>
        /// <param name="getter">the getter or null if not exists</param>
        /// <param name="property">the bool indicating whether it exists or not</param>
        public VariantPropertyDesc(Type propertyType, EventPropertyGetter getter, bool property)
        {
            this.propertyType = TypeHelper.GetBoxedType(propertyType);
            this.getter = getter;
            isProperty = property;
        }
    
        /// <summary>True if the property exists, false if not. </summary>
        /// <returns>indicator whether property exists</returns>
        public bool IsProperty
        {
            get { return isProperty; }
        }

        /// <summary>Returns the property type. </summary>
        /// <returns>property type</returns>
        public Type PropertyType
        {
            get { return propertyType; }
        }

        /// <summary>Returns the getter for the property. </summary>
        /// <returns>property getter</returns>
        public EventPropertyGetter Getter
        {
            get { return getter; }
        }
    }
}
