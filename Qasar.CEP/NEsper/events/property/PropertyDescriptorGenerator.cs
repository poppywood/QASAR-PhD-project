using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

using net.esper.compat;

namespace net.esper.events.property
{
    public class PropertyDescriptorGenerator
    {
        private static int dynamicTypeIncrement = 0;

        private static readonly PropertyDescriptorGenerator staticInstance = new PropertyDescriptorGenerator();

        /// <summary>
        /// Gets the static singleton instance for the class.
        /// </summary>
        /// <value>The instance.</value>
        public static PropertyDescriptorGenerator Instance
        {
            get { return staticInstance; }
        }

        /// <summary>
        /// Assembly builder for this generator
        /// </summary>
        private readonly AssemblyBuilder assemblyBuilder;
        /// <summary>
        /// Module builder for this generator
        /// </summary>
        private readonly ModuleBuilder moduleBuilder;
        /// <summary>
        /// Caches properties that are already mapped
        /// </summary>
        private readonly Dictionary<MemberInfo, Type> propertyTypeCache;
        /// <summary>
        /// Thread synchronization
        /// </summary>
        private readonly MonitorLock dataLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyDescriptorGenerator"/> class.
        /// </summary>
        
        public PropertyDescriptorGenerator()
        {
            dataLock = new MonitorLock();

            propertyTypeCache = new Dictionary<MemberInfo, Type>();

            AppDomain currDomain = Thread.GetDomain();

            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = "DynamicAssembly";
            
            assemblyBuilder =
                currDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            moduleBuilder = assemblyBuilder.DefineDynamicModule(
                "DynamicAssembly",
                //"DynamicAssembly.dll",
                true);
        }

        /// <summary>
        /// Creates the name of the dynamic type.
        /// </summary>
        /// <returns></returns>

        private static String CreateDynamicTypeName()
        {
            string dynamicTypeName =
                String.Format("DynamicPropertyDescriptor{0}", Interlocked.Increment(ref dynamicTypeIncrement));
            return dynamicTypeName;
        }

        /// <summary>
        /// Creates the dynamic property descriptor.
        /// </summary>
        /// <param name="propertyOrField">The property or field.</param>
        /// <returns></returns>
        public PropertyDescriptor CreateDynamicPropertyDescriptor(MemberInfo propertyOrField)
        {
            return (PropertyDescriptor)Activator.CreateInstance(CreateDynamicPropertyDescriptorType(propertyOrField));
        }

        /// <summary>
        /// Creates the dynamic property descriptor.
        /// </summary>
        /// <param name="propertyOrField">The property or field.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public PropertyDescriptor CreateDynamicPropertyDescriptor(MemberInfo propertyOrField, string propertyName)
        {
            return (PropertyDescriptor)Activator.CreateInstance(CreateDynamicPropertyDescriptorType(propertyOrField, propertyName));
        }

        /// <summary>
        /// Creates the type of the dynamic property descriptor.
        /// </summary>
        /// <param name="propertyOrField">The property or field.</param>
        /// <returns></returns>
        public Type CreateDynamicPropertyDescriptorType(MemberInfo propertyOrField)
        {
            return CreateDynamicPropertyDescriptorType(propertyOrField, propertyOrField.Name);
        }

        /// <summary>
        /// Creates the type of the dynamic property descriptor.
        /// </summary>
        /// <param name="propertyOrField">The property or field.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        public Type CreateDynamicPropertyDescriptorType(MemberInfo propertyOrField, String propertyName)
        {
            Type typeForProperty;

            using (dataLock.Acquire())
            {
                if (!propertyTypeCache.TryGetValue(propertyOrField, out typeForProperty))
                {
                    TypeBuilder typeBuilder = CreateTypeBuilder(moduleBuilder, propertyOrField, propertyName);
                    propertyTypeCache[propertyOrField] = typeForProperty = typeBuilder.CreateType();
                }
            }

            return typeForProperty;
        }

        /// <summary>
        /// Creates the type builder.
        /// </summary>
        /// <param name="moduleBuilder">The module builder.</param>
        /// <param name="propertyOrField">The property or field.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns></returns>
        private static TypeBuilder CreateTypeBuilder(ModuleBuilder moduleBuilder, MemberInfo propertyOrField, String propertyName)
        {
            String typeName = CreateDynamicTypeName();

            // Create propertyInfo in case thats what this is
            PropertyInfo propertyInfo = propertyOrField as PropertyInfo;
            // Create fieldInfo in case thats what this is
            FieldInfo fieldInfo = propertyOrField as FieldInfo;
            // What are you?

            TypeBuilder typeBuilder = moduleBuilder.DefineType(
                typeName,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.Sealed |
                TypeAttributes.AutoLayout,
                typeof(AbstractPropertyDescriptor));

            CreateConstructor(typeBuilder, propertyOrField, propertyName);
            CreateComponentTypeProperty(typeBuilder, propertyOrField);

            // Complete the property descriptor by filling out the parts
            // that are specific to the type of input.

            if (propertyInfo != null)
            {
                CreatePropertyTypeProperty(typeBuilder, propertyInfo.PropertyType);
                CreateGetValue(typeBuilder, propertyInfo);
            }
            else if ( fieldInfo != null )
            {
                CreatePropertyTypeProperty(typeBuilder, fieldInfo.FieldType);
                CreateGetValue(typeBuilder, fieldInfo);
            }

            return typeBuilder;
        }

