using nRoute.Components.Composition;
using nRoute.Internal;
using System;

namespace nRoute.Components.Messaging
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = false)]
    public class ResolveChannelAttribute
        : ResolveResourceBaseAttribute
    {
        private static readonly Type ICHANNEL_GENERICTYPE = typeof(IChannel<>);
        private const string RESOLVETYPE_MUSTBE_ICHANNELTYPE = "Channel to resolve must be an IChannel<T> type, targeted type is '{0}'";

        public ResolveChannelAttribute()
            : base() { }

        public ResolveChannelAttribute(string key)
            : base(key)
        {
            Guard.ArgumentNotNullOrWhiteSpace(key, "key");
        }

        #region Properties

        public string Key
        {
            get { return base.ResourceName; }
        }

        #endregion

        #region Overrides

        protected internal override object ResolveResource(Type targetType)
        {
            Guard.ArgumentNotNull(targetType, "targetType");
            Guard.ArgumentValue(!targetType.IsGenericType || targetType.GetGenericTypeDefinition() != ICHANNEL_GENERICTYPE,
                RESOLVETYPE_MUSTBE_ICHANNELTYPE, targetType.FullName);

            var _channelType = targetType.GetGenericArguments()[0];
            return (string.IsNullOrEmpty(Key)) ? Channel.GetChannel(_channelType) : Channel.GetChannel(_channelType, Key);
        }

        #endregion

    }
}
