using System;

namespace com.espertech.esper.compat
{
    /// <summary>
    /// Allows for a class to override the name of a property.
    /// </summary>

    public class OverrideNameAttribute : Attribute
    {
        private string m_name;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverrideNameAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public OverrideNameAttribute( String name )
        {
            this.m_name = name;    
        }
    }
}
