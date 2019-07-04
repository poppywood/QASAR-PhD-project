using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using com.espertech.esper.client;
using com.espertech.esper.events;

namespace com.espertech.esper.events.property
{
    /// <summary>
    /// Introspector that considers explicitly configured event properties only.
    /// </summary>

    public class PropertyListBuilderExplicit : PropertyListBuilder
    {
        private readonly ConfigurationEventTypeLegacy legacyConfig;

        /// <summary> Ctor.</summary>
        /// <param name="legacyConfig">is a legacy type specification containing
        /// information about explicitly configured fields and methods
        /// </param>

        public PropertyListBuilderExplicit(ConfigurationEventTypeLegacy legacyConfig)
        {
            if (legacyConfig == null)
            {
                throw new ArgumentException("Required configuration not passed");
            }

            this.legacyConfig = legacyConfig;
        }

        /// <summary>
        /// Introspect the type and deterime exposed event properties.
        /// </summary>
        /// <param name="type">type to introspect</param>
        /// <returns>list of event property descriptors</returns>

        public IList<EventPropertyDescriptor> AssessProperties(Type type)
        {
            IList<EventPropertyDescriptor> result = new List<EventPropertyDescriptor>();
            GetExplicitProperties(result, type, legacyConfig);
            return result;
        }

        /// <summary>
        /// Populates explicitly-defined properties into the result list.
        /// </summary>
        /// <param name="result">is the resulting list of event property descriptors</param>
        /// <param name="type">is the class to introspect</param>
        /// <param name="legacyConfig">supplies specification of explicit methods and fields to expose</param>

        internal static void GetExplicitProperties(
            IList<EventPropertyDescriptor> result,
            Type type,
            ConfigurationEventTypeLegacy legacyConfig)
        {
            foreach (ConfigurationEventTypeLegacy.LegacyFieldPropDesc desc in legacyConfig.FieldProperties)
            {
                result.Add(MakeDesc(type, desc));
            }

            foreach (ConfigurationEventTypeLegacy.LegacyMethodPropDesc desc in legacyConfig.MethodProperties)
            {
                result.Add(MakeDesc(type, desc));
            }
        }

        private static EventPropertyDescriptor MakeDesc(Type type, ConfigurationEventTypeLegacy.LegacyMethodPropDesc methodDesc)
        {
            MethodInfo[] methods = type.GetMethods();
            MethodInfo method = null;

            foreach (MethodInfo _mi in methods)
            {
                if (!_mi.Name.Equals(methodDesc.AccessorMethodName))
                {
                    continue;
                }
                if (_mi.ReturnType == typeof(void))
                {
                    continue;
                }
                if (_mi.GetParameters().Length >= 2)
                {
                    continue;
                }
                if (_mi.GetParameters().Length == 0)
                {
                    method = _mi;
                    break;
                }

                Type parameterType = _mi.GetParameters()[0].ParameterType;
                if ((parameterType != typeof(int)) &&
                    (parameterType != typeof(string)))
                {
                    continue;
                }

                method = _mi;
                break;
            }

            if (method == null)
            {
                throw new ConfigurationException(
                    "Configured method named '" + methodDesc.AccessorMethodName +
                    "' not found for class " + type.FullName);
            }

            return MakeMethodDesc(method, methodDesc.Name);
        }

        /// <summary>
        /// Creates an event property descriptor for a field or native property
        /// </summary>
        /// <param name="type"></param>
        /// <param name="fieldDesc"></param>
        /// <returns></returns>

        private static EventPropertyDescriptor MakeDesc(Type type, ConfigurationEventTypeLegacy.LegacyFieldPropDesc fieldDesc)
        {
            FieldInfo field = type.GetField(fieldDesc.AccessorFieldName);
            if (field != null)
            {
                return MakeFieldDesc(field, fieldDesc.Name);
            }

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(type);
            PropertyDescriptor property = properties[fieldDesc.AccessorFieldName];
            if (property != null)
            {
                return MakeNativeDesc(property, fieldDesc.Name);
            }

            throw new ConfigurationException(
                "Configured field named '" + fieldDesc.AccessorFieldName +
                "' not found for class " + type.FullName);
        }

        /// <summary>
        /// Makes a simple-type event property descriptor based on a reflected field.
        /// </summary>
        /// <param name="field">is the public field</param>
        /// <param name="name">is the name of the event property</param>
        /// <returns> property descriptor</returns>

        internal static EventPropertyDescriptor MakeFieldDesc(FieldInfo field, String name)
        {
            PropertyDescriptor descriptor = PropertyResolver.Current.GetPropertyFor(field);
            //PropertyDescriptor descriptor = new SimpleFieldPropertyDescriptor(field.Name, field);
            return new EventPropertyDescriptor(name, name, descriptor, EventPropertyType.SIMPLE);
        }

        /// <summary>
        /// Makes a simple-type event property descriptor based on a reflected property.
        /// </summary>
        /// <param name="property">the property descriptor</param>
        /// <param name="name">the name of the event property</param>
        /// <returns> property descriptor</returns>

        internal static EventPropertyDescriptor MakeNativeDesc(PropertyDescriptor property, String name)
        {
            return new EventPropertyDescriptor(name, name, property, EventPropertyType.SIMPLE);
        }

        /// <summary>
        /// Makes an event property descriptor based on a reflected method, considering
        /// the methods parameters to determine if this is an indexed or mapped event property.
        /// </summary>
        /// <param name="method">is the public method</param>
        /// <param name="name">is the name of the event property</param>
        /// <returns> property descriptor</returns>

        internal static EventPropertyDescriptor MakeMethodDesc(MethodInfo method, String name)
        {
            EventPropertyType propertyType;
            PropertyDescriptor descriptor;

            ParameterInfo[] methodParameters = method.GetParameters();
            switch (methodParameters.Length)
            {
                case 0:
                    descriptor = new FastAccessorPropertyDescriptor(name, method);
                    propertyType = EventPropertyType.SIMPLE;
                    break;
                case 1:
                    Type parameterType = methodParameters[0].ParameterType;
                    descriptor = new IndexedAccessorPropertyDescriptor(name, method);
                    if (parameterType == typeof(string))
                    {
                        propertyType = EventPropertyType.MAPPED;
                    }
                    else if (parameterType == typeof(int))
                    {
                        propertyType = EventPropertyType.INDEXED;
                    }
                    else
                    {
                        throw new ArgumentException("Method parameter was not of type string or int");
                    }
                    break;
                default:
                    throw new ArgumentException("Method must have zero or one parameter");
            }

            return new EventPropertyDescriptor(name, name, descriptor, propertyType);
        }
    }
}
