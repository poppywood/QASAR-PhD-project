///////////////////////////////////////////////////////////////////////////////////////
// Copyright (C) 2007 Esper Team. All rights reserved.                                /
// http://esper.codehaus.org                                                          /
// ---------------------------------------------------------------------------------- /
// The software in this package is published under the terms of the GPL license       /
// a copy of which has been included with this distribution in the license.txt file.  /
///////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;

using net.esper.client;
using net.esper.collection;
using net.esper.compat;
using net.esper.util;

namespace net.esper.eql.spec
{
    /// <summary>
    /// Repository for pluggable objects of different types that follow a "namespace:name" notation.
    /// </summary>
    public class PluggableObjectCollection
    {
        /// <summary>
        /// Map of namespace, name and class plus type
        /// </summary>

        private readonly EDictionary<String, EDictionary<String, Pair<Type, PluggableObjectType>>> pluggables;

        /// <summary>
        /// Initializes a new instance of the <see cref="PluggableObjectCollection"/> class.
        /// </summary>
        public PluggableObjectCollection()
        {
            pluggables = new HashDictionary<String, EDictionary<String, Pair<Type, PluggableObjectType>>>();
        }

        /// <summary>
        /// Add a plug-in view.
        /// </summary>
        /// <param name="configurationPlugInViews">a list of configured plug-in view objects.</param>
        public void AddViews(IEnumerable<ConfigurationPlugInView> configurationPlugInViews)
        {
            InitViews(configurationPlugInViews);
        }

        /// <summary>
        /// Add a plug-in pattern object.
        /// </summary>
        /// <param name="configPattern">a list of configured plug-in pattern objects.</param>
        public void AddPatternObjects(IEnumerable<ConfigurationPlugInPatternObject> configPattern)
        {
            InitPatterns(configPattern);
        }

        /// <summary>
        /// Add a plug-in objects for another collection.
        /// </summary>
        /// <param name="other">the collection to add</param>
        public void AddObjects(PluggableObjectCollection other)
        {
            foreach (
                KeyValuePair<String, EDictionary<String, Pair<Type, PluggableObjectType>>> entry in other.Pluggables)
            {
                EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap = pluggables.Get(entry.Key);
                if (namespaceMap == null)
                {
                    namespaceMap = new HashDictionary<String, Pair<Type, PluggableObjectType>>();
                    pluggables[entry.Key] = namespaceMap;
                }

                foreach (String name in entry.Value.Keys)
                {
                    if (namespaceMap.ContainsKey(name))
                    {
                        throw new ConfigurationException("Duplicate object detected in namespace '" + entry.Key +
                                                         "' by name '" + name + "'");
                    }
                }

                namespaceMap.PutAll(entry.Value);
            }
        }

        /// <summary>Add a single object to the collection.</summary>
        /// <param name="_namespace">is the object's namespace</param>
        /// <param name="name">is the object's name</param>
        /// <param name="clazz">is the class the object resolves to</param>
        /// <param name="type">is the object type</param>
        public void AddObject(String _namespace, String name, Type clazz, PluggableObjectType type)
        {
            EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap = pluggables.Get(_namespace);
            if (namespaceMap == null)
            {
                namespaceMap = new HashDictionary<String, Pair<Type, PluggableObjectType>>();
                pluggables[_namespace] = namespaceMap;
            }
            namespaceMap[name] = new Pair<Type, PluggableObjectType>(clazz, type);
        }

        /// <summary>
        /// Gets the underlying nested map of namespace keys and name-to-object maps.
        /// </summary>
        /// <value>The pluggables.</value>
        /// Returns
        public EDictionary<String, EDictionary<String, Pair<Type, PluggableObjectType>>> Pluggables
        {
            get { return pluggables; }
        }

        private void InitViews(IEnumerable<ConfigurationPlugInView> configurationPlugInViews)
        {
            if (configurationPlugInViews == null)
            {
                return;
            }

            foreach (ConfigurationPlugInView entry in configurationPlugInViews)
            {
                if (entry.FactoryClassName == null)
                {
                    throw new ConfigurationException("Factory class name has not been supplied for object '" + entry.Name + "'");
                }
                if (entry.Namespace == null)
                {
                    throw new ConfigurationException("Namespace name has not been supplied for object '" + entry.Name + "'");
                }
                if (entry.Name == null)
                {
                    throw new ConfigurationException("Name has not been supplied for object in namespace '" + entry.Namespace + "'");
                }

                Type type;
                try
                {
                    type = TypeHelper.ResolveType(entry.FactoryClassName);
                }
                catch (TypeLoadException)
                {
                    throw new ConfigurationException("View factory class " + entry.FactoryClassName + " could not be loaded");
                }

                EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap = pluggables.Get(entry.Namespace);
                if (namespaceMap == null)
                {
                    namespaceMap = new HashDictionary<String, Pair<Type, PluggableObjectType>>();
                    pluggables[entry.Namespace] = namespaceMap;
                }
                namespaceMap[entry.Name] = new Pair<Type, PluggableObjectType>(type, PluggableObjectType.VIEW);
            }
        }

        private void InitPatterns(IEnumerable<ConfigurationPlugInPatternObject> configEntries)
        {
            if (configEntries == null)
            {
                return;
            }

            foreach (ConfigurationPlugInPatternObject entry in configEntries)
            {
                if (entry.FactoryClassName == null)
                {
                    throw new ConfigurationException("Factory class name has not been supplied for object '" + entry.Name + "'");
                }
                if (entry.Namespace == null)
                {
                    throw new ConfigurationException("Namespace name has not been supplied for object '" + entry.Name + "'");
                }
                if (entry.Name == null)
                {
                    throw new ConfigurationException("Name has not been supplied for object in namespace '" + entry.Namespace + "'");
                }
                if (entry.PatternObjectType == null)
                {
                    throw new ConfigurationException("Pattern object type has not been supplied for object '" + entry.Name + "'");
                }

                Type type;
                try
                {
                    type = TypeHelper.ResolveType(entry.FactoryClassName);
                }
                catch (TypeLoadException)
                {
                    throw new ConfigurationException("Pattern object factory class " + entry.FactoryClassName + " could not be loaded");
                }

                EDictionary<String, Pair<Type, PluggableObjectType>> namespaceMap = pluggables.Get(entry.Namespace);
                if (namespaceMap == null)
                {
                    namespaceMap = new HashDictionary<String, Pair<Type, PluggableObjectType>>();
                    pluggables[entry.Namespace] = namespaceMap;
                }

                PluggableObjectType typeEnum;
                if (entry.PatternObjectType == ConfigurationPlugInPatternObject.PatternObjectTypeEnum.GUARD)
                {
                    typeEnum = PluggableObjectType.PATTERN_GUARD;
                }
                else if (entry.PatternObjectType == ConfigurationPlugInPatternObject.PatternObjectTypeEnum.OBSERVER)
                {
                    typeEnum = PluggableObjectType.PATTERN_OBSERVER;
                }
                else
                {
                    throw new ArgumentException("Pattern object type '" + entry.PatternObjectType + "' not known");
                }
                namespaceMap[entry.Name] = new Pair<Type, PluggableObjectType>(type, typeEnum);
            }
        }
    }
}