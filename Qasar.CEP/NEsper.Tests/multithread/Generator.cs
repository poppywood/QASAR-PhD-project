// ---------------------------------------------------------------------------------- /
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
// ---------------------------------------------------------------------------------- /

using System;
using System.Collections.Generic;

using net.esper.events;
using net.esper.support.bean;

namespace net.esper.multithread
{
	public class Generator
	{
        public static List<SupportBean> CreateEventList( int maxNumEvents )
        {
            List<SupportBean> beanList = new List<SupportBean>();

            for (int ii = 0; ii < maxNumEvents; ii++)
            {
                SupportBean bean = new SupportBean(Convert.ToString(ii), ii);
                beanList.Add(bean);
            }

            return beanList;
        }

	    public static IEnumerator<Object> Create( int maxNumEvents )
	    {
	        return Create(CreateEventList(maxNumEvents));
	    }

        public static IEnumerator<Object> Create(List<SupportBean> beanList)
        {
            foreach( SupportBean bean in beanList )
            {
                yield return bean;
            }
        }
    }
} // End of namespace
