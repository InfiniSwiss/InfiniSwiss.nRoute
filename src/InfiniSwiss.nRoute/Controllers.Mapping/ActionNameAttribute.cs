using nRoute.Internal;
using System;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class ActionNameAttribute : ActionNameSelectorAttribute
    {
        private readonly string _name;

        public ActionNameAttribute(string name)
        {
            Guard.ArgumentNotNullOrWhiteSpace(name, "name");
            _name = name;
        }

        #region Properties

        public string Name
        {
            get { return _name; }
        }

        #endregion

        #region Overrides

        public override bool IsValidName(ControllerContext actionContext, string actionName, MethodInfo methodInfo)
        {
            return String.Equals(actionName, Name, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

    }
}
