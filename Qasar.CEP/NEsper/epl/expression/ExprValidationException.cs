using System;
namespace com.espertech.esper.epl.expression
{

    /// <summary> Thrown to indicate a validation error in a filter expression.</summary>
    [Serializable]
    public class ExprValidationException : System.Exception
    {
        /// <summary> Ctor.</summary>
        /// <param name="message">validation error message
        /// </param>
        public ExprValidationException(String message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExprValidationException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="cause">The cause.</param>
        public ExprValidationException(String message, Exception cause)
            : base(message, cause)
        {
        }
    }
}