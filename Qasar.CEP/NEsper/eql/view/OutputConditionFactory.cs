using net.esper.eql.spec;
using net.esper.core;

namespace net.esper.eql.view
{
	/// <summary>
    /// Factory for output condition instances.
    /// </summary>

    public interface OutputConditionFactory
	{
	    /// <summary>
	    /// Creates an output condition instance.
	    /// </summary>
	    /// <param name="outputLimitSpec">specifies what kind of condition to create</param>
	    /// <param name="statementContext">supplies the services required such as for scheduling callbacks</param>
	    /// <param name="outputCallback">is the method to invoke for output</param>
	    /// <returns>instance for performing output</returns>
	    OutputCondition CreateCondition( OutputLimitSpec outputLimitSpec,
	                                     StatementContext statementContext,
	                                     OutputCallback outputCallback);
	}
}
