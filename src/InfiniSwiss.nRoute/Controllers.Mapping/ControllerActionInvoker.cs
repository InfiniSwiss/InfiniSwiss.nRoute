using nRoute.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace nRoute.Controllers.Mapping
{
    public class ControllerActionInvoker : IActionInvoker
    {

        #region Declarations

        private const string ACTION_NOTRESOLVED = "Action '{0}' in Controller Type {1} for Url '{2}' could not be resolved.";

        private readonly static Dictionary<Type, ControllerDescriptor> _controllerDescriptorsCache = new Dictionary<Type, ControllerDescriptor>();
        private readonly static Object _lock = new Object();

        private readonly Type _controllerType;
        private readonly ControllerDescriptor _controllerDescriptor;

        #endregion

        public ControllerActionInvoker(Type controllerType)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            _controllerType = controllerType;

            lock (_lock)
            {
                if (!_controllerDescriptorsCache.TryGetValue(controllerType, out _controllerDescriptor))
                {
                    _controllerDescriptor = new ReflectedControllerDescriptor(controllerType);
                    _controllerDescriptorsCache.Add(controllerType, _controllerDescriptor);
                }
            }
        }

        #region Properties

        public Type ControllerType
        {
            get { return _controllerType; }
        }

        #endregion

        #region IActionInvoker Members

        public void InvokeAction(ControllerContext actionContext, string actionName)
        {
            Guard.ArgumentNotNull(actionContext, "actionContext");
            Guard.ArgumentNotNullOrWhiteSpace(actionName, "actionName");

            // we get the action descriptor
            var _actionDescriptor = _controllerDescriptor.FindAction(actionContext, actionName);
            if (_actionDescriptor == null) throw new InvalidOperationException(string.Format(
                ACTION_NOTRESOLVED, actionName, ControllerType.FullName, actionContext.Request.RequestUrl));

            // tranform to a dictionary
            IDictionary<string, Object> _dictionary = (actionContext.ResponseParameters == null) ? null :
                actionContext.ResponseParameters.ToDictionary((p) => p.Key, (p) => p.Value, StringComparer.InvariantCultureIgnoreCase);

            // we get the ActionResult and if it ain't null, we execute 
            var _result = _actionDescriptor.Execute(actionContext, _dictionary) as ActionResult ??
                EmptyResult.Instance;
            _result.ExecuteResult(actionContext);

        }

        #endregion

        #region Helpers



        #endregion

    }
}