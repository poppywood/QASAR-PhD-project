///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using net.esper.eql.spec;
using net.esper.pattern.guard;
using net.esper.pattern.observer;

namespace net.esper.pattern
{
    /// <summary>
    /// Helper producing a repository of built-in pattern objects.
    /// </summary>
	public class PatternObjectHelper
	{
	    private readonly static PluggableObjectCollection builtinPatternObjects;

	    static PatternObjectHelper()
	    {
	        builtinPatternObjects = new PluggableObjectCollection();
	        foreach (GuardEnum guardEnum in GuardEnum.Values)
	        {
	            builtinPatternObjects.AddObject(guardEnum.Namespace, guardEnum.Name, guardEnum.Clazz, PluggableObjectType.PATTERN_GUARD);
	        }

            foreach (ObserverEnum observerEnum in ObserverEnum.Values)
	        {
	            builtinPatternObjects.AddObject(observerEnum.Namespace, observerEnum.Name, observerEnum.Type, PluggableObjectType.PATTERN_OBSERVER);
	        }
	    }

	    /// <summary>Returns the built-in pattern objects.</summary>
	    /// <returns>collection of built-in pattern objects.</returns>
        public static PluggableObjectCollection BuiltinPatternObjects
        {
            get { return builtinPatternObjects; }
        }
	}
} // End of namespace
