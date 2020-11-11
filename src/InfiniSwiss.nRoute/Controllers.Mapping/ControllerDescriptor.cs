using nRoute.Internal;
using System;
using System.Reflection;

namespace nRoute.Controllers.Mapping
{
    public abstract class ControllerDescriptor
        : ICustomAttributeProvider
    {

        #region Properties

        public abstract Type ControllerType { get; }

        public abstract ActionDescriptor FindAction(ControllerContext actionContext, string actionName);

        public abstract ActionDescriptor[] GetCanonicalActions();

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

    }
}
