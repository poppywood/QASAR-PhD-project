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

namespace com.espertech.esper.core
{
    /// <summary>Converts column results into a Map of key-value pairs. </summary>
    public class DeliveryConvertorMap : DeliveryConvertor
    {
        private readonly String[] columnNames;
    
        /// <summary>Ctor. </summary>
        /// <param name="columnNames">the names for columns</param>
        public DeliveryConvertorMap(String[] columnNames) {
            this.columnNames = columnNames;
        }
    
        public Object[] ConvertRow(Object[] columns) {
            Map<String, Object> map = new HashMap<String, Object>();
            for (int i = 0; i < columns.Length; i++)
            {
                map.Put(columnNames[i], columns[i]);
            }
            return new Object[] {map};
        }
    }
}
