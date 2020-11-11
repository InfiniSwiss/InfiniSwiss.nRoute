using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public class DefineResourceAttribute
        : MapResourceAttribute
    {
        private readonly Type _implementationType;

        public DefineResourceAttribute(Type resourceType)
            : this(resourceType, resourceType, null, InstanceLifetime.PerInstance) { }

        public DefineResourceAttribute(Type resourceType, string name)
            : this(resourceType, resourceType, name, InstanceLifetime.PerInstance) { }

        public DefineResourceAttribute(Type resourceType, InstanceLifetime lifetime)
            : this(resourceType, resourceType, null, lifetime) { }

        public DefineResourceAttribute(Type resourceType, string name, InstanceLifetime lifetime)
            : this(resourceType, resourceType, name, lifetime) { }

        public DefineResourceAttribute(Type resourceType, Type implementationType)
            : this(resourceType, implementationType, null, InstanceLifetime.PerInstance) { }

        public DefineResourceAttribute(Type resourceType, Type implementationType, string name)
            : this(resourceType, implementationType, name, InstanceLifetime.PerInstance) { }

        public DefineResourceAttribute(Type resourceType, Type implementationType, InstanceLifetime lifetime)
            : this(resourceType, implementationType, null, lifetime) { }

        public DefineResourceAttribute(Type resourceType, Type implementationType, string name, InstanceLifetime lifetime)
            : base(resourceType, name, lifetime)
        {
            Guard.ArgumentNotNull(implementationType, "implementationType");
            EnsureIsResourceType(implementationType, resourceType);

            _implementationType = implementationType;
        }

        public virtual Type ImplementationType
        {
            get { return _implementationType; }
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return base.GetResourceLocator(this.ImplementationType);
        }
    }
}
