using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface,
        AllowMultiple = true, Inherited = false)]
    public class MapAsKnownResourceAttribute
        : Attribute
    {
        private readonly Type _resourceType;

        public MapAsKnownResourceAttribute() { }

        public MapAsKnownResourceAttribute(Type resourceType)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            _resourceType = resourceType;
        }

        #region Override

        protected internal virtual void RegisterResource(Type targetType)
        {
            Resource.RegisterAsKnownResourceType(_resourceType ?? targetType);
        }

        #endregion

    }
}