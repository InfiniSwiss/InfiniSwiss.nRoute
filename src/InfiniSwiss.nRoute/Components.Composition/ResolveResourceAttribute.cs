using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = false)]
    public class ResolveResourceAttribute
        : ResolveResourceBaseAttribute
    {
        private bool _isNullable;

        public ResolveResourceAttribute() { }

        public ResolveResourceAttribute(Type resourceType)
            : this(resourceType, null) { }

        public ResolveResourceAttribute(string resourceName)
            : this(null, resourceName) { }

        public ResolveResourceAttribute(Type resourceType, string resourceName)
            : base(resourceType, resourceName) { }

        #region Properties

        public bool IsNullable
        {
            get { return _isNullable; }
            set { _isNullable = value; }
        }

        #endregion

        #region Override

        protected internal override Object ResolveResource(Type targetType)
        {
            return GetResourceInstance(base.ResourceType ?? targetType);
        }

        #endregion

        #region Helper

        protected virtual Object GetResourceInstance(Type resourceType)
        {
            if (!IsNullable)
            {
                return string.IsNullOrEmpty(base.ResourceName) ? ResourceLocator.GetResource(resourceType)
                       : Resource.GetResource(resourceType, base.ResourceName);
            }
            else
            {
                // if it is nullable, then we first check and then 
                if (string.IsNullOrEmpty(base.ResourceName))
                {
                    return Resource.IsResourceOrAdapterRegistered(resourceType) ? Resource.GetResource(resourceType) : null;
                }
                else
                {
                    return Resource.IsResourceOrAdapterRegistered(resourceType, base.ResourceName)
                        ? Resource.GetResource(resourceType, base.ResourceName) : null;
                }
            }
        }

        #endregion

    }
}