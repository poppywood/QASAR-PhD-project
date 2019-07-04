using System;

namespace com.espertech.esper.core
{
	public class DirectoryException : Exception
	{
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryException"/> class.
        /// </summary>
		public DirectoryException() : base() { }
        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryException"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
		public DirectoryException( string message ) : base( message ) { }

	}
}
