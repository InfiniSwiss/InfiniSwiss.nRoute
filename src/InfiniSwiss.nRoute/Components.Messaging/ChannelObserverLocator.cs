using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public static class ChannelObserverLocator
    {
        private readonly static Type _observerGenericType = typeof(IObserver<Object>).GetGenericTypeDefinition();

        #region Static Methods

        // Get ChannelObserver Related

        public static IObserver<T> GetChannelObserver<T>()
        {
            return ResourceLocator.GetResource<IObserver<T>>();
        }

        public static object GetChannelObserver(Type channelType)
        {
            return ResourceLocator.GetResource(GetObserverTypeForChannel(channelType));
        }

        public static IObserver<T> GetChannelObserver<T>(string name)
        {
            return ResourceLocator.GetResource<IObserver<T>>(name);
        }

        public static object GetChannelObserver(Type channelType, string name)
        {
            return ResourceLocator.GetResource(GetObserverTypeForChannel(channelType), name);
        }

        // Try Get ChannelObserver Related

        public static bool TryGetChannelObserver<T>(out IObserver<T> viewServiceInstance)
        {
            Object _service = null;
            var _response = ResourceLocator.TryGetResource(typeof(IObserver<T>), out _service);
            viewServiceInstance = _service == null ? null : (IObserver<T>)_service;
            return _response;
        }

        public static bool TryGetChannelObserver(Type channelType, out object viewServiceInstance)
        {
            return ResourceLocator.TryGetResource(GetObserverTypeForChannel(channelType), out viewServiceInstance);
        }

        public static bool TryGetChannelObserver<T>(string name, out IObserver<T> viewServiceInstance)
        {
            Object _service = null;
            var _response = ResourceLocator.TryGetResource(typeof(IObserver<T>), name, out _service);
            viewServiceInstance = _service == null ? null : (IObserver<T>)_service;
            return _response;
        }

        public static bool GetChannelObserver(Type channelType, string name, out object viewServiceInstance)
        {
            return ResourceLocator.TryGetResource(GetObserverTypeForChannel(channelType), name, out viewServiceInstance);
        }

        // Registration Related

        public static bool IsChannelObserverRegistered<T>()
        {
            return ResourceLocator.IsResourceRegistered(typeof(IObservable<T>), true);
        }

        public static bool IsChannelObserverRegistered(Type channelType)
        {
            return ResourceLocator.IsResourceRegistered(GetObserverTypeForChannel(channelType), true);
        }

        public static bool IsChannelObserverRegistered<T>(bool ensureHasDefaultChannelObserver)
        {
            return ResourceLocator.IsResourceRegistered(typeof(IObservable<T>), ensureHasDefaultChannelObserver);
        }

        public static bool IsChannelObserverRegistered(Type channelType, bool ensureHasDefaultChannelObserver)
        {
            return ResourceLocator.IsResourceRegistered(GetObserverTypeForChannel(channelType), ensureHasDefaultChannelObserver);
        }

        public static bool IsChannelObserverRegistered<T>(string name)
        {
            return ResourceLocator.IsResourceRegistered(typeof(IObserver<T>), name);
        }

        public static bool IsChannelObserverRegistered(Type channelType, string name)
        {
            return ResourceLocator.IsResourceRegistered(GetObserverTypeForChannel(channelType), name);
        }

        public static void SetDefaultChannelObserver<T>(string name)
        {
            ResourceLocator.SetDefaultResourceLocator(typeof(IObserver<T>), name);
        }

        public static void SetDefaultChannelObserver(Type channelType, string name)
        {
            ResourceLocator.SetDefaultResourceLocator(GetObserverTypeForChannel(channelType), name);
        }

        #endregion

        #region Helpers

        private static Type GetObserverTypeForChannel(Type channelType)
        {
            Guard.ArgumentNotNull(channelType, "channelType");
            return _observerGenericType.MakeGenericType(channelType);
        }

        #endregion

    }
}
