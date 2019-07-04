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
    /// A result delivery strategy that uses an "update" method that accepts
    /// a pair of object array array.
    /// </summary>
    public class ResultDeliveryStrategyObjectArr : ResultDeliveryStrategy
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Object subscriber;
        private readonly FastMethod fastMethod;
    
        /// <summary>Ctor. </summary>
        /// <param name="subscriber">is the subscriber to deliver to</param>
        /// <param name="method">the method to invoke</param>
        public ResultDeliveryStrategyObjectArr(Object subscriber, MethodInfo method)
        {
            this.subscriber = subscriber;
            FastClass fastClass = FastClass.Create(subscriber.GetType());
            this.fastMethod = fastClass.GetMethod(method);
        }
    
        public void Execute(UniformPair<EventBean[]> result)
        {
            Object[][] newData = Convert(result.First);
            Object[][] oldData = Convert(result.Second);
    
            Object[] paramList = new Object[] {newData, oldData};
            try {
                fastMethod.Invoke(subscriber, paramList);
            } catch (TargetInvocationException e) {
                ResultDeliveryStrategyImpl.Handle(log, e, paramList, subscriber, fastMethod);
            }
        }
    
        private static Object[][] Convert(EventBean[] events)
        {
            if ((events == null) || (events.Length == 0))
            {
                return null;
            }
    
            Object[][] result = new Object[events.Length][];
            int length = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (events[i] is NaturalEventBean)
                {
                    NaturalEventBean natural = (NaturalEventBean) events[i];
                    result[length] = natural.Natural;
                    length++;
                }
            }
    
            if (length == 0)
            {
                return null;
            }
            if (length != events.Length)
            {
                Object[][] reduced = new Object[length][];
                Array.Copy(result, 0, reduced, 0, length);
                result = reduced;
            }
            return result;
        }
    }
}
