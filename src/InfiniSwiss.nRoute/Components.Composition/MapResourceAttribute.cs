using System;

namespace nRoute.Components.Composition
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapResourceAttribute
         : MapResourceBaseAttribute
    {
        private const string RESOURCE_MUSTBE_OFTYPE = "{0} type must be an implementation of {1}.";

        private string _name;
        private Type _resourceType;
        private readonly InstanceLifetime _lifetime;

        public MapResourceAttribute()
         : this(null, null, InstanceLifetime.PerInstance) { }

        public MapResourceAttribute(string name)
         : this(null, name, InstanceLifetime.PerInstance) { }

        public MapResourceAttribute(InstanceLifetime lifetime)
         : this(null, null, lifetime) { }

        public MapResourceAttribute(string name, InstanceLifetime lifetime)
         : this(null, name, lifetime) { }

        public MapResourceAttribute(Type resourceType)
         : this(resourceType, null, InstanceLifetime.PerInstance) { }

        public MapResourceAttribute(Type resourceType, string name)
         : this(resourceType, name, InstanceLifetime.PerInstance) { }

        public MapResourceAttribute(Type resourceType, InstanceLifetime lifetime)
         : this(resourceType, null, lifetime) { }

        public MapResourceAttribute(Type resourceType, string name, InstanceLifetime lifetime)
        {
            _resourceType = resourceType;
            _name = name;
            _lifetime = lifetime;
        }

        #region Override

        protected internal override Type GetResourceType(Type targetType)
        {
            if (_resourceType != null) return _resourceType;

            // else we save
            _resourceType = targetType;
            return _resourceType;
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            if (_resourceType != null) EnsureIsResourceType(targetType, _resourceType);

            if (string.IsNullOrEmpty(_name)) _name = targetType.FullName;
            var _resourceMeta = new ResourceMeta(_resourceType, targetType, _lifetime);
            return new DefaultResourceLocator(_name, _resourceMeta);
        }

        #endregion

        #region Helper

        protected static void EnsureIsResourceType(Type implementationType, Type serviceType)
        {
            if (!serviceType.IsAssignableFrom(implementationType)) throw new InvalidOperationException(
               string.Format(RESOURCE_MUSTBE_OFTYPE, implementationType.FullName, serviceType.FullName));
        }

        #endregion

    }
}
