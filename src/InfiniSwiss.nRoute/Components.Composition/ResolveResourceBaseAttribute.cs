using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public abstract class ResolveResourceBaseAttribute
        : Attribute, IResourceResolver
    {
        private readonly Type _resourceType;
        private readonly string _resourceName;

        protected ResolveResourceBaseAttribute() { }

        protected ResolveResourceBaseAttribute(Type resourceType)
            : this(resourceType, null) { }

        protected ResolveResourceBaseAttribute(string resourceName)
            : this(null, resourceName) { }

        protected ResolveResourceBaseAttribute(Type resourceType, string resourceName)
        {
            _resourceType = resourceType;
            _resourceName = resourceName;
        }

        #region Properties

        public Type ResourceType
        {
            get { return _resourceType; }
        }

        public string ResourceName
        {
            get { return _resourceName; }
        }

        #endregion

        #region Must Override

        protected internal abstract Object ResolveResource(Type targetType);

        #endregion

        #region IResolveTarget Members

        object IResourceResolver.Resolve(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            return this.ResolveResource(targetType);
        }

        #endregion
    }
}
