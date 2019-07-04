using System;
using System.Data.Common;

using com.espertech.esper.client;

using com.espertech.esper.epl.db.drivers;

namespace com.espertech.esper.epl.db
{
	/// <summary>
	///  Factory for new database connections.
	/// </summary>
	
	public interface DatabaseConnectionFactory
	{
        /// <summary>
        /// Gets the database driver.
        /// </summary>

        DbDriver Driver { get; }
	}
}
