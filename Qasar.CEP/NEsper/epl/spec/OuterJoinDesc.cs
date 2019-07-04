using System;

using com.espertech.esper.type;
using com.espertech.esper.epl.expression;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.spec
{
	/// <summary>
	/// Contains the ON-clause criteria in an outer join.
	/// </summary>
	public class OuterJoinDesc : MetaDefItem
	{
		/// <summary> Returns the type of outer join (left/right/full).</summary>
		/// <returns> outer join type
		/// </returns>
		public OuterJoinType OuterJoinType
		{
            get { return outerJoinType; }
		}
		/// <summary> Returns left hand identifier node.</summary>
		/// <returns> left hand
		/// </returns>
		public ExprIdentNode LeftNode
		{
            get { return leftNode; }
		}
		/// <summary> Returns right hand identifier node.</summary>
		/// <returns> right hand
		/// </returns>
		public ExprIdentNode RightNode
		{
            get { return rightNode; }
		}
        
        /// <summary>Returns additional properties in the on-clause, if any, that are connected via logical-and</summary>
        /// <returns>additional properties</returns>
        public ExprIdentNode[] AdditionalLeftNodes
        {
            get {return addLeftNode;}
        }

        /// <summary>Returns additional properties in the on-clause, if any, that are connected via logical-and</summary>
        /// <returns>additional properties</returns>
        public ExprIdentNode[] AdditionalRightNodes
        {
            get {return addRightNode;}
        }

		private readonly OuterJoinType outerJoinType;
        private readonly ExprIdentNode leftNode;
        private readonly ExprIdentNode rightNode;

        private readonly ExprIdentNode[] addLeftNode;
        private readonly ExprIdentNode[] addRightNode;

        /// <summary>Ctor.</summary>
        /// <param name="outerJoinType">type of the outer join</param>
        /// <param name="leftNode">left hand identifier node</param>
        /// <param name="rightNode">right hand identifier node</param>
        /// <param name="addLeftNode">additional optional left hand identifier nodes for the on-clause in a logical-and</param>
        /// <param name="addRightNode">additional optional right hand identifier nodes for the on-clause in a logical-and</param>
        public OuterJoinDesc(OuterJoinType outerJoinType, ExprIdentNode leftNode, ExprIdentNode rightNode, ExprIdentNode[] addLeftNode, ExprIdentNode[] addRightNode)
        {
            this.outerJoinType = outerJoinType;
            this.leftNode = leftNode;
            this.rightNode = rightNode;
            this.addLeftNode = addLeftNode;
            this.addRightNode = addRightNode;
        }
	}
}
