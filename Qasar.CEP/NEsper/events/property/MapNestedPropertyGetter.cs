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
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Getter for one or more levels deep nested properties of maps.
    /// </summary>
	public class MapNestedPropertyGetter : EventPropertyGetter
	{
	    private readonly EventPropertyGetter[] getterChain;
        private readonly BeanEventTypeFactory beanEventTypeFactory;

        /// <summary>Ctor.</summary>
        /// <param name="getterChain">is the chain of getters to retrieve each nested property</param>
        /// <param name="beanEventTypeFactory">is a factory for bean event types</param>
        public MapNestedPropertyGetter(ICollection<EventPropertyGetter> getterChain,
                                       BeanEventTypeFactory beanEventTypeFactory)
        {
            this.getterChain = CollectionHelper.ToArray(getterChain);
            this.beanEventTypeFactory = beanEventTypeFactory;
        }

	    public Object GetValue(EventBean eventBean)
	    {
	        Object value = null;

	        for (int i = 0; i < getterChain.Length; i++)
	        {
	            Object result = getterChain[i].GetValue(eventBean);

	            if (result == null)
	            {
	                return null;
	            }

	            // this is not the last element
	            if (i < (getterChain.Length - 1)) {
	                if (result is Map<string,object>)
	                {
                        eventBean = new MapEventBean((Map<string, object>)result, null);
	                }
	            else
	                {
	                    BeanEventType type =
	                        beanEventTypeFactory.CreateBeanType(result.GetType().FullName, result.GetType());
	                    eventBean = new BeanEventBean(result, type);
	                }
	            } else
	            {
	                value = result;
	            }
	        }
	        return value;
	    }

	    public bool IsExistsProperty(EventBean eventBean)
	    {
	        int lastElementIndex = getterChain.Length - 1;

	        // walk the getter chain up to the previous-to-last element, returning its object value.
	        // any null values in between mean the property does not exists
            for (int i = 0; i < getterChain.Length - 1; i++) {
                Object result = getterChain[i].GetValue(eventBean);

                if (result == null) {
                    return false;
                } else {
                    if (result is Map<string, object>) {
                        eventBean = new MapEventBean((Map<string, object>) result, null);
                    } else {
                        BeanEventType type =
                            beanEventTypeFactory.CreateBeanType(result.GetType().FullName, result.GetType());
                        eventBean = new BeanEventBean(result, type);
                    }
                }
            }

	        return getterChain[lastElementIndex].IsExistsProperty(eventBean);
	    }
	}
} // End of namespace
