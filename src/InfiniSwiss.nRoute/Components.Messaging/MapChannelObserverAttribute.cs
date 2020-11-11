using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true, Inherited = false)]
    public class MapChannelObserverAttribute
         : MapResourceBaseAttribute
    {
        private const string MUST_BE_IMPLEMENTATION_TYPE = "{0} type must be an implementation of {1}.";
        private readonly static Type _observerGenericType = typeof(IObserver<Object>).GetGenericTypeDefinition();

        private readonly Type _channelType;
        private readonly string _name;

        public MapChannelObserverAttribute(Type channelType)
            : this(channelType, null) { }

        public MapChannelObserverAttribute(Type channelType, string name)
        {
            Guard.ArgumentNotNull(channelType, "channelType");

            _channelType = channelType;
            _name = name;
            Lifetime = InstanceLifetime.Singleton;
            ThreadOption = ThreadOption.PublisherThread;
        }

        #region Properties

        public string ChannelKey { get; set; }

        public InitializationMode InitializationMode { get; set; }

        public InstanceLifetime Lifetime { get; set; }

        public ThreadOption ThreadOption { get; set; }

        public bool IsDefault { get; set; }

        #endregion

        #region Overrides

        protected internal override bool IsDefaultResource
        {
            get { return IsDefault; }
        }

        protected internal override Type GetResourceType(Type targetType)
        {
            return _observerGenericType.MakeGenericType(_channelType);
        }

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            var _observerType = _observerGenericType.MakeGenericType(_channelType);
            if (!_observerType.IsAssignableFrom(targetType))
            {
                throw new InvalidOperationException(string.Format(MUST_BE_IMPLEMENTATION_TYPE,
                    targetType.FullName, _observerType.FullName));
            }

            return new DefaultChannelObserverLocator(new ChannelObserverMeta(_channelType, targetType, ChannelKey, _name,
                ThreadOption, InitializationMode, Lifetime));
        }

        #endregion

    }
}

