///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Reflection;
using System.Text;
using com.espertech.esper.client;
using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;

using CGLib;

using log4net;

namespace com.espertech.esper.core
{
    /// <summary>
    /// A result delivery strategy that uses a matching "update" method and
    /// optional start, end, and updateRStream methods, to deliver column-wise
    /// to parameters of the update method.
    /// </summary>
    public class ResultDeliveryStrategyImpl : ResultDeliveryStrategy
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Object subscriber;
        private readonly FastMethod updateFastMethod;
        private readonly FastMethod startFastMethod;
        private readonly FastMethod endFastMethod;
        private readonly FastMethod updateRStreamFastMethod;
        private readonly DeliveryConvertor deliveryConvertor;

        /// <summary>Ctor. </summary>
        /// <param name="subscriber">is the subscriber receiving method invocations</param>
        /// <param name="deliveryConvertor">for converting individual rows</param>
        /// <param name="method">to deliver the insert stream to</param>
        /// <param name="startMethod">to call to indicate when delivery starts, or null if no such indication is required</param>
        /// <param name="endMethod">to call to indicate when delivery ends, or null if no such indication is required</param>
        /// <param name="rStreamMethod">to deliver the remove stream to, or null if no such indication is required</param>
        public ResultDeliveryStrategyImpl(Object subscriber,
                                          DeliveryConvertor deliveryConvertor,
                                          MethodInfo method,
                                          MethodInfo startMethod,
                                          MethodInfo endMethod,
                                          MethodInfo rStreamMethod)
        {
            this.subscriber = subscriber;
            this.deliveryConvertor = deliveryConvertor;
            FastClass fastClass = FastClass.Create(subscriber.GetType());
            this.updateFastMethod = fastClass.GetMethod(method);
            
            if (startMethod != null)
            {
                startFastMethod = fastClass.GetMethod(startMethod);
            }
            else
            {
                startFastMethod = null;
            }
    
            if (endMethod != null)
            {
                endFastMethod = fastClass.GetMethod(endMethod);
            }
            else
            {
                endFastMethod = null;
            }
    
            if (rStreamMethod != null)
            {
                updateRStreamFastMethod = fastClass.GetMethod(rStreamMethod);
            }
            else
            {
                updateRStreamFastMethod = null;
            }
        }

        /// <summary>
        /// Execute the dispatch.
        /// </summary>
        /// <param name="result">is the insert and remove stream to indicate</param>
        public void Execute(UniformPair<EventBean[]> result)
        {
            if (startFastMethod != null)
            {
                int countNew = Count(result.First);
                int countOld = Count(result.Second);
    
                Object[] paramList = new Object[] {countNew, countOld};
                try {
                    startFastMethod.Invoke(subscriber, paramList);
                }
                catch (TargetInvocationException e) {
                    Handle(log, e, paramList, subscriber, startFastMethod);
                }
            }
    
            EventBean[] newData = result.First;
            EventBean[] oldData = result.Second;
    
            if ((newData != null) && (newData.Length > 0)) {
                for (int i = 0; i < newData.Length; i++) {
                    EventBean @event = newData[i];
                    if (@event is NaturalEventBean) {
                        NaturalEventBean natural = (NaturalEventBean) @event;
                        Object[] paramList = deliveryConvertor.ConvertRow(natural.Natural);
                        try
                        {
                            updateFastMethod.Invoke(subscriber, paramList);
                        }
                        catch (TargetInvocationException e)
                        {
                            Handle(log, e, paramList, subscriber, updateFastMethod);
                        }
                        catch (Exception e)
                        {
                            Handle(log, e, paramList, subscriber, updateFastMethod);
                        }
                    }
                }
            }
    
            if ((updateRStreamFastMethod != null) && (oldData != null) && (oldData.Length > 0)) {
                for (int i = 0; i < oldData.Length; i++) {
                    EventBean @event = oldData[i];
                    if (@event is NaturalEventBean) {
                        NaturalEventBean natural = (NaturalEventBean) @event;
                        Object[] paramList = deliveryConvertor.ConvertRow(natural.Natural);
                        try {
                            updateRStreamFastMethod.Invoke(subscriber, paramList);
                        }
                        catch (TargetInvocationException e)
                        {
                            Handle(log, e, paramList, subscriber, updateRStreamFastMethod);
                        }
                        catch (Exception e)
                        {
                            Handle(log, e, paramList, subscriber, updateRStreamFastMethod);
                        }
                    }
                }
            }
    
            if (endFastMethod != null) {
                try {
                    endFastMethod.Invoke(subscriber, null);
                }
                catch (TargetInvocationException e)
                {
                    Handle(log, e, null, subscriber, endFastMethod);
                }
                catch (Exception e)
                {
                    Handle(log, e, null, subscriber, endFastMethod);
                }
            }
        }
    
        /// <summary>Handle the exception, displaying a nice message and converting to <see cref="EPException"/>. </summary>
        /// <param name="logger">is the logger to use for error logging</param>
        /// <param name="e">is the exception</param>
        /// <param name="paramList">the method parameters</param>
        /// <param name="subscriber">the object to deliver to</param>
        /// <param name="method">the method to call</param>
        /// <throws>EPException converted from the passed invocation exception</throws>
        protected internal static void Handle(ILog logger, Exception e, Object[] paramList, Object subscriber, FastMethod method)
        {
            Exception realException = e is TargetInvocationException ? e.InnerException : e;

            StringBuilder message = new StringBuilder();
            message.AppendFormat("Invocation exception when invoking method '{0}'", method.Name);
            message.AppendFormat(" on subscriber class '{0}'", subscriber.GetType().Name);
            message.AppendFormat(" for parameters {0}",
                                 (paramList == null) ? "null" : CollectionHelper.Render(paramList));
            message.AppendFormat(" : " + realException.GetType().Name + " : " + realException.Message);

            logger.Error(message, realException);
            throw new EPException(message.ToString(), realException);
        }
    
        private static int Count(EventBean[] events) {
            if (events == null)
            {
                return 0;
            }
            int count = 0;
            for (int i = 0; i < events.Length; i++)
            {
                EventBean @event = events[i];
                if (@event is NaturalEventBean)
                {
                    count++;
                }
            }
            return count;
        }
    }
}
