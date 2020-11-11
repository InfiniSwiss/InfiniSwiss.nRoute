using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    public class ReflectedControllerDescriptor
        : ControllerDescriptor
    {
        private readonly Type _controllerType;
        private readonly Object _lock = new Object();
        private readonly Dictionary<MethodInfo, ActionDescriptor> _actionDescriptorCache = new Dictionary<MethodInfo, ActionDescriptor>();
        private readonly ActionMethodSelector _selector;

        public ReflectedControllerDescriptor(Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");

            _controllerType = controllerType;
            _selector = new ActionMethodSelector(controllerType);
        }

        #region ControllerDescriptor Overrides

        public override Type ControllerType
        {
            get { return _controllerType; }
        }

        public override ActionDescriptor FindAction(ControllerContext actionContext, string actionName)
        {
            // we use a cache
            var _methodInfo = _selector.FindActionMethod(actionContext, actionName);
            lock (_lock)
            {
                var _actionDescriptor = (ActionDescriptor)null;
                if (!_actionDescriptorCache.TryGetValue(_methodInfo, out _actionDescriptor))
                {
                    _actionDescriptor = new ReflectedActionDescriptor(_methodInfo, actionName, this);
                    _actionDescriptorCache.Add(_methodInfo, _actionDescriptor);
                }
                return _actionDescriptor;
            }
        }

        public override ActionDescriptor[] GetCanonicalActions()
        {
            throw new NotSupportedException();
        }

        #endregion

        #region ICustomAttributeProvider Overrides

        public override object[] GetCustomAttributes(bool inherit)
        {
            return ControllerType.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return ControllerType.GetCustomAttributes(attributeType, inherit);
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return ControllerType.IsDefined(attributeType, inherit);
        }

        #endregion

    }
}
