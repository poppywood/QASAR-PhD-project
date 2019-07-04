using System;
using System.Collections.Generic;
using System.Data.Common;

using com.espertech.esper.client;
using com.espertech.esper.compat;
using com.espertech.esper.util;

namespace com.espertech.esper.epl.db
{
    /// <summary>
    /// Database driver semantics are captured in the DbDriver.  Each
    /// driver instance is completely separate from other instances.
    /// Drivers encapsulate management of the connection, so specific
    /// properties are given to it so that it can build its connection
    /// string.
    /// </summary>

    public interface DbDriver
    {
        /// <summary>
        /// Gets the default meta origin policy.
        /// </summary>
        /// <value>The default meta origin policy.</value>
        ConfigurationDBRef.MetadataOriginEnum DefaultMetaOriginPolicy { get; }

        /// <summary>
        /// Creates a database driver command from a collection of fragments.
        /// </summary>
        /// <param name="sqlFragments">The SQL fragments.</param>
        /// <param name="metadataSettings">The metadata settings.</param>
        /// <returns></returns>
        DbDriverCommand CreateCommand(IEnumerable<PlaceholderParser.Fragment> sqlFragments, ColumnSettings metadataSettings);

        /// <summary>
        /// Gets or sets the properties for the driver.
        /// </summary>
        /// <value>The properties.</value>
        Properties Properties { get; set; }

        /// <summary>
        /// Gets the connection string associated with this driver.
        /// </summary>
        String ConnectionString { get; }

        /// <summary>
        /// Creates a database connection; this should be used sparingly if possible.
        /// </summary>
        /// <returns></returns>

        DbConnection CreateConnection();
    }
}
