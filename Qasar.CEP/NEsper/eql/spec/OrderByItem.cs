///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using net.esper.eql.expression;
using net.esper.util;

namespace net.esper.eql.spec
{
    /// <summary>
    /// Specification object to an element in the order-by expression.
    /// </summary>
    public class OrderByItem : MetaDefItem
    {
        private readonly ExprNode exprNode;
        private readonly bool isDescending;

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="exprNode">is the order-by expression node</param>
        /// <param name="ascending">is true for ascending, or false for descending sort</param>
        public OrderByItem(ExprNode exprNode, bool ascending)
        {
            this.exprNode = exprNode;
            isDescending = ascending;
        }

        /// <summary>
        /// Returns the order-by expression node.
        /// </summary>
        /// <value>The expr node.</value>
        /// <returns>expression node.</returns>
        public ExprNode ExprNode
        {
            get { return exprNode; }
        }

        /// <summary>
        /// Returns true for ascending, false for descending.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is descending; otherwise, <c>false</c>.
        /// </value>
        /// <returns>indicator if ascending or descending</returns>
        public bool IsDescending
        {
            get { return isDescending; }
        }
    }
}
