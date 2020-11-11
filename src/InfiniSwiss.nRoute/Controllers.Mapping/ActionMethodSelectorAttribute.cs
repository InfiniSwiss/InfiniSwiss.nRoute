using System;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public abstract class ActionMethodSelectorAttribute : Attribute
    {
        public abstract bool IsValidForRequest(ControllerContext actionContext, MethodInfo methodInfo);
    }
}