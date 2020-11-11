using System;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ActionNameSelectorAttribute
        : Attribute
    {

        public abstract bool IsValidName(ControllerContext actionContext, string actionName, MethodInfo methodInfo);

    }
}
