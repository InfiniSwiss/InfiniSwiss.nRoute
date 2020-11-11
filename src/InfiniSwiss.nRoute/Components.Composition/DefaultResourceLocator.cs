using System;

namespace nRoute.Components.Composition
{
    public class DefaultResourceLocator
        : ResourceLocatorBase<Object, ResourceMeta>
    {
        private readonly Object _lock = new Object();
        private Object _instance;

        public DefaultResourceLocator(string resourceName, ResourceMeta resourceMeta)
            : base(resourceName, resourceMeta) { }

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
            return TypeBuilder.BuildType(ResourceMeta.ImplementationType);
        }

        #endregion

    }
}
