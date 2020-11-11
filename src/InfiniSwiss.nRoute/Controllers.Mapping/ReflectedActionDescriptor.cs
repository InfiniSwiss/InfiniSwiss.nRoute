using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    public class ReflectedActionDescriptor : ActionDescriptor
    {
        private readonly string _actionName;
        private readonly ControllerDescriptor _controllerDescriptor;
        private readonly MethodInfo _methodInfo;

        public ReflectedActionDescriptor(MethodInfo methodInfo, string actionName, ControllerDescriptor controllerDescriptor)
            : this(methodInfo, actionName, controllerDescriptor, true) { }

        internal ReflectedActionDescriptor(MethodInfo methodInfo, string actionName, ControllerDescriptor controllerDescriptor,
            bool validateMethod)
        {
            Guard.ArgumentNotNull(methodInfo, "methodInfo");
            Guard.ArgumentNotNullOrWhiteSpace(actionName, "actionName");
            Guard.ArgumentNotNull(controllerDescriptor, "controllerDescriptor");

            // check
            if (validateMethod) VerifyActionMethodIsCallable(methodInfo);

            _methodInfo = methodInfo;
            _actionName = actionName;
            _controllerDescriptor = controllerDescriptor;
        }

        #region Properties

        public MethodInfo MethodInfo
        {
            get { return _methodInfo; }
        }

        public ControllerDescriptor ControllerDescriptor
        {
            get { return _controllerDescriptor; }
        }

        #endregion

        #region ActionDescriptor

        public override string ActionName
        {
            get { return _actionName; }
        }

        public override object Execute(ControllerContext actionContext, IDictionary<string, object> parameters)
        {
            Guard.ArgumentNotNull(actionContext, "actionContext");
            Guard.ArgumentNotNull(parameters, "parameters");

            // extract the param
            ParameterInfo[] _parameterInfos = MethodInfo.GetParameters();
            var _rawParameterValues = from _parameterInfo in _parameterInfos
                                      select ExtractParameterFromDictionary(_parameterInfo, parameters, MethodInfo);
            object[] parametersArray = _rawParameterValues.ToArray();

            // and execute
            var dispatcher = new ActionMethodDispatcher(MethodInfo);
            object actionReturnValue = dispatcher.Execute((ControllerBase)actionContext.Controller, parametersArray);
            return actionReturnValue;
        }

        #endregion

        #region ICustomAttributeProvider Overrides

        public override object[] GetCustomAttributes(bool inherit)
        {
            return MethodInfo.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return MethodInfo.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return MethodInfo.IsDefined(attributeType, inherit);
        }

        #endregion

    }
}
