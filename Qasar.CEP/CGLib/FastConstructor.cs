using System;
using System.Reflection;
using System.Reflection.Emit;

namespace CGLib
{
    public class FastConstructor : FastBase
    {
        private static int constructorIdCounter = 0;

        /// <summary>
        /// Class object that this constructor belongs to.
        /// </summary>

        private readonly FastClass fastClass;

        /// <summary>
        /// Method that is being proxied
        /// </summary>

        private readonly ConstructorInfo targetConstructor;

        /// <summary>
        /// Dynamic method that is constructed for invocation.
        /// </summary>

        private readonly DynamicMethod dynamicMethod;

        private readonly Invoker invoker;

        /// <summary>
        /// Gets the target constructor.
        /// </summary>
        /// <value>The target method.</value>
        public ConstructorInfo Target
        {
            get { return targetConstructor; }
        }

        /// <summary>
        /// Gets the type of the declaring.
        /// </summary>
        /// <value>The type of the declaring.</value>
        public FastClass DeclaringType
        {
            get { return fastClass; }
        }

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <value>The parameter count.</value>
        public int ParameterCount
        {
            get { return targetConstructor.GetParameters().Length; }
        }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        /// <returns></returns>
        internal static Type[] GetParameterTypes(ConstructorInfo constructor)
        {
            // Get the method parameters
            ParameterInfo[] paramInfoList = constructor.GetParameters();
            // Convert the paramInfoList into raw types
            Type[] paramTypeList = new Type[paramInfoList.Length];
            for (int ii = 0; ii < paramInfoList.Length; ii++)
            {
                paramTypeList[ii] = paramInfoList[ii].ParameterType;
            }

            // Return the list
            return paramTypeList;
        }

        /// <summary>
        /// Constructs a wrapper around the target constructor.
        /// </summary>
        /// <param name="_fastClass">The _fast class.</param>
        /// <param name="constructor">The constructor.</param>

        internal FastConstructor(FastClass _fastClass, ConstructorInfo constructor)
        {
            // Store the class that spawned us
            fastClass = _fastClass;

            targetConstructor = constructor;

            int uid = System.Threading.Interlocked.Increment(ref constructorIdCounter);

            // Create a unique name for the method
            String dynamicMethodName = "_CGLibC_" + fastClass.Id + "_" + uid;
            // Generate the method
            dynamicMethod = new DynamicMethod(
                dynamicMethodName,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(Object),
                new Type[] { typeof(Object), typeof(Object[]) },
                targetConstructor.Module,
                true);

            EmitInvoker(targetConstructor, dynamicMethod.GetILGenerator());
            invoker = (Invoker)dynamicMethod.CreateDelegate(typeof(Invoker));
        }

        static void EmitInvoker(ConstructorInfo ctor, ILGenerator il)
        {
            il.DeclareLocal(typeof(object));

            // Get the method parameters
            Type[] parameterTypes = GetParameterTypes(ctor);

            // Load method arguments
            for (int ii = 0; ii < parameterTypes.Length; ii++)
            {
                Type paramType = parameterTypes[ii];
                il.Emit(OpCodes.Ldarg_1);
                EmitLoadInt(il, ii);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastConversion(il, paramType);
            }

            // Emit code to construct object
            il.Emit(OpCodes.Newobj, ctor);

            if (ctor.DeclaringType.IsValueType)
            {
                il.Emit(OpCodes.Box, ctor.DeclaringType);
            }

            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            // Emit code for return value
            il.Emit(OpCodes.Ret);
        }
    
        /// <summary>
        /// Creates a new instance of the target using the parameters.
        /// </summary>
        /// <param name="paramList">The param list.</param>
        public Object New(params Object[] paramList)
        {
#if USE_REFLECTION
            return targetMethod.Invoke(target, paramList);
#else
            return invoker(null, paramList);
#endif
        }
    }
}
