using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace CGLib
{
    public class FastMethod : FastBase
    {
        /// <summary>
        /// Class object that this method belongs to.
        /// </summary>

        private readonly FastClass fastClass;

        /// <summary>
        /// Method that is being proxied
        /// </summary>

        private readonly MethodInfo targetMethod;

        /// <summary>
        /// Dynamic method that is constructed for invocation.
        /// </summary>

        private readonly DynamicMethod dynamicMethod;

        private readonly Invoker invoker;

        /// <summary>
        /// Gets the target method.
        /// </summary>
        /// <value>The target method.</value>
        public MethodInfo Target
        {
            get { return targetMethod; }
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        public String Name
        {
            get { return targetMethod.Name; }
        }

        /// <summary>
        /// Gets the type of the return.
        /// </summary>
        /// <value>The type of the return.</value>
        public Type ReturnType
        {
            get { return targetMethod.ReturnType; }
        }

        /// <summary>
        /// Gets the type of the declaring.
        /// </summary>
        /// <value>The type of the declaring.</value>
        public FastClass DeclaringType
        {
            get { return fastClass;  }            
        }

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        /// <value>The parameter count.</value>
        public int ParameterCount
        {
            get { return targetMethod.GetParameters().Length; }
        }

        /// <summary>
        /// Constructs a wrapper around the target method.
        /// </summary>
        /// <param name="_fastClass">The _fast class.</param>
        /// <param name="method">The method.</param>

        internal FastMethod(FastClass _fastClass, MethodInfo method)
        {
            // Store the class that spawned us
            fastClass = _fastClass;

            targetMethod = method;

            // Create a unique name for the method
            String dynamicMethodName = "_CGLibM_" + fastClass.Id + "_" + method.Name;
            // Generate the method
            dynamicMethod = new DynamicMethod(
                dynamicMethodName,
                MethodAttributes.Public | MethodAttributes.Static,
                CallingConventions.Standard,
                typeof(Object),
                new Type[]{ typeof(Object), typeof(Object[]) }, 
                targetMethod.Module,
                true);

            EmitInvoker(method, dynamicMethod.GetILGenerator());
            invoker = (Invoker) dynamicMethod.CreateDelegate(typeof (Invoker));
        }

        static void EmitInvoker(MethodInfo method, ILGenerator il)
        {
            il.DeclareLocal(typeof(object));

            // Get the method parameters
            Type[] parameterTypes = GetParameterTypes(method);

            // Is the method non-static
            if ( !method.IsStatic )
            {
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Castclass, method.DeclaringType);
            }

            // Load method arguments
            for( int ii = 0 ; ii < parameterTypes.Length ; ii++ )
            {
                Type paramType = parameterTypes[ii];
                il.Emit(OpCodes.Ldarg_1);
                EmitLoadInt(il, ii);
                il.Emit(OpCodes.Ldelem_Ref);
                EmitCastConversion(il, paramType);
            }
            
            // Emit code to call the method
            il.Emit(OpCodes.Call, method);

            if (method.ReturnType == typeof(void))
            {
                il.Emit(OpCodes.Ldnull);
            }
            else if ( method.ReturnType.IsValueType )
            {
                il.Emit(OpCodes.Box, method.ReturnType);
            }

            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            // Emit code for return value
            il.Emit(OpCodes.Ret);
        }

        /// <summary>
        /// Invokes the method on the specified target.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="paramList">The param list.</param>
        public Object Invoke(Object target, params Object[] paramList)
        {
#if USE_REFLECTION
            return targetMethod.Invoke(target, paramList);
#else
            return invoker(target, paramList);
#endif
        }

        /// <summary>
        /// Invokes the method on the specified target.
        /// </summary>
        /// <param name="paramList">The param list.</param>
        /// <returns></returns>
        public Object InvokeStatic(params Object[] paramList)
        {
            return invoker(null, paramList);
        }
    }
}
