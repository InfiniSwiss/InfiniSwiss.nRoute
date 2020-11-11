using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public abstract class MapResourceBaseAttribute
         : Attribute
    {
        #region Overridable

        protected internal virtual bool IsDefaultResource
        {
            get { return false; }
        }

        protected internal virtual bool CanInitialize(Type targetType)
        {
            return true;    // by default
        }

        protected internal abstract Type GetResourceType(Type targetType);

        protected internal abstract IResourceLocator GetResourceLocator(Type targetType);

        #endregion

    }
}
