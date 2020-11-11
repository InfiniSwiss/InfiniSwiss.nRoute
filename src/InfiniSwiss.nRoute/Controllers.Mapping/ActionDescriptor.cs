using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    public abstract class ActionDescriptor
        : ICustomAttributeProvider
    {

        #region Declarations

        private const string MUST_BE_CONTROLLERTYPE =
            "Cannot create a descriptor for instance method '{0}' on type '{1}' because the type does not derive from ControllerBase.";
        private const string CANNOT_BE_OPEN_GENERIC_TYPES =
            "Cannot call action method '{0}' on controller '{1}' because the action method is a generic method.";
        private const string NO_OUTREF_PARAMS =
            "Cannot call action method '{0}' on controller '{1}' because the parameter '{2}' is passed by reference.";
        private const string PARAM_NOTIN_DICTIONARY =
            "The parameters dictionary does not contain an entry for parameter '{0}' of type '{1}' for method '{2}' in '{3}'. The dictionary must contain an entry for each parameter, including parameters that have null values.";
        private const string PARAM_CANNOT_BENULL =
            "The parameters dictionary contains a null entry for parameter '{0}' of non-nullable type '{1}' for method '{2}' in '{3}'. An optional parameter must be a reference type, a nullable type, or be declared as an optional parameter.";
        private const string PARAM_WRONG_TYPE =
            "The parameters dictionary contains an invalid entry for parameter '{0}' for method '{1}' in '{2}'. The dictionary contains a value of type '{3}', but the parameter requires a value of type '{4}'.";

        #endregion

        #region Properties

        public abstract string ActionName { get; }

        #endregion

        #region Methods

        public abstract object Execute(ControllerContext actionContext, IDictionary<string, object> parameters);

        #endregion

        #region ICustomAttributeProvider Members

        public virtual object[] GetCustomAttributes(bool inherit)
        {
            return GetCustomAttributes(typeof(object), inherit);
        }

        public virtual object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            Guard.ArgumentNotNull(attributeType, "attributeType");
            return (object[])Array.CreateInstance(attributeType, 0);
        }

        public virtual bool IsDefined(Type attributeType, bool inherit)
        {
            Guard.ArgumentNotNull(attributeType, "attributeType");
            return false;
        }

        #endregion

        #region Static Helpers

        internal static object ExtractParameterFromDictionary(ParameterInfo parameterInfo, IDictionary<string, object> parameters,
            MethodInfo methodInfo)
        {
            object value;

            // the key should always be present, even if the parameter value is null
            Guard.ArgumentValue(!parameters.TryGetValue(parameterInfo.Name, out value), "parameters", PARAM_NOTIN_DICTIONARY,
                parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);

            // tried to pass a null value for a non-nullable parameter type
            Guard.ArgumentValue((value == null && !(!parameterInfo.ParameterType.IsValueType ||
                (Nullable.GetUnderlyingType(parameterInfo.ParameterType) != null))), "parameters", PARAM_CANNOT_BENULL,
                    parameterInfo.Name, parameterInfo.ParameterType, methodInfo, methodInfo.DeclaringType);

            // value was supplied but is not of the proper type
            Guard.ArgumentValue(value != null && !parameterInfo.ParameterType.IsInstanceOfType(value), "parameters",
                PARAM_WRONG_TYPE, parameterInfo.Name, methodInfo, methodInfo.DeclaringType, value.GetType(), parameterInfo.ParameterType);

            return value;
        }

        internal static void VerifyActionMethodIsCallable(MethodInfo methodInfo)
        {
            // we can't call instance methods where the 'this' parameter is a type other than ControllerBase
            if (!methodInfo.IsStatic && !typeof(ControllerBase).IsAssignableFrom(methodInfo.ReflectedType))
            {
                throw new InvalidOperationException(string.Format(MUST_BE_CONTROLLERTYPE,
                    methodInfo, methodInfo.ReflectedType.FullName));
            }

            // we can't call methods with open generic type parameters
            if (methodInfo.ContainsGenericParameters)
            {
                throw new InvalidOperationException(string.Format(CANNOT_BE_OPEN_GENERIC_TYPES,
                    methodInfo, methodInfo.ReflectedType.FullName));
            }

            // we can't call methods with ref/out parameters
            ParameterInfo[] parameterInfos = methodInfo.GetParameters();
            foreach (ParameterInfo parameterInfo in parameterInfos)
            {
                if (parameterInfo.IsOut || parameterInfo.ParameterType.IsByRef)
                {
                    throw new InvalidOperationException(string.Format(NO_OUTREF_PARAMS,
                        methodInfo, methodInfo.ReflectedType.FullName, parameterInfo));
                }
            }
        }

        #endregion

    }
}
