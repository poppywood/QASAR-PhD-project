using System;
using System.Configuration;

namespace net.esper.client
{
    public enum PropertyResolutionStyle
    {
        /// <summary>
        /// Properties are only matched if the names are identical in name
        /// and case to the original property name.
        /// </summary>

        CASE_SENSITIVE,

        /// <summary>
        /// Properties are matched if the names are identical.  A case insensitive
        /// search is used and will choose the first property that matches
        /// the name exactly or the first property that matches case insensitively
        /// should no match be found.
        /// </summary>

        CASE_INSENSITIVE,

        /// <summary>
        /// Properties are matched if the names are identical.  A case insensitive
        /// search is used and will choose the first property that matches
        /// the name exactly case insensitively.  If more than one 'name' can be
        /// mapped to the property an exception is thrown.
        /// </summary>
        
        DISTINCT_CASE_INSENSITIVE
    }

    /// <summary>
    /// A class that helps with the use of the PropertyResolutionStyle.  Among other
    /// things it allows developers to get or set the property resolution style that 
    /// should be used when one is not specified.
    /// </summary>

    public class PropertyResolutionStyleHelper
    {
        private static PropertyResolutionStyle? defaultPropertyResolutionStyle;

        /// <summary>
        /// Initializes the <see cref="PropertyResolutionStyleHelper"/> class.
        /// </summary>
        static PropertyResolutionStyleHelper()
        {
            // Default setting is consistent with the way that esper works
            defaultPropertyResolutionStyle = PropertyResolutionStyle.CASE_SENSITIVE;
            // Check with configuration management to see if the user has specified a
            // setting that overrides the default behavior.
            string styleSetting = ConfigurationManager.AppSettings["PropertyResolutionStyle"];
            if ( styleSetting != null )
            {
                defaultPropertyResolutionStyle = (PropertyResolutionStyle) Enum.Parse(typeof (PropertyResolutionStyle), styleSetting, true);
            }
        }

        /// <summary>
        /// Gets or sets the property resolution style that should be used whe
        /// one is not specified.
        /// </summary>

        public static PropertyResolutionStyle DefaultPropertyResolutionStyle
        {
            get { return defaultPropertyResolutionStyle.GetValueOrDefault(PropertyResolutionStyle.CASE_SENSITIVE); }
            set { defaultPropertyResolutionStyle = value; }
        }
    }
}