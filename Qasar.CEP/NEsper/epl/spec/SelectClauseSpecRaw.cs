///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2006-2008 Esper Team. All rights reserved.                           /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System.Collections.Generic;

using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
    /// <summary>Encapsulates the parsed select expressions in a select-clause in an EPL statement. </summary>
    public class SelectClauseSpecRaw : MetaDefItem
    {
    	private readonly List<SelectClauseElementRaw> selectClauseElements;
    
        /// <summary>Ctor. </summary>
        public SelectClauseSpecRaw()
    	{
    		selectClauseElements = new List<SelectClauseElementRaw>();
        }
    
        /// <summary>Adds an select expression within the select clause. </summary>
        /// <param name="element">is the expression to add</param>
        public void Add(SelectClauseElementRaw element)
    	{
    		selectClauseElements.Add(element);
    	}

        /// <summary>Returns the list of select expressions. </summary>
        /// <returns>list of expressions</returns>
        public List<SelectClauseElementRaw> SelectExprList
        {
            get { return selectClauseElements; }
        }

        /// <summary>Returns true if the select clause contains at least one wildcard. </summary>
        /// <returns>true if clause contains wildcard, false if not</returns>
        public bool IsUsingWildcard
        {
            get
            {
                foreach (SelectClauseElementRaw element in selectClauseElements) {
                    if (element is SelectClauseElementWildcard) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
