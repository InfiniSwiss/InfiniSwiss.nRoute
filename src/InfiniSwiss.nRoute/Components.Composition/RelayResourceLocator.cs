using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public class RelayResourceLocator
        : ResourceLocatorBase<Object, ResourceMeta>
    {
        private readonly Object _lock = new Object();
        private Func<Object> _resolver;
        private Object _instance;

        public RelayResourceLocator(Type resourceType, string resourceName)
            : this(resourceType, resourceName, InstanceLifetime.PerInstance, () => TypeBuilder.BuildType(resourceType)) { }

        public RelayResourceLocator(Type resourceType, string resourceName, InstanceLifetime lifetime)
            : this(resourceType, resourceName, lifetime, () => TypeBuilder.BuildType(resourceType)) { }

        public RelayResourceLocator(Type resourceType, string resourceName, Func<Object> resolver)
            : this(resourceType, resourceName, InstanceLifetime.PerInstance, resolver) { }

        public RelayResourceLocator(Type resourceType, string resourceName, InstanceLifetime lifetime, Func<Object> resolver)
            : base(resourceName, new ResourceMeta(resourceType, resourceType, lifetime))
        {
            Guard.ArgumentNotNull(resourceType, "resourceType");
            Guard.ArgumentNotNull(resolver, "resolver");
            _resolver = resolver;
        }

        #region Overrides 

        public override object GetResourceInstance()
        {
            if (ResourceMeta.Lifetime == InstanceLifetime.Singleton)
            {
                // we hold a single instance
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = CreateResourceImplementationInstance();
                    }
                }
                return _instance;
            }
            else
            {
                // always return a new instance
                return CreateResourceImplementationInstance();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (_lock)
                {
                    if (_instance != null)
                    {
                        _instance = null;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        protected virtual Object CreateResourceImplementationInstance()
        {
            return _resolver();
        }

        #endregion

    }
}