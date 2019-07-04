///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Reflection;

using com.espertech.esper.collection;
using com.espertech.esper.events;

using CGLib;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// A result delivery strategy that uses an "update" method that accepts a underlying 
    /// array for use in wildcard selection. 
    /// </summary>
    public class ResultDeliveryStrategyTypeArr : ResultDeliveryStrategy
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Object subscriber;
        private readonly FastMethod fastMethod;
        private readonly Type componentType;
    
        /// <summary>Ctor. </summary>
        /// <param name="subscriber">is the receiver to method invocations</param>
        /// <param name="method">is the method to deliver to</param>
        public ResultDeliveryStrategyTypeArr(Object subscriber, MethodInfo method)
        {
            this.subscriber = subscriber;
            FastClass fastClass = FastClass.Create(subscriber.GetType());
            this.fastMethod = fastClass.GetMethod(method);
            componentType = method.GetParameters()[0].ParameterType.GetElementType();
        }
    
        public void Execute(UniformPair<EventBean[]> result)
        {
            Object newData = Convert(result.First);
            Object oldData = Convert(result.Second);
    
            Object[] paramList = new Object[] {newData, oldData};
            try {
                fastMethod.Invoke(subscriber, paramList);
            }
            catch (TargetInvocationException e) {
                ResultDeliveryStrategyImpl.Handle(log, e, paramList, subscriber, fastMethod);
            }
        }
    
        private Object Convert(EventBean[] events)
        {
            if ((events == null) || (events.Length == 0))
            {
                return null;
            }
    
            Array array = Array.CreateInstance(componentType, events.Length);
            int length = 0;
            for (int i = 0; i < events.Length; i++)
            {
                if (events[i] is NaturalEventBean)
                {
                    NaturalEventBean natural = (NaturalEventBean) events[i];
                    array.SetValue(natural.Natural[0], length);
                    length++;
                }
            }
    
            if (length == 0)
            {
                return null;
            }
            if (length != events.Length)
            {
                Array reduced = Array.CreateInstance(componentType, events.Length);
                array.CopyTo(reduced, 0);
                array = reduced;
            }
            return array;
        }
    }
}
