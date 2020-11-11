using nRoute.Components.Messaging;

namespace nRoute.Components.Composition.Adapters
{
    public class ChannelLocatorAdapter<T>
        : LocatorAdapterBase<T, IChannel<T>>
        where
            T : class
    {
        public ChannelLocatorAdapter()
            : base((s) =>
            {
                if (string.IsNullOrEmpty(s))
                {
                    return Channel<T>.Public;
                }
                else
                {
                    return Channel<T>.Private[s];
                }
            })
        { }
    }
}
