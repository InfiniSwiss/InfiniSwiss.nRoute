using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    public class ChannelObserverMeta
    {
        private readonly Type _channelType;
        private readonly Type _observerType;
        private readonly string _channelKey;
        private readonly string _observerName;
        private readonly InstanceLifetime _lifetime;
        private readonly ThreadOption _threadOption;
        private readonly InitializationMode _initializationMode;

        public ChannelObserverMeta(Type channelType, Type observerType, string channelKey, string observerName, ThreadOption threadOption,
            InitializationMode initializationMode, InstanceLifetime lifetime)
        {
            Guard.ArgumentNotNull(channelType, "channelType");
            Guard.ArgumentNotNull(observerType, "observerType");
            if (string.IsNullOrEmpty(observerName)) observerName = observerType.FullName;

            _channelType = channelType;
            _observerType = observerType;
            _channelKey = channelKey;
            _observerName = observerName;
            _lifetime = lifetime;
            _threadOption = threadOption;
            _initializationMode = initializationMode;
        }

        #region Properties

        public string ObserverName
        {
            get { return _observerName; }
        }

        public Type ChannelType
        {
            get { return _channelType; }
        }

        public Type ObserverType
        {
            get { return _observerType; }
        }

        public string ChannelKey
        {
            get { return _channelKey; }
        }

        public InitializationMode InitializationMode
        {
            get { return _initializationMode; }
        }

        public InstanceLifetime Lifetime
        {
            get { return _lifetime; }
        }

        public ThreadOption ThreadOption
        {
            get { return _threadOption; }
        }

        #endregion

    }
}
