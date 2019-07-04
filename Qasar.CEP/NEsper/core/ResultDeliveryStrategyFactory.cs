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

using CGLib;

using com.espertech.esper.client;
using com.espertech.esper.util;

using DataMap = com.espertech.esper.compat.Map<string, object>;

namespace com.espertech.esper.core
{
    /// <summary>
    /// Factory for creating a dispatch strategy based on the subscriber object and the columns
    /// produced by a select-clause.
    /// </summary>
    public class ResultDeliveryStrategyFactory 
    {
        /// <summary>Creates a strategy implementation that indicates to subscribers the statement results based on the select-clause columns. </summary>
        /// <param name="subscriber">to indicate to</param>
        /// <param name="selectClauseTypes">are the types of each column in the select clause</param>
        /// <param name="selectClauseColumns">the names of each column in the select clause</param>
        /// <returns>strategy for dispatching naturals</returns>
        /// <throws>EPSubscriberException if the subscriber is invalid</throws>
        public static ResultDeliveryStrategy Create(Object subscriber,
                                                    Type[] selectClauseTypes,
                                                    String[] selectClauseColumns)
        {
            // Locate update methods
            MethodInfo subscriptionMethod = null;
            List<MethodInfo> updateMethods = new List<MethodInfo>();
            foreach (MethodInfo method in subscriber.GetType().GetMethods())
            {
                if ((method.Name == "Update") && (method.IsPublic))
                {
                    updateMethods.Add(method);
                }
            }
    
            // none found
            if (updateMethods.Count == 0)
            {
                String message = "Subscriber object does not provide a public method by name 'Update'";
                throw new EPSubscriberException(message);
            }
    
            // match to parameters
            bool isMapArrayDelivery = false;
            bool isObjectArrayDelivery = false;
            bool isSingleRowMap = false;
            bool isSingleRowObjectArr = false;
            bool isTypeArrayDelivery = false;
            foreach (MethodInfo method in updateMethods)
            {
                ParameterInfo[] parameters = method.GetParameters();
    
                if (parameters.Length == selectClauseTypes.Length)
                {
                    bool fitsParameters = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Type boxedExpressionType = TypeHelper.GetBoxedType(selectClauseTypes[i]);
                        Type boxedParameterType = TypeHelper.GetBoxedType(parameters[i].ParameterType);
                        if ((boxedExpressionType != null) && (!TypeHelper.IsAssignmentCompatible(boxedExpressionType, boxedParameterType)))
                        {
                            fitsParameters = false;
                            break;
                        }
                    }
                    if (fitsParameters)
                    {
                        subscriptionMethod = method;
                        break;
                    }
                }


                if (parameters.Length == 1) {
                    Type paramType0 = parameters[0].ParameterType;
                    if (paramType0 == typeof(DataMap))
                    {
                        isSingleRowMap = true;
                        subscriptionMethod = method;
                        break;
                    }
                    if (paramType0 == typeof (Object[])) {
                        isSingleRowObjectArr = true;
                        subscriptionMethod = method;
                        break;
                    }
                }
                else if (parameters.Length == 2) {
                    Type paramType0 = parameters[0].ParameterType;
                    Type paramType1 = parameters[1].ParameterType;
                    if ((paramType0 == typeof(DataMap[])) &&
                        (paramType1 == typeof (DataMap[]))) {
                        subscriptionMethod = method;
                        isMapArrayDelivery = true;
                        break;
                    }

                    if ((paramType0 == typeof (Object[][])) &&
                        (paramType1 == typeof (Object[][]))) {
                        subscriptionMethod = method;
                        isObjectArrayDelivery = true;
                        break;
                    }

                    // Handle uniform underlying or column type array dispatch
                    if ((paramType0.Equals(paramType1)) &&
                        (paramType0.IsArray) &&
                        (selectClauseTypes.Length == 1)) {
                        Type componentType = paramType0.GetElementType();
                        if (TypeHelper.IsAssignmentCompatible(selectClauseTypes[0], componentType)) {
                            subscriptionMethod = method;
                            isTypeArrayDelivery = true;
                            break;
                        }
                    }
                }
            }
    
            if (subscriptionMethod == null)
            {
                if (updateMethods.Count > 1)
                {
                    String parametersDesc = TypeHelper.GetParameterAsString(selectClauseTypes);
                    String message = "No suitable subscriber method named 'Update' found, expecting a method that takes " +
                            selectClauseTypes.Length + " parameter of type " + parametersDesc;
                    throw new EPSubscriberException(message);
                }
                else
                {
                    ParameterInfo[] parameters = updateMethods[0].GetParameters();
                    String parametersDesc = TypeHelper.GetParameterAsString(selectClauseTypes);
                    if (parameters.Length != selectClauseTypes.Length)
                    {
                        String message = "No suitable subscriber method named 'Update' found, expecting a method that takes " +
                                selectClauseTypes.Length + " parameter of type " + parametersDesc;
                        throw new EPSubscriberException(message);
                    }
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        Type boxedExpressionType = TypeHelper.GetBoxedType(selectClauseTypes[i]);
                        Type boxedParameterType = TypeHelper.GetBoxedType(parameters[i].ParameterType);
                        if ((boxedExpressionType != null) && (!TypeHelper.IsAssignmentCompatible(boxedExpressionType, boxedParameterType)))
                        {
                            String message = "Subscriber method named 'Update' for parameter number " + (i + 1) + " is not assignable, " +
                                    "expecting type '" + TypeHelper.GetParameterAsString(selectClauseTypes[i]) + "' but found type '"
                                    + TypeHelper.GetParameterAsString(parameters[i].ParameterType) + "'";
                            throw new EPSubscriberException(message);
                        }
                    }
                }
            }
    
            if (isMapArrayDelivery)
            {
                return new ResultDeliveryStrategyMap(subscriber, subscriptionMethod, selectClauseColumns);
            }
            else if (isObjectArrayDelivery)
            {
                return new ResultDeliveryStrategyObjectArr(subscriber, subscriptionMethod);
            }
            else if (isTypeArrayDelivery)
            {
                return new ResultDeliveryStrategyTypeArr(subscriber, subscriptionMethod);
            }
    
            // Try to find the "start", "end" and "updateRStream" methods
            MethodInfo startMethod;
            MethodInfo endMethod;
            MethodInfo rStreamMethod;

            Type subscriberType = subscriber.GetType();
            Type[] parameterList = FastBase.GetParameterTypes(subscriptionMethod);

            startMethod = subscriberType.GetMethod("UpdateStart", new Type[] {typeof (int), typeof (int)});
            endMethod = subscriberType.GetMethod("UpdateEnd", new Type[] { });
            rStreamMethod = subscriberType.GetMethod("UpdateRStream", parameterList);
    
            DeliveryConvertor convertor;
            if (isSingleRowMap)
            {
                convertor = new DeliveryConvertorMap(selectClauseColumns);
            }
            else if (isSingleRowObjectArr)
            {
                convertor = new DeliveryConvertorObjectArr();
            }
            else
            {
                convertor = new DeliveryConvertorNull();
            }
    
            return new ResultDeliveryStrategyImpl(subscriber, convertor, subscriptionMethod, startMethod, endMethod, rStreamMethod);
        }
    }
}
