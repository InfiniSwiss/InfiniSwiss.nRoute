using nRoute.Internal;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    // The methods in this class don't perform error checking; that is the responsibility of the
    // caller.
    internal sealed class ActionMethodDispatcher
    {

        private delegate object ActionExecutor(ControllerBase controller, object[] parameters);
        private delegate void VoidActionExecutor(ControllerBase controller, object[] parameters);

        private readonly ActionExecutor _executor;

        public ActionMethodDispatcher(MethodInfo methodInfo)
        {
            _executor = GetExecutor(methodInfo);
            MethodInfo = methodInfo;
        }

        #region Properties

        public MethodInfo MethodInfo { get; private set; }

        #endregion

        #region Methods

        public object Execute(ControllerBase controller, object[] parameters)
        {
            return _executor(controller, parameters);
        }

        #endregion

        #region Helpers

        private static ActionExecutor GetExecutor(MethodInfo methodInfo)
        {
            Guard.ArgumentNotNull(methodInfo, "methodInfo");

            // Parameters to executor
            ParameterExpression controllerParameter = Expression.Parameter(typeof(ControllerBase), "controller");
            ParameterExpression parametersParameter = Expression.Parameter(typeof(object[]), "parameters");

            // Build parameter list
            List<Expression> parameters = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (int i = 0; i < paramInfos.Length; i++)
            {
                ParameterInfo paramInfo = paramInfos[i];
                BinaryExpression valueObj = Expression.ArrayIndex(parametersParameter, Expression.Constant(i));
                UnaryExpression valueCast = Expression.Convert(valueObj, paramInfo.ParameterType);

                // valueCast is "(Ti) parameters[i]"
                parameters.Add(valueCast);
            }

            // Call method
            UnaryExpression instanceCast = (!methodInfo.IsStatic) ? Expression.Convert(controllerParameter, methodInfo.ReflectedType) : null;
            MethodCallExpression methodCall = methodCall = Expression.Call(instanceCast, methodInfo, parameters);

            // methodCall is "((TController) controller) method((T0) parameters[0], (T1) parameters[1], ...)"
            // Create function
            if (methodCall.Type == typeof(void))
            {
                Expression<VoidActionExecutor> lambda = Expression.Lambda<VoidActionExecutor>(methodCall, controllerParameter, parametersParameter);
                VoidActionExecutor voidExecutor = lambda.Compile();
                return WrapVoidAction(voidExecutor);
            }
            else
            {
                // must coerce methodCall to match ActionExecutor signature
                UnaryExpression castMethodCall = Expression.Convert(methodCall, typeof(object));
                Expression<ActionExecutor> lambda = Expression.Lambda<ActionExecutor>(castMethodCall, controllerParameter, parametersParameter);
                return lambda.Compile();
            }
        }

        private static ActionExecutor WrapVoidAction(VoidActionExecutor executor)
        {
            return delegate (ControllerBase controller, object[] parameters)
            {
                executor(controller, parameters);
                return null;
            };
        }


        #endregion

    }

}
