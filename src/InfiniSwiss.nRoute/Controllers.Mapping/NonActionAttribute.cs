using System;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class NonActionAttribute
        : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext actionContext, MethodInfo methodInfo)
        {
            return false;
        }
    }
}
