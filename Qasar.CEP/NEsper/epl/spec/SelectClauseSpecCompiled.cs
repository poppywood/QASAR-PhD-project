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
    /// <summary>
    /// Encapsulates the parsed select expressions in a select-clause in an EPL statement.
    /// </summary>
    public class SelectClauseSpecCompiled : MetaDefItem
    {
    	private readonly IList<SelectClauseElementCompiled> selectClauseElements;
    
        /// <summary>Ctor. </summary>
        public SelectClauseSpecCompiled()
    	{
    		selectClauseElements = new List<SelectClauseElementCompiled>();
        }
    
        /// <summary>Ctor. </summary>
        /// <param name="selectList">for a populates list of select expressions</param>
        public SelectClauseSpecCompiled(IList<SelectClauseElementCompiled> selectList)
    	{
            this.selectClauseElements = selectList;
    	}
    
        /// <summary>Adds an select expression within the select clause. </summary>
        /// <param name="element">is the expression to add</param>
        public void Add(SelectClauseElementCompiled element)
    	{
    		selectClauseElements.Add(element);
    	}
    
        /// <summary>Returns the list of select expressions. </summary>
        /// <returns>list of expressions</returns>
        public IList<SelectClauseElementCompiled> SelectExprList
    	{
            get { return selectClauseElements; }
    	}
    
        /// <summary>Returns true if the select clause contains at least one wildcard. </summary>
        /// <returns>true if clause contains wildcard, false if not</returns>
        public bool IsUsingWildcard
        {
            get
            {
                foreach (SelectClauseElementCompiled element in selectClauseElements) {
                    if (element is SelectClauseElementWildcard) {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
