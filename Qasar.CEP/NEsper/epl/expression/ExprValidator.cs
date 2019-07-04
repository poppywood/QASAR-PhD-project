using System;

using com.espertech.esper.epl.core;
using com.espertech.esper.epl.variable;
using com.espertech.esper.schedule;

namespace com.espertech.esper.epl.expression
{
	/// <summary>
    /// Validation interface for expression nodes.
    /// </summary>

    public interface ExprValidator
	{
        /// <summary>
        /// Returns the type that the node's evaluate method returns an instance of.
        /// </summary>
        /// <value>The type.</value>
        /// <returns> type returned when evaluated</returns>
        /// <throws>ExprValidationException thrown when validation failed </throws>

        Type ReturnType { get ; }

        /// <summary>
        /// Validate node.
        /// </summary>
        /// <param name="streamTypeService">serves stream event type info</param>
        /// <param name="methodResolutionService">for resolving class names in library method invocations</param>
        /// <param name="viewResourceDelegate">delegates for view resources to expression nodes</param>
        /// <param name="timeProvider">provides engine current time</param>
        /// <param name="variableService">provides access to variable values</param>
        /// <throws>ExprValidationException thrown when validation failed </throws>
	    void Validate(StreamTypeService streamTypeService,
                      MethodResolutionService methodResolutionService,
                      ViewResourceDelegate viewResourceDelegate,
                      TimeProvider timeProvider,
                      VariableService variableService);
	}
}
