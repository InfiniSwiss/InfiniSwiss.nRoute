using nRoute.Internal;
using System;

namespace nRoute.Components.Composition
{
    public class RelayResourceLocator<T>
        : ResourceLocatorBase<T, ResourceMeta>
        where
            T : class
    {
        private readonly Object _lock = new Object();
        private Func<T> _resolver;
        private T _instance;

        public RelayResourceLocator(string resourceName)
            : this(resourceName, InstanceLifetime.PerInstance, () => TypeBuilder.BuildType<T>()) { }

        public RelayResourceLocator(string resourceName, InstanceLifetime lifetime)
            : this(resourceName, lifetime, () => TypeBuilder.BuildType<T>()) { }

        public RelayResourceLocator(string resourceName, Func<T> resolver)
            : this(resourceName, InstanceLifetime.PerInstance, resolver) { }

        public RelayResourceLocator(string resourceName, InstanceLifetime lifetime, Func<T> resolver)
            : base(resourceName, new ResourceMeta(typeof(T), typeof(T), lifetime))
        {
            Guard.ArgumentNotNull(resolver, "resolver");
            _resolver = resolver;
        }

        #region Overrides 

        public override T GetResourceInstance()
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

        protected virtual T CreateResourceImplementationInstance()
        {
            return _resolver();
        }

        #endregion

    }
}