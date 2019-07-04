///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.IO;
using System.Reflection;

using CGLib;

using com.espertech.esper.compat;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Base class for getters for a dynamic property (syntax field.inner?), caches methods
    /// to use for classes.
    /// </summary>
	public abstract class DynamicPropertyGetterBase : EventPropertyGetter
	{
	    private readonly CopyOnWriteList<DynamicPropertyDescriptor> cache;

	    /// <summary>
	    /// To be implemented to return the method required, or null to indicate an appropriate method could not be found.
	    /// </summary>
	    /// <param name="clazz">to search for a matching method</param>
	    /// <returns>method if found, or null if no matching method exists</returns>
        protected abstract ValueGetter DetermineGetter(Type clazz);

	    /// <summary>
	    /// Call the getter to obtains the return result object, or null if no such method exists.
	    /// </summary>
	    /// <param name="descriptor">provides method information for the class</param>
	    /// <param name="underlying">
	    /// is the underlying object to ask for the property value
	    /// </param>
	    /// <returns>underlying</returns>
        protected virtual Object Call(DynamicPropertyDescriptor descriptor, Object underlying)
        {
            try
            {
                return descriptor.Getter.Invoke(underlying);
            }
            catch (InvalidCastException)
            {
                throw new PropertyAccessException("Mismatched getter instance to event bean type");
            }
            catch (TargetInvocationException e)
            {
                throw new PropertyAccessException(e);
            }
            catch (ArgumentException e)
            {
                throw new PropertyAccessException(e);
            }
        }

	    /// <summary>Ctor.</summary>
	    public DynamicPropertyGetterBase()
	    {
	        cache = new CopyOnWriteList<DynamicPropertyDescriptor>();
	    }

	    public Object GetValue(EventBean obj)
	    {
	        DynamicPropertyDescriptor desc = GetPopulateCache(obj);
	        if (desc.Getter == null)
	        {
	            return null;
	        }
	        return Call(desc, obj.Underlying);
	    }

	    public virtual bool IsExistsProperty(EventBean eventBean)
	    {
	        DynamicPropertyDescriptor desc = GetPopulateCache(eventBean);
	        return desc.Getter != null;
	    }

	    private DynamicPropertyDescriptor GetPopulateCache(EventBean obj)
	    {
	        // Check if the method is already there
	        Type target = obj.Underlying.GetType();
	        foreach (DynamicPropertyDescriptor desc in cache)
	        {
	            if (desc.Clazz == target)
	            {
	                return desc;
	            }
	        }

	        // need to add it
	        lock(this)
	        {
	            foreach (DynamicPropertyDescriptor desc in cache)
	            {
	                if (desc.Clazz == target)
	                {
	                    return desc;
	                }
	            }

	            // Lookup method to use
                ValueGetter getter = DetermineGetter(target);

	            // Cache descriptor and create fast method
	            DynamicPropertyDescriptor propertyDescriptor;
                if (getter == null)
	            {
	                propertyDescriptor = new DynamicPropertyDescriptor(target, null);
	            }
	            else
	            {
	                propertyDescriptor = new DynamicPropertyDescriptor(target, getter);
	            }
	            cache.Add(propertyDescriptor);
	            return propertyDescriptor;
	        }
	    }

        /// <summary>
        /// Gets the correct name of a method that matches a property.  For
        /// example, if the caller supplied 'test' for the property, then the
        /// method would return the transformed name of 'GetTest'.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static String GetGetterMethodName(String propertyName)
        {
            StringWriter writer = new StringWriter();
            writer.Write("Get");
            writer.Write(Char.ToUpper(propertyName[0]));
            writer.Write(propertyName.Substring(1));
            return writer.ToString();
        }

        /// <summary>
        /// Gets the correct canonized form of a property name.    For
        /// example, if the caller supplied 'test' for the property, then the
        /// method would return the transformed name of 'Test'.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public static String GetPropertyName(String propertyName)
        {
            StringWriter writer = new StringWriter();
            writer.Write(Char.ToUpper(propertyName[0]));
            writer.Write(propertyName.Substring(1));
            return writer.ToString();
        }
    }
} // End of namespace
