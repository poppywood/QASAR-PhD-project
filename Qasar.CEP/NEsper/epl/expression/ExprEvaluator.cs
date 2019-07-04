using System;

using com.espertech.esper.events ;

namespace com.espertech.esper.epl.expression
{
    /// <summary>
    /// Interface for evaluating of an event tuple.
    /// </summary>

    public interface ExprEvaluator
    {
        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>

        Object Evaluate(EventBean[] eventsPerStream, bool isNewData);
    }

    public delegate Object ExprEvaluatorDelegate(EventBean[] eventsPerStream, bool isNewData);

    public sealed class ProxyExprEvaluator : ExprEvaluator
    {
        private readonly ExprEvaluatorDelegate m_dg;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProxyExprEvaluator"/> class.
        /// </summary>
        /// <param name="m_dg">The M_DG.</param>
        public ProxyExprEvaluator(ExprEvaluatorDelegate m_dg)
        {
            this.m_dg = m_dg;
        }

        /// <summary>
        /// Evaluate event tuple and return result.
        /// </summary>
        /// <param name="eventsPerStream">event tuple</param>
        /// <param name="isNewData">indicates whether we are dealing with new data (istream) or old data (rstream)</param>
        /// <returns>
        /// evaluation result, a bool value for OR/AND-type evalution nodes.
        /// </returns>

        public Object Evaluate(EventBean[] eventsPerStream, bool isNewData)
        {
            return m_dg.Invoke(eventsPerStream, isNewData);
        }
    }
}
