using System;

namespace com.espertech.esper.type
{
	/// <summary> Enum for the type of outer join.</summary>
    public enum OuterJoinType
    {
	    /// <summary> Left outer join.</summary>
	    LEFT,
        /// <summary> Right outer join.</summary>
        RIGHT,
        /// <summary> Full outer join.</summary>
        FULL
    }

    public class OuterJoinTypeHelper
    {
        public static String GetText(OuterJoinType joinType)
        {
            switch (joinType)
            {
                case OuterJoinType.LEFT:
                    return "left";
                case OuterJoinType.RIGHT:
                    return "right";
                case OuterJoinType.FULL:
                    return "full";
                default:
                    throw new ArgumentException("Unknown joinType " + joinType);
            }
        }
    }
}