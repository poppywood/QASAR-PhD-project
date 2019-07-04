///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;

using com.espertech.esper.client;
using com.espertech.esper.core;
using com.espertech.esper.epl.expression;
using com.espertech.esper.epl.named;
using com.espertech.esper.events;
using com.espertech.esper.view;

namespace com.espertech.esper.events.vaevent
{
    /// <summary>
    /// Represents a variant event stream, allowing events of disparate event types to be
    /// treated polymophically.
    /// </summary>
    public class VAEVariantProcessor : ValueAddEventProcessor
    {
        private readonly VariantSpec variantSpec;
        private readonly VariantEventType variantEventType;
    
        /// <summary>Ctor. </summary>
        /// <param name="variantSpec">specifies how to handle the disparate events</param>
        public VAEVariantProcessor(VariantSpec variantSpec)
        {
            this.variantSpec = variantSpec;
    
            VariantPropResolutionStrategy strategy;
            if (variantSpec.TypeVariance == TypeVariance.ANY)
            {
                strategy = new VariantPropResolutionStrategyAny(variantSpec);
            }
            else
            {
                strategy = new VariantPropResolutionStrategyDefault(variantSpec);
            }
            variantEventType = new VariantEventType(variantSpec, strategy);
        }

        public EventType ValueAddEventType
        {
            get { return variantEventType; }
        }

        public void ValidateEventType(EventType eventType)
        {
            if (variantSpec.TypeVariance == TypeVariance.ANY)
            {
                return;
            }
    
            if (eventType == null)
            {
                throw new ExprValidationException(GetMessage());
            }
            
            foreach (EventType variant in variantSpec.EventTypes)
            {
                if (variant == eventType)
                {
                    return;
                }
    
                // Check all the supertypes to see if one of the matches the full or delta types
                IEnumerable<EventType> deepSupers = eventType.DeepSuperTypes;
                if (deepSupers == null)
                {
                    throw new ExprValidationException(GetMessage());
                }

                foreach( EventType type in deepSupers ) {
                    if ( type == eventType ) {
                        return;
                    }
                }
            }
    
            throw new ExprValidationException(GetMessage());        
        }
    
        public EventBean GetValueAddEventBean(EventBean @event)
        {
            return new VariantEventBean(variantEventType, @event);
        }
    
        public void OnUpdate(EventBean[] newData, EventBean[] oldData, NamedWindowRootView namedWindowRootView, NamedWindowIndexRepository indexRepository)
        {
            throw new NotSupportedException();
        }
    
        public ICollection<EventBean> GetSnapshot(EPStatementHandle createWindowStmtHandle, Viewable parent)
        {
            throw new NotSupportedException();
        }
    
        public void RemoveOldData(EventBean[] oldData, NamedWindowIndexRepository indexRepository)
        {
            throw new NotSupportedException();
        }
    
        private String GetMessage()
        {
            return "Selected event type is not a valid event type of the variant stream '" + variantSpec.VariantStreamName + "'";
        }
    }
}
