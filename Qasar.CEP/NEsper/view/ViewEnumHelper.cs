///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using net.esper.eql.spec;

namespace net.esper.view
{
    /// <summary>Helper producing a repository of built-in views.</summary>
	public class ViewEnumHelper
	{
	    private readonly static PluggableObjectCollection builtinViews;

	    static ViewEnumHelper()
	    {
	        builtinViews = new PluggableObjectCollection();
	        foreach (ViewEnum viewEnum in ViewEnum.Values)
	        {
	            builtinViews.AddObject(viewEnum.Namespace, viewEnum.Name, viewEnum.FactoryType, PluggableObjectType.VIEW);
	        }
	    }

	    /// <summary>Returns a collection of plug-in views.</summary>
	    /// <returns>built-in view definitions</returns>
	    public static PluggableObjectCollection BuiltinViews
	    {
            get { return builtinViews; }
	    }
	}
} // End of namespace
