using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Controllers.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class DefineControllerAttribute
        : MapControllerAttribute
    {
        private const string CONTROLLER_MUSTBE_OFTYPE = "{0} type must be an implementation of the IController type.";

        private readonly Type _controllerType;

        public DefineControllerAttribute(Type controllerType, string url)
            : base(url)
        {
            Guard.ArgumentNotNull(controllerType, "controllerType");
            Guard.ArgumentIsType(controllerType, typeof(IController), "controllerType", CONTROLLER_MUSTBE_OFTYPE, controllerType.FullName);

            _controllerType = controllerType;
        }

        #region Properties

        public Type ControllerType
        {
            get { return _controllerType; }
        }

        #endregion

        #region Overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return base.GetResourceLocator(_controllerType);
        }

        #endregion

    }
}
