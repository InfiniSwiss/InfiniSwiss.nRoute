using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public class DefaultChannelObserverLocator
         : ResourceLocatorBase<Object, ChannelObserverMeta>
    {
        private readonly Object _lock = new Object();
        private Object _instance;
        private IDisposable _instanceDisposer;

        public DefaultChannelObserverLocator(ChannelObserverMeta meta) :
            base(meta.ObserverName, meta)
        {
            Guard.ArgumentNotNull(meta, "meta");

            // if the lifetime is a singleton, then we initialize an instance immediately
            if (meta.Lifetime == InstanceLifetime.Singleton && meta.InitializationMode == InitializationMode.WhenAvailable)
            {
                InitializeInstance();
            }
        }

        #region Overrides

        public override object GetResourceInstance()
        {
            return InitializeInstance();
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
                        if (_instanceDisposer != null) _instanceDisposer.Dispose();
                        _instanceDisposer = null;
                        _instance = null;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private Object InitializeInstance()
        {
            if (ResourceMeta.Lifetime == InstanceLifetime.Singleton)
            {
                if (_instance != null) return _instance;
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = CreateObserverTypeInstance();
                        if (string.IsNullOrEmpty(ResourceMeta.ChannelKey))
                        {
                            _instanceDisposer = Channel.Subscribe(ResourceMeta.ChannelType, _instance, ResourceMeta.ThreadOption, false);
                        }
                        else
                        {
                            _instanceDisposer = Channel.Subscribe(ResourceMeta.ChannelType, ResourceMeta.ChannelKey, _instance, ResourceMeta.ThreadOption, false);
                        }
                    }
                }
                return _instance;
            }
            else
            {
                lock (_lock)
                {
                    var _observerInstance = CreateObserverTypeInstance();
                    if (string.IsNullOrEmpty(ResourceMeta.ChannelKey))
                    {
                        Channel.Subscribe(ResourceMeta.ChannelType, _observerInstance, ResourceMeta.ThreadOption, true);
                    }
                    else
                    {
                        Channel.Subscribe(ResourceMeta.ChannelType, ResourceMeta.ChannelKey, _observerInstance, ResourceMeta.ThreadOption, true);
                    }
                    return _observerInstance;
                }
            }
        }

        protected virtual Object CreateObserverTypeInstance()
        {
            return TypeBuilder.BuildType(ResourceMeta.ObserverType);
        }

        #endregion

    }
}