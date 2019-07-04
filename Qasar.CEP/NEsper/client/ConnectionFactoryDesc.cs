using System;
using System.Data.Common;
using System.Configuration;
using System.Collections;
using System.Collections.Specialized;
using net.esper.eql.db;

namespace net.esper.client
{
    /// <summary>
	/// Marker for different connection factory settings.
	/// </summary>
    
	public interface ConnectionFactoryDesc
    {
    }

    /// <summary>
    /// Connection factory settings for using a DbProviderFactory.
    /// </summary>

    public class DbProviderFactoryConnection : ConnectionFactoryDesc
    {
        private readonly ConnectionStringSettings settings ;

        /// <summary>
        /// Gets the connection settings.
        /// </summary>
        
        public virtual ConnectionStringSettings Settings
        {
            get { return settings; }
        }

        /// <summary>
        /// Database parameter model specification.
        /// </summary>

        private readonly ParameterModel parameterModel;

        /// <summary>
        /// Gets the parameter model.
        /// </summary>
        /// <value>The parameter model.</value>
        public virtual ParameterModel ParameterModel
        {
            get { return parameterModel; }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="providerName">The name of the provider.</param>
        /// <param name="properties">Properties that should be applied to the connection.</param>

        public DbProviderFactoryConnection(String providerName, NameValueCollection properties, ParameterModel parameterModel)
        {
            String name = null;

            DbProviderFactory dbProviderFactory = DbProviderFactories.GetFactory(providerName);
            DbConnectionStringBuilder builder = dbProviderFactory.CreateConnectionStringBuilder();

            this.parameterModel = parameterModel;

            foreach (String key in properties)
            {
                String value = properties[key];
                if (key.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    name = value;
                }
                else
                {
                    builder.Add(key, value);
                }
            }

            this.settings = new ConnectionStringSettings();
            this.settings.ProviderName = providerName;
            this.settings.ConnectionString = builder.ConnectionString;

            if (name != null)
            {
                this.settings.Name = name;
            }
        }

        /// <summary>
        /// Ctor.
        /// </summary>
        /// <param name="settings">The settings.</param>
        
        public DbProviderFactoryConnection(ConnectionStringSettings settings, ParameterModel parameterModel)
        {
            this.settings = settings;
            this.parameterModel = parameterModel;
        }
    }
}
