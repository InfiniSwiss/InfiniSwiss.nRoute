using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Services
{
    public class DefaultServicesLocator
        : ResourceLocatorBase<Object, ServiceMeta>
    {
        private readonly Object _lock = new Object();
        private Object _instance;

        public DefaultServicesLocator(ServiceMeta meta)
            : base(meta.ServiceName, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");

            if (meta.InitializationMode == InitializationMode.WhenAvailable &&
                meta.Lifetime != InstanceLifetime.PerInstance)
            {
                GetServiceInstance();      // we create one instance
            }
        }

        #region IResourceLocator Members

        public override object GetResourceInstance()
        {
            return GetServiceInstance();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // TODO: check if locks in disposer are a good idea? 
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

        private Object GetServiceInstance()
        {
            if (ResourceMeta.Lifetime == InstanceLifetime.Singleton)
            {
                // we hold a single instance
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = CreateServiceTypeInstance();
                    }
                }
                return _instance;
            }
            else
            {
                // always return a new instance
                return CreateServiceTypeInstance();
            }
        }

        protected virtual Object CreateServiceTypeInstance()
        {
            return TypeBuilder.BuildType(ResourceMeta.ImplementationType);
        }

        #endregion

    }
}
