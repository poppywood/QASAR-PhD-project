///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Reflection;

using com.espertech.esper.collection;
using com.espertech.esper.compat;
using com.espertech.esper.events;

using CGLib;

using log4net;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.core
{
    /// <summary>A result delivery strategy that uses an "update" method that accepts a pair of map array. </summary>
    public class ResultDeliveryStrategyMap : ResultDeliveryStrategy
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly Object subscriber;
        private readonly FastMethod fastMethod;
        private readonly String[] columnNames;
    
        /// <summary>Ctor. </summary>
        /// <param name="subscriber">the object to deliver to</param>
        /// <param name="method">the delivery method</param>
        /// <param name="columnNames">the column names for the map</param>
        public ResultDeliveryStrategyMap(Object subscriber, MethodInfo method, String[] columnNames)
        {
            this.subscriber = subscriber;
            FastClass fastClass = FastClass.Create(subscriber.GetType());
            this.fastMethod = fastClass.GetMethod(method);
            this.columnNames = columnNames;
        }
    
        public void Execute(UniformPair<EventBean[]> result)
        {
            DataMap[] newData = Convert(result.First);
            DataMap[] oldData = Convert(result.Second);
    
            Object[] paramList = new Object[] {newData, oldData};
            try {
                fastMethod.Invoke(subscriber, paramList);
            }
            catch (TargetInvocationException e) {
                ResultDeliveryStrategyImpl.Handle(log, e, paramList, subscriber, fastMethod);
            }
        }

        private DataMap[] Convert(EventBean[] events)
        {
            if ((events == null) || (events.Length == 0))
            {
                return null;
            }

            DataMap[] result = new DataMap[events.Length];
            int length = 0;
            for (int i = 0; i < result.Length; i++)
            {
                if (events[i] is NaturalEventBean)
                {
                    NaturalEventBean natural = (NaturalEventBean) events[i];
                    result[length] = Convert(natural);
                    length++;
                }
            }
    
            if (length == 0)
            {
                return null;
            }
            if (length != events.Length)
            {
                DataMap[] reduced = new DataMap[length];
                Array.Copy(result, 0, reduced, 0, length);
                result = reduced;
            }
            return result;
        }

        private DataMap Convert(NaturalEventBean natural)
        {
            DataMap map = new HashMap<String, Object>();
            Object[] columns = natural.Natural;
            for (int i = 0; i < columns.Length; i++)
            {
                map.Put(columnNames[i], columns[i]);
            }
            return map;
        }
    }
}