        /// <summary>
        /// Creates the property type property.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="propertyType">The property type.</param>
        private static void CreatePropertyTypeProperty(TypeBuilder typeBuilder, Type propertyType)
        {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                "PropertyType",
                PropertyAttributes.None,
                typeof(System.Type),
                null);

            //define the get method for the property 
            MethodBuilder getMethod = typeBuilder.DefineMethod(
                "get_PropertyType",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName,
                typeof(System.Type),
                null);

            ILGenerator ilGenerator = getMethod.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(Type));
            ilGenerator.Emit(OpCodes.Ldtoken, propertyType);
            ilGenerator.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Br_S, (byte)0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethod);
        }

        /// <summary>
        /// Creates the component type property.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="member">The member.</param>
        private static void CreateComponentTypeProperty(TypeBuilder typeBuilder, MemberInfo member)
        {
            Type declaringType = member.DeclaringType;

            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                "ComponentType",
                PropertyAttributes.None,
                typeof(System.Type),
                null);

            //define the get method for the property 
            MethodBuilder getMethod = typeBuilder.DefineMethod(
                "get_ComponentType",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName,
                typeof(System.Type),
                null);

            ILGenerator ilGenerator = getMethod.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(Type));
            ilGenerator.Emit(OpCodes.Ldtoken, declaringType);
            ilGenerator.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"), null);
            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Br_S, (byte)0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethod);
        }

        /// <summary>
        /// Creates the get value.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="property">The property.</param>
        private static void CreateGetValue(TypeBuilder typeBuilder, PropertyInfo property)
        {
            Type declaringType = property.DeclaringType;

            MethodBuilder methodBuilder;
            methodBuilder = typeBuilder.DefineMethod(
                "GetValue",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
                typeof(Object),
                new Type[] { typeof(Object) });

            MethodInfo underlyingGetMethod = property.GetGetMethod();

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(System.Object));
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, declaringType);
            ilGenerator.EmitCall(OpCodes.Callvirt, underlyingGetMethod, null);

            if (underlyingGetMethod.ReturnType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, underlyingGetMethod.ReturnType);
            }

            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Creates the get value.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="field">The field.</param>
        private static void CreateGetValue(TypeBuilder typeBuilder, FieldInfo field)
        {
            Type declaringType = field.DeclaringType;

            MethodBuilder methodBuilder;
            methodBuilder = typeBuilder.DefineMethod(
                "GetValue",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
                typeof(Object),
                new Type[] { typeof(Object) });

            ILGenerator ilGenerator = methodBuilder.GetILGenerator();
            ilGenerator.DeclareLocal(typeof(System.Object));
            ilGenerator.Emit(OpCodes.Ldarg_1);
            ilGenerator.Emit(OpCodes.Castclass, declaringType);
            ilGenerator.Emit(OpCodes.Ldfld, field);

            if (field.FieldType.IsValueType)
            {
                ilGenerator.Emit(OpCodes.Box, field.FieldType);
            }

            ilGenerator.Emit(OpCodes.Stloc_0);
            ilGenerator.Emit(OpCodes.Ldloc_0);
            ilGenerator.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Creates the constructor.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="member">The member.</param>
        /// <param name="propertyName">Name of the property.</param>
        private static void CreateConstructor(TypeBuilder typeBuilder, MemberInfo member, String propertyName)
        {
            ConstructorInfo baseCtorInfo =
                typeof(AbstractPropertyDescriptor).GetConstructor(new Type[] { typeof(string) });

            // Create the constructor

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public |
                MethodAttributes.HideBySig |
                MethodAttributes.SpecialName |
                MethodAttributes.RTSpecialName,
                CallingConventions.Standard,
                new Type[] { });

            ILGenerator ilGenerator = constructorBuilder.GetILGenerator();
            ilGenerator.Emit(OpCodes.Ldarg_0);
            ilGenerator.Emit(OpCodes.Ldstr, propertyName);
            ilGenerator.Emit(OpCodes.Call, baseCtorInfo);
            ilGenerator.Emit(OpCodes.Ret);
        }
    }
}
