using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly,
        AllowMultiple = true, Inherited = false)]
    public class DefineChannelObserverAttribute
         : MapChannelObserverAttribute
    {
        private readonly Type _observerType;

        public DefineChannelObserverAttribute(Type channelType, Type observerType)
         : this(channelType, observerType, null) { }

        public DefineChannelObserverAttribute(Type channelType, Type observerType, string name)
         : base(channelType, name)
        {
            Guard.ArgumentNotNull(observerType, "observerType");
            _observerType = observerType;
        }

        #region overrides

        protected internal override IResourceLocator GetResourceLocator(Type targetType)
        {
            return base.GetResourceLocator(_observerType);
        }

        #endregion

    }
}
