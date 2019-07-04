///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;

namespace net.esper.support.util
{
    [Serializable]
	public class SupportSerializableBean
	{
	    private readonly String @string;

	    public SupportSerializableBean(String @string)
	    {
	        this.@string = @string;
	    }

	    public String String
	    {
	        get { return @string; }
	    }
	}
} // End of namespace
