using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public class ResourceMeta
    {
        private readonly Type _implementationType;
        private readonly Type _resourceType;
        private readonly InstanceLifetime _lifetime;

        public ResourceMeta(Type resourceType, Type implementationType, InstanceLifetime lifetime)
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNull(implementationType, "implementationType");

            _resourceType = resourceType;
            _implementationType = implementationType;
            _lifetime = lifetime;
        }

        public Type ResourceType
        {
            get { return _resourceType; }
        }

        public Type ImplementationType
        {
            get { return _implementationType; }
        }

        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }
    }
}
