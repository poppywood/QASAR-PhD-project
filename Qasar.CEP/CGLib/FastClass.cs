using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace CGLib
{
    /// <summary>
    /// Provides access to dynamic code reflection; resulting code reflection
    /// will be faster than using reflection, but will result in code being
    /// loaded into the current address space.  In other words, this class
    /// builds a proxy around a class and generates MSIL specifically for
    /// the purpose of exposing functionality.
    /// </summary>

    public sealed class FastClass
    {
        /// <summary>
        /// Static sequential id generation for classes
        /// </summary>
        private static int fastClassId;

        /// <summary>
        /// Unique identifier assigned to each FastClass upon creation.
        /// </summary>
        private readonly int id;

        /// <summary>
        /// Type that the FastClass is proxying.
        /// </summary>

        private readonly Type targetType;

        /// <summary>
        /// Maps methods to FastMethods
        /// </summary>

        private readonly Dictionary<MethodInfo, FastMethod> methodCache;

        /// <summary>
        /// Maps fields to FastFields
        /// </summary>
        private readonly Dictionary<FieldInfo, FastField> fieldCache;

        /// <summary>
        /// Maps fields to FastProperty
        /// </summary>
        private readonly Dictionary<PropertyInfo, FastProperty> propertyCache;

        /// <summary>
        /// Maps constructors to FastConstructors
        /// </summary>
        private readonly Dictionary<ConstructorInfo, FastConstructor> ctorCache;

        /// <summary>
        /// Internal lock
        /// </summary>

        private readonly Object instanceLock;

        /// <summary>
        /// Gets the type the FastClass is proxying for.
        /// </summary>
        public Type TargetType
        {
            get { return targetType; }
        }

        /// <summary>
        /// Maps types to their FastClass implementation.
        /// </summary>
        private static readonly Dictionary<Type, FastClass> fastClassCache;

        /// <summary>
        /// Static lock used for the fastClassCache
        /// </summary>

        private static readonly Object fastClassCacheLock;

        /// <summary>
        /// Initializes the <see cref="FastClass"/> class.
        /// </summary>
        static FastClass()
        {
            fastClassId = 0;
            fastClassCache = new Dictionary<Type, FastClass>();
            fastClassCacheLock = new Object();
        }

        /// <summary>
        /// Gets the unique internally assigned identifier.
        /// </summary>
        /// <value>The id.</value>
        internal int Id
        {
            get { return id; }
        }

        /// <summary>
        /// Creates a FastClass for the specified target type.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        /// <returns></returns>
        public static FastClass Create(Type targetType)
        {
            lock(fastClassCacheLock)
            {
                FastClass fastClass = null;
                if (!fastClassCache.TryGetValue(targetType, out fastClass))
                {
                    fastClass = new FastClass(targetType, fastClassId++);
                    fastClassCache[targetType] = fastClass;
                }

                return fastClass;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FastClass"/> class.
        /// </summary>
        /// <param name="targetType">Type of the target.</param>
        private FastClass(Type targetType, int id)
        {
            this.id = id;
            this.targetType = targetType;
            this.instanceLock = new Object();
            this.methodCache = new Dictionary<MethodInfo, FastMethod>();
            this.fieldCache = new Dictionary<FieldInfo, FastField>();
            this.propertyCache = new Dictionary<PropertyInfo, FastProperty>();
            this.ctorCache = new Dictionary<ConstructorInfo, FastConstructor>();
        }

        #region "FastConstructor"
        /// <summary>
        /// Gets a fast constructor implementation for the given
        /// constructor.
        /// </summary>
        /// <param name="ctor">The constructor.</param>
        /// <returns></returns>
        public FastConstructor GetConstructor(ConstructorInfo ctor)
        {
            lock (instanceLock)
            {
                FastConstructor fastCtor;
                if (!ctorCache.TryGetValue(ctor, out fastCtor))
                {
                    fastCtor = new FastConstructor(this, ctor);
                    ctorCache[ctor] = fastCtor;
                }

                return fastCtor;
            }
        }

        /// <summary>
        /// Gets the constructor.
        /// </summary>
        /// <param name="paramTypes">The param types.</param>
        /// <returns></returns>
        public FastConstructor GetConstructor(Type[] paramTypes)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            ConstructorInfo ctor = targetType.GetConstructor(bindingFlags, null, paramTypes, null);
            if (ctor == null)
            {
                return null;
            }
            else
            {
                return GetConstructor(ctor);
            }
        }
        #endregion

        #region "FastMethod"
        /// <summary>
        /// Creates the method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public static FastMethod CreateMethod(MethodInfo method)
        {
            FastClass fastClass = Create(method.DeclaringType);
            return fastClass.GetMethod(method);
        }

        /// <summary>
        /// Gets a fast method implementation for the given
        /// method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns></returns>
        public FastMethod GetMethod(MethodInfo method)
        {
            lock (instanceLock)
            {
                FastMethod fastMethod;
                if (!methodCache.TryGetValue(method, out fastMethod))
                {
                    fastMethod = new FastMethod(this, method);
                    methodCache[method] = fastMethod;
                }

                return fastMethod;
            }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FastMethod GetMethod(String name)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            MethodInfo method = targetType.GetMethod(name, bindingFlags);
            if (method == null)
            {
                return null;
            }
            else
            {
                return GetMethod(method);
            }
        }

        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="paramTypes">The param types.</param>
        /// <returns></returns>
        public FastMethod GetMethod(String name, Type[] paramTypes)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public|
                BindingFlags.NonPublic|
                BindingFlags.Instance|
                BindingFlags.Static;

            MethodInfo method = targetType.GetMethod(name, bindingFlags, null, paramTypes, null);
            if (method == null)
            {
                return null;
            }
            else
            {
                return GetMethod(method);
            }
        }
        #endregion

        #region "FastField"
        /// <summary>
        /// Gets a fast field implementation for the given
        /// field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns></returns>
        public FastField GetField(FieldInfo field)
        {
            lock (instanceLock)
            {
                FastField fastField;
                if (!fieldCache.TryGetValue(field, out fastField))
                {
                    fastField = new FastField(this, field);
                    fieldCache[field] = fastField;
                }

                return fastField;
            }
        }

        /// <summary>
        /// Gets the field.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FastField GetField(String name)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            FieldInfo field = targetType.GetField(name, bindingFlags);
            if (field == null)
            {
                return null;
            }
            else
            {
                return GetField(field);
            }
        }
        #endregion

        #region "FastProperty"
        /// <summary>
        /// Gets a fast property implementation for the given
        /// property.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public FastProperty GetProperty(PropertyInfo property)
        {
            lock (instanceLock)
            {
                FastProperty fastProperty;
                if (!propertyCache.TryGetValue(property, out fastProperty))
                {
                    fastProperty = new FastProperty(this, property);
                    propertyCache[property] = fastProperty;
                }

                return fastProperty;
            }
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public FastProperty GetProperty(String name)
        {
            BindingFlags bindingFlags =
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Static;

            PropertyInfo property = targetType.GetProperty(name, bindingFlags);
            if (property == null)
            {
                return null;
            }
            else
            {
                return GetProperty(property);
            }
        }
        #endregion
    }
}
